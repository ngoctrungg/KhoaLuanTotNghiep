using Facebook;
using KLTN_E.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KLTN_E.Controllers
{
    public class MessengerController : Controller
    {
        private readonly IConfiguration _configuration;

        public MessengerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private const string RedirectUri = "https://localhost:7121/GGLogin/Callback";
        public IActionResult Login()
        {
            var appId = _configuration.GetValue<string>("Messenger:AppId");
            var redirectUrl = $"https://www.facebook.com/v12.0/dialog/oauth?client_id={appId}&redirect_uri={RedirectUri}&scope=pages_manage_metadata,pages_read_engagement";
            return Redirect(redirectUrl);
        }

        public IActionResult Callback(string code)
        {
            var appId = _configuration.GetValue<string>("Messenger:AppId");
            var appSecret = _configuration.GetValue<string>("Messenger:AppSecret");
            var fbClient = new FacebookClient();
            dynamic result = fbClient.Get("oauth/access_token", new
            {
                client_id = appId,
                client_secret = appSecret,
                redirect_uri = RedirectUri,
                code
            });

            var accessToken = result.access_token;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveMessage([FromBody] MessengerMessageVM message)
        {
            // Kiểm tra xem tin nhắn có phải là từ Messenger không
            if (message != null && message.Object == "page")
            {
                foreach (var entry in message.Entry)
                {
                    foreach (var messagingEvent in entry.Messaging)
                    {
                        var senderId = messagingEvent.Sender.Id;
                        var recipientId = messagingEvent.Recipient.Id;
                        var timestamp = messagingEvent.Timestamp;
                        var messageText = messagingEvent.Message?.Text;

                        // Xử lý tin nhắn ở đây, ví dụ: trả lời lại tin nhắn
                        if (!string.IsNullOrEmpty(messageText))
                        {
                            await SendMessage(senderId, "Bạn vừa gửi tin nhắn: " + messageText);
                        }
                    }
                }
            }

            return Ok();
        }

        private async Task SendMessage(string recipientId, string messageText)
        {
            var accessToken = _configuration.GetValue<string>("Messenger:AccessToken");
            var fbClient = new FacebookClient(accessToken);

            var messageData = new
            {
                recipient = new { id = recipientId },
                message = new { text = messageText }
            };

            dynamic response = fbClient.Post("me/messages", messageData);
        }


    }
}
