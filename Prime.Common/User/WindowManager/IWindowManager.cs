using System;
using System.Collections.Generic;

namespace Prime.Common
{
    public interface IWindowManager
    {
        void Init(object state);

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