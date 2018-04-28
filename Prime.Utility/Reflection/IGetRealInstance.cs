namespace Prime.Common
{
    public interface IGetRealInstance
    {
        T GetRealInstance<T>() where T : class;
    }
}