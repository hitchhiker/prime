using System;

namespace Prime.Common
{
    public interface ICanGenerateCommand
    {
        CommandContent GetPageCommand();
    }
}