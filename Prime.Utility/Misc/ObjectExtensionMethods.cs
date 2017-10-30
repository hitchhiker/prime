using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Utility.Misc
{
    public static class ObjectExtensionMethods
    {
        public static bool EqualOrBothNull(this object o1, object o2)
        {
            if (o1 == null && o2 == null)
                return true;
            if (o1 == null || o2 == null)
                return false;
            return Equals(o1, o2);
        }
    }
}
