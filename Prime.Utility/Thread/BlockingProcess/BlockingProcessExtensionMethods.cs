using System;
using System.Threading;

namespace Prime.Utility
{
    public static class BlockingProcessExtensionMethods
    {
        public static void WaitFor(this IHasProcessState processState, ProcessState targetState, TimeSpan timeOut)
        {
            if (processState.ProcessState == targetState)
                return;

            do
            {
                Thread.Sleep(1);
            } while (processState.ProcessState != targetState);
        }

        public static void Wait(this IHasProcessState processState, TimeSpan timeOut)
        {
            if (processState.ProcessState == ProcessState.Failed || processState.ProcessState == ProcessState.Success)
                return;

            do
            {
                Thread.Sleep(1);
            } while (processState.ProcessState != ProcessState.Failed && processState.ProcessState != ProcessState.Success);
        }

        public static bool IsFinished(this IHasProcessState processState)
        {
            return processState?.ProcessState == ProcessState.Failed || processState?.ProcessState == ProcessState.Success;
        }

        public static bool IsSuccess(this IHasProcessState processState)
        {
            return processState?.ProcessState == ProcessState.Success;
        }

        public static bool IsFailed(this IHasProcessState processState)
        {
            return processState?.ProcessState == ProcessState.Success;
        }
    }
}