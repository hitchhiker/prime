using Prime.Core;

namespace Prime.Settings
{
    public class UpdateTimeKindResponseMessage : BaseTransportResponseMessage
    {
        public UpdateTimeKindResponseMessage(UpdateTimeKindRequestMessage requestMessage, string currentTime) : base(
            requestMessage)
        {
            CurrentTime = currentTime;
        }

        public string CurrentTime { get; set; }
    }
}
