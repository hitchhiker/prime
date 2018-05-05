using System;

namespace Prime.Core
{
    public interface IUserContextMessenger : IDisposable
    {
        IUserContextMessenger GetInstance(UserContext context);
    }
}