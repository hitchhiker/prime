using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.KeysManager.Messages
{
    class TestPrivateApiMessage
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}
