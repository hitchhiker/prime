using LiteDB;

namespace Prime.Common
{
    public abstract class WindowInstanceBase : ModelBase
    {
        [Bson]
        public ObjectId UserId { get; set; }

        [Bson]
        public WindowOpenState OpenState { get; private set; }

        [Bson]
        public WindowOpenState PreviousOpenState { get; private set; }

        public bool IsFullscreen => OpenState == WindowOpenState.Fullscreen;

        public bool IsMinimised => OpenState == WindowOpenState.None;

        public bool IsMaximised => OpenState == WindowOpenState.Maximised;

        public bool IsNormal => OpenState == WindowOpenState.Normal;

        [Bson]
        public double Top { get; set; }

        [Bson]
        public double Left { get; set; }

        [Bson]
        public double Height { get; set; }

        [Bson]
        public double Width { get; set; }

        public void SetState(WindowOpenState state)
        {
            if (OpenState != WindowOpenState.None)
                PreviousOpenState = OpenState;

            OpenState = state;
        }
    }
}