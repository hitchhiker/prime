namespace Prime.Core
{
    public interface IGetRealInstance
    {
        T GetRealInstance<T>() where T : class;
    }
}