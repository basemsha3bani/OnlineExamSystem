param([Parameter(Mandatory = $true)][string]$ApplicationDll)
$ErrorActionPreference = 'Stop'
$applicationPath = (Resolve-Path -LiteralPath $ApplicationDll).Path
$smokeDirectory = Join-Path ([IO.Path]::GetTempPath()) ('ExamBrokerOutageSmoke_' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $smokeDirectory | Out-Null
$listeners = @()
try {
    foreach ($index in 1..3) {
        $listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
        $listener.Start()
        $listeners += $listener
    }
    $webPort = $listeners[0].LocalEndpoint.Port
    $brokerPort = $listeners[1].LocalEndpoint.Port
    $databasePort = $listeners[2].LocalEndpoint.Port
} finally { foreach ($listener in $listeners) { $listener.Stop() } }
@{ Logging = @{ LogLevel = @{ Default = 'Warning' }; EventLog = @{ LogLevel = @{ Default = 'None' } } }; RabbitMq = @{ Host = '127.0.0.1'; Port = $brokerPort } } |
    ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $smokeDirectory 'appsettings.json')
$overrides = @{
    'ASPNETCORE_ENVIRONMENT' = 'Development'
    'DOTNET_ENVIRONMENT' = 'Development'
    'RabbitMq__Host' = '127.0.0.1'
    'RabbitMq__Port' = [string]$brokerPort
    'ConnectionStrings__DbCoreConnectionString' = "Server=tcp:127.0.0.1,$databasePort;Database=BrokerOutageSmoke;Integrated Security=true;Connect Timeout=1;Encrypt=false"
    'ASPNETCORE_HTTPS_PORT' = $null
}
$previous = @{}
$applicationProcess = $null
try {
    foreach ($key in $overrides.Keys) {
        $previous[$key] = [Environment]::GetEnvironmentVariable($key, 'Process')
        [Environment]::SetEnvironmentVariable($key, $overrides[$key], 'Process')
    }
    $outputLog = Join-Path $smokeDirectory 'stdout.log'
    $errorLog = Join-Path $smokeDirectory 'stderr.log'
    $applicationProcess = Start-Process -FilePath (Get-Command dotnet).Source -ArgumentList @(('"' + $applicationPath + '"'), '--urls', "http://127.0.0.1:$webPort") -WorkingDirectory $smokeDirectory -WindowStyle Hidden -RedirectStandardOutput $outputLog -RedirectStandardError $errorLog -PassThru
    $ready = $false
    foreach ($attempt in 1..25) {
        if ($applicationProcess.HasExited) { throw 'Isolated web application exited during startup.' }
        try {
            $response = Invoke-WebRequest -UseBasicParsing -Uri "http://127.0.0.1:$webPort/Auth/Login" -TimeoutSec 2
            if ($response.StatusCode -eq 200) { $ready = $true; break }
        } catch { }
        Start-Sleep -Milliseconds 500
    }
    if (-not $ready) { throw 'Isolated web application did not become ready.' }
    Start-Sleep -Seconds 8
    if ($applicationProcess.HasExited) { throw 'Web application shut down while RabbitMQ was unavailable.' }
    $response = Invoke-WebRequest -UseBasicParsing -Uri "http://127.0.0.1:$webPort/Auth/Login" -TimeoutSec 3
    if ($response.StatusCode -ne 200) { throw 'Web application stopped serving requests during the broker outage.' }
    $retryCount = @(Select-String -LiteralPath $outputLog -Pattern 'Background operation failed; retrying').Count
    if ($retryCount -lt 2) { throw 'Expected broker/worker retries were not logged.' }
    Write-Output "PASS: isolated web host served HTTP 200 before and after broker/database failures; $retryCount retry events logged."
    Write-Output "Diagnostic logs: $smokeDirectory"
} catch {
    if (Test-Path -LiteralPath (Join-Path $smokeDirectory 'stderr.log')) { Get-Content -LiteralPath (Join-Path $smokeDirectory 'stderr.log') -Tail 12 }
    throw
} finally {
    if ($applicationProcess -and -not $applicationProcess.HasExited) { Stop-Process -Id $applicationProcess.Id -Force }
    foreach ($key in $previous.Keys) { [Environment]::SetEnvironmentVariable($key, $previous[$key], 'Process') }
}
