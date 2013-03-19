using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.VSScripts
{
    public class Script
    {
        public enum OutputMode
        {
            Discard,
            FirstLineToStatusBar,
            LastLineToStatusBar,
            ReplaceSelection,
            ReplaceOutputWindow,
            AppendToOutputWindow,
        }

        public enum InputMode
        {
            None,
            CurrentLine,
            Selection,
        }

        struct ScriptData
        {
            public InputMode stdinMode;
            public OutputMode stderrMode;
            public OutputMode stdoutMode;
            public string name;
            public string command;
        }

        public InputMode StdinMode
        {
            get { return _data.stdinMode; }
            set { _data.stdinMode = value; }
        }

        public OutputMode StderrMode
        {
            get { return _data.stderrMode; }
            set { _data.stderrMode = value; }
        }

        public OutputMode StdoutMode
        {
            get { return _data.stdoutMode; }
            set { _data.stdoutMode = value; }
        }

        public string Name
        {
            get { return _data.name; }
            set { _data.name = value; }
        }

        public string Command
        {
            get { return _data.command; }
            set { _data.command = value; }
        }

        public string Caption
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                    return Name;
                else
                    return Command;
            }
        }

        private ScriptData _data;

        public Script()
        {
            _data.stdinMode = InputMode.None;
            _data.stderrMode = OutputMode.LastLineToStatusBar;
            _data.stdoutMode = OutputMode.ReplaceSelection;
            _data.name = null;
            _data.command = null;
        }

        public Script Clone()
        {
            var s = new Script();

            s._data = _data;

            return s;
        }
    }
}
