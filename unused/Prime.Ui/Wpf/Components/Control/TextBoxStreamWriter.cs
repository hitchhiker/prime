#region

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using Prime.Utility;

#endregion

namespace prime
{
    public class TextBoxStreamWriter : TextWriter
    {
        private readonly TextBox _longOutput;
        private readonly TextBox _output;
        private StringBuilder _sb = new StringBuilder();
        private readonly Dispatcher _dispatcher;
        internal readonly Timer Timer;

        public TextBoxStreamWriter(TextBox output, TextBox longOutput, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _output = output;
            _longOutput = longOutput;
            var autoEvent = new AutoResetEvent(false);
            Timer = new Timer(TimerElapsed, autoEvent, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public override Encoding Encoding => Encoding.UTF8;

        private void TimerElapsed(object stateInfo)
        {
            base.Write("");
        }

        public override void WriteLine(string line)
        {
            if (line == null)
                return;

            base.WriteLine();

            if (line.StartsWith("*"))
            {
                Append(_longOutput, line.Substring(1));
                Append(_longOutput, Environment.NewLine);
            } else
            {
                 Append(_output,line);
                 Append(_output, Environment.NewLine);
            }
            _sb = new StringBuilder();
        }

        public override void Write(string line)
        {
            if (line == null)
                return;
            base.WriteLine();

            if (line.StartsWith("*"))
            {
                Append(_longOutput,line.Substring(1));
            } else
            {
                Append(_output, line);
            }
            _sb = new StringBuilder();
        }

        public override void Write(char value)
        {
            base.Write(value);
            _sb.Append(value.ToString());
        }

        public void Inline(string line)
        {
            Inline(_output, line);
        }

        private void Append(TextBox destination, string text)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => destination.AppendText(text)));
        }

        private void Inline(TextBox destination, string text)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                var io = destination.Text.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

                if (io == -1)
                    destination.Text = destination.Text + Environment.NewLine + text;
                else
                    destination.Text = destination.Text.Substring(0, io + Environment.NewLine.Length) + text;
            }));
        }

        public void Clear()
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => _output.Clear()));
        }
        public void Raw(string s)
        {
            _dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => _output.Text+=s));
        }
    }
}