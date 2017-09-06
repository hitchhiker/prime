using System.Threading.Tasks;

namespace Prime.Utility
{
    public static class TaskEx
    {
        public static Task Delay(int dueTime)
        {
            return Task.Delay(dueTime);
        }
    }
}