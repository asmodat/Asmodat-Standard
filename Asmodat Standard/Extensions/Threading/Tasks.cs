using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class TasksEx
    {
        public static T Await<T>(this Task<T> t)
        {
            t.Wait();
            return t.Result;
        }
    }
}
