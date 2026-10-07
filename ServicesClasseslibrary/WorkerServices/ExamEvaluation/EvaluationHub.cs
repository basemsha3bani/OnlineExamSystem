using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ServicesClasseslibrary.WorkerServices.ExamEvaluation
{
    public class EvaluationHub : Hub
    {
        // Client will listen to this
        public async Task SendEvaluationCompleted(string candidateId, string examTitle)
        {
            await Clients.User(candidateId).SendAsync("EvaluationCompleted", examTitle);
        }
        public override async Task OnConnectedAsync()
        {
            var candidateId = Context.GetHttpContext().Request.Query["candidateId"];
            if (!string.IsNullOrEmpty(candidateId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"candidate_{candidateId}");
            }
            await base.OnConnectedAsync();
        }
    }
}
