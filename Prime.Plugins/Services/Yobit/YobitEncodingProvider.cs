using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Plugins.Services.Yobit
{
    public class YobitEncodingProvider : EncodingProvider
    {
        public override Encoding GetEncoding(string name)
        {
            return name.Equals("utf8") ? Encoding.UTF8 : null;
        }

        public override Encoding GetEncoding(int codepage)
        {
            return null;
        }
    }
}
