using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public interface IExecute
    {
        void Start();

        void Cancel();

        int StageCount();
    }
}
