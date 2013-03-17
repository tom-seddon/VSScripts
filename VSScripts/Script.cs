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
            StatusBar,
            ReplaceSelection,
        }

        public enum InputMode
        {
            None,
            CurrentLine,
            Selection,
        }

        public InputMode StdinMode;
        public OutputMode StdoutMode;
        public OutputMode StderrMode;
        public string Name;
        public string Command;

        public Script()
        {
            StdinMode = InputMode.None;
            StdoutMode = OutputMode.ReplaceSelection;
            StderrMode = OutputMode.StatusBar;
        }
    }
}
