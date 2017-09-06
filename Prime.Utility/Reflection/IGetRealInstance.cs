namespace Prime.Utility
{
    public interface IGetRealInstance
    {
        T GetRealInstance<T>() where T : class;
    }
}