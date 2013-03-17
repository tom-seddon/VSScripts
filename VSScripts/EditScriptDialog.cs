using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Company.VSScripts
{
    public partial class EditScriptDialog : Form
    {
        private Script _script;

        public EditScriptDialog(Script script)
        {
            InitializeComponent();

            //
            _stderrCombo.Items.Add(Script.OutputMode.Discard);
            _stderrCombo.Items.Add(Script.OutputMode.ReplaceSelection);
            _stderrCombo.Items.Add(Script.OutputMode.StatusBar);

            _stdoutCombo.Items.Add(Script.OutputMode.Discard);
            _stdoutCombo.Items.Add(Script.OutputMode.ReplaceSelection);
            _stdoutCombo.Items.Add(Script.OutputMode.StatusBar);

            _stdinCombo.Items.Add(Script.InputMode.None);
            _stdinCombo.Items.Add(Script.InputMode.CurrentLine);
            _stdinCombo.Items.Add(Script.InputMode.Selection);

            //
            _script = script;

            //
            _stdinCombo.SelectedValue = _script.StdinMode;
            _stdoutCombo.SelectedValue = _script.StdoutMode;
            _stderrCombo.SelectedValue = _script.StderrMode;
            _nameText.Text = script.Name;
            _commandText.Text = script.Command;

        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _script.StdinMode = (Script.InputMode)_stdinCombo.SelectedValue;
            _script.StdoutMode = (Script.OutputMode)_stdoutCombo.SelectedValue;
            _script.StderrMode = (Script.OutputMode)_stderrCombo.SelectedValue;
            _script.Name = _nameText.Text;
            _script.Command = _commandText.Text;
        }
    }
}
