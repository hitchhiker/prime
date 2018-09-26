using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public interface IExecute
    {
        void Start();

        void Cancel();

        int StageCount();
    }
}
