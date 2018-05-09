using System;

namespace Prime.Core
{
    [Flags]
    public enum TruncateOptions
    {
        None = 0x0,
        FinishWord = 0x1,
        AllowLastWordToGoOverMaxLength = 0x2,
        IncludeEllipsis = 0x4
    }
}