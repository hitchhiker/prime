using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Prime.Common
{
    public interface IModelBase
    {
        ObjectId Id { get; set; }
    }
}
