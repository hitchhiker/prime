using System.Threading.Tasks;

namespace Prime.Common
{
    public static class TaskEx
    {
        public static Task Delay(int dueTime)
        {
            return Task.Delay(dueTime);
        }
    }
}