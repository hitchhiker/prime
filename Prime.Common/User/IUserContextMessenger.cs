using System;

namespace Prime.Common
{
    public interface IUserContextMessenger : IDisposable
    {
        IUserContextMessenger GetInstance(UserContext context);
    }
}