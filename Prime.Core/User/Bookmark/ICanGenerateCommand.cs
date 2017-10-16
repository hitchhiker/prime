using System;

namespace Prime.Core
{
    public interface ICanGenerateCommand
    {
        CommandContent GetPageCommand();
    }
}