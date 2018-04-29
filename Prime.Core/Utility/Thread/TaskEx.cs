using System.Threading.Tasks;

namespace Prime.Core
{
    public static class TaskEx
    {
        public static Task Delay(int dueTime)
        {
            return Task.Delay(dueTime);
        }
    }
}