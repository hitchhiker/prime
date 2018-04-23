using System.Text;

namespace Prime.Finance.Services.Services.Yobit
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
