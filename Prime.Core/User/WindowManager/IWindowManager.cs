using System;
using System.Collections.Generic;

namespace Prime.Core
{
    public interface IWindowManager
    {
        void Init();

        void CreateNewWindow();

        void Toggle();

        void MinimiseAll();

        void ShowAll();

        bool CanSpawn();

        bool CanMinimiseAll();

        bool CanShowAll();

        void Close(Object window);
    }
}