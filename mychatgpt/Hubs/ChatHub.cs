using Microsoft.AspNetCore.SignalR;
using mychatgpt.APIServices;
using System.Text.RegularExpressions;

namespace mychatgpt.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GPTAPIService _gptService;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(GPTAPIService gptService, ILogger<ChatHub> logger)
        {
            _gptService = gptService;
            _logger = logger;
        }
        public async Task SetUserParameters()
        {
            Guid currentUserRoomId = Guid.NewGuid();

            await Groups.AddToGroupAsync(Context.ConnectionId, currentUserRoomId.ToString());

            await Clients.Caller.SendAsync("ReceiveUserParams", Guid.NewGuid().ToString(), currentUserRoomId.ToString());
        }

        public async Task SendToServer(string message, string roomId)
        {
            var responseMessage = await _gptService.postSend(message);

            if (responseMessage != null)
            {
                await Clients.Group(roomId).SendAsync("ReceiveFromServer", responseMessage);
            }
            else
            {
                _logger.LogInformation("error");
            }
        }

    }
}
