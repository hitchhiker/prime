using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Chat.Net.Interfaces;
using Rocket.Chat.Net.Models;
using Rocket.Chat.Net.Models.MethodResults;

namespace chat
{
    public class ChatContext
    {
        public string Username { get; set; } = "m@silvenga.com";
        public string Password { get; set; } = "silverlight";
        public string RocketServerUrl { get; set; } = "demo.rocket.chat:3000";
        public string RoomName { get; private set; } = "general";
        public string RoomId { get; private set; }
        public bool UseSSL { get; set; }
        public bool IsInitialised { get; set; }

        public IRocketChatDriver Driver { get; set; }

        public async Task<bool> ChangeRoom(string roomName)
        {
            if (Driver == null)
                return false;

            var roomIdResult = await Driver.GetRoomIdAsync(roomName);

            if (roomIdResult.HasError)
                return false;

            RoomId = roomIdResult.Id;
            RoomName = roomName;
            return true;
        }

        public async Task<MethodResult<RocketMessage>> MessageWrite(string message)
        {
            if (Driver == null)
                return null;

            return await Driver.SendMessageAsync(message, RoomId);
        }

        public Action<RocketMessage> MessageRead { get; set; }
    }
}
