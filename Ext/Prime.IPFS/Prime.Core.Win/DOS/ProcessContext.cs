namespace Prime.IPFS
{
    public abstract class ProcessContext
    {
        public readonly string Command;

        protected ProcessContext(string command)
        {
            Command = command;
        }
    }
}