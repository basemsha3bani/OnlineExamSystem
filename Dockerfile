FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OnlineExamSystem/OnlineExamSystem.csproj", "OnlineExamSystem/"]
RUN dotnet restore "OnlineExamSystem/OnlineExamSystem.csproj"
COPY . .
WORKDIR "/src/OnlineExamSystem"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "OnlineExamSystem.dll"]