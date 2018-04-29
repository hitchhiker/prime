using System;

namespace Prime.Core
{
    public interface ICanStop
    {
        Action OnStopped { get; set; }
    }
}