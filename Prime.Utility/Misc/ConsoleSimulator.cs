using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prime.Utility
{
    public class ConsoleSimulator
    {
        private readonly List<string> _l = new List<string>();

        public void Write(int space, object obj)
        {
            var str = obj.ToString();
            Write(str);
            if (str.Length < space)
                Write(new string(' ', space - str.Length));
        }

        public void Write(object obj)
        {
            if (obj == null)
                return;

            if (!_l.Any())
            {
                _l.Add(obj.ToString());
                return;
            }

            _l[_l.Count - 1] = _l[_l.Count - 1] + obj;
        }

        public void WriteLine(object obj = null)
        {
            Write(obj);
            _l.Add(string.Empty);
        }

        public void WriteLines(IEnumerable<object> objs = null)
        {
            if (objs == null)
                return;
            foreach (var o in objs)
                WriteLine(o);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _l);
        }


        public void At(int index)
        {
            if (!_l.Any())
            {
                _l.Add(new string(' ', index));
                return;
            }

            var line = _l[_l.Count - 1];

            var lineIndex = line.Length - 1;

            if (lineIndex == index)
                return;

            if (lineIndex > index)
                line = line.Substring(0, index + 1);
            else
                line = line + new string(' ', index- lineIndex);

            _l[_l.Count - 1] = line;
        }
    }
}
