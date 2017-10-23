using System;

namespace Prime.Common
{
    public interface ICanStop
    {
        Action OnStopped { get; set; }
    }
}