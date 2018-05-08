using Rocket.Chat.Net.Driver;
using Rocket.Chat.Net.Interfaces;
using Rocket.Chat.Net.Models.LoginOptions;

namespace chat
{
    public static class ChatCore
    {
        public static async void Initialise(ChatContext context)
        {
            ILoginOption loginOption = new LdapLoginOption
            {
                Username = context.Username,
                Password = context.Password
            };

            IRocketChatDriver driver = new RocketChatDriver(context.RocketServerUrl, context.UseSSL);

            await driver.ConnectAsync(); // Connect to server
            await driver.LoginAsync(loginOption); // Login via LDAP
            await driver.SubscribeToRoomAsync(); // Listen on all rooms

            driver.MessageReceived += message =>
            {
                context.MessageRead?.Invoke(message);
            };
        }
    }
}