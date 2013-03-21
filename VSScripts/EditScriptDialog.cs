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

        public Script Script
        {
            get { return _script; }
        }
        public EditScriptDialog(Script script, string reason, int index)
        {
            InitializeComponent();

            this.Text = string.Format("{0} Script {1:D2}", reason, index);

            //
            foreach (ComboBox combo in new ComboBox[] { _stderrCombo, _stdoutCombo })
            {
                combo.Items.Add(Script.OutputMode.Discard);
                combo.Items.Add(Script.OutputMode.ReplaceSelection);
                combo.Items.Add(Script.OutputMode.FirstLineToStatusBar);
                combo.Items.Add(Script.OutputMode.LastLineToStatusBar);
                combo.Items.Add(Script.OutputMode.ReplaceOutputWindow);
                combo.Items.Add(Script.OutputMode.AppendToOutputWindow);
            }

            _stdinCombo.Items.Add(Script.InputMode.None);
            _stdinCombo.Items.Add(Script.InputMode.CurrentLine);
            _stdinCombo.Items.Add(Script.InputMode.Selection);

            //
            _script = script.Clone();

            //
            _stdinCombo.SelectedItem = _script.StdinMode;
            _stdoutCombo.SelectedItem = _script.StdoutMode;
            _stderrCombo.SelectedItem = _script.StderrMode;
            _nameText.Text = script.Name;
            _commandText.Text = script.Command;
        }

        private T Unbox<T>(T x, object sel)
        {
            if (sel == null)
                return x;

            if (!(sel is T))
                return x;

            return (T)sel;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _script.StdinMode = Unbox(_script.StdinMode, _stdinCombo.SelectedItem);
            _script.StdoutMode = Unbox(_script.StdoutMode, _stdoutCombo.SelectedItem);
            _script.StderrMode = Unbox(_script.StderrMode, _stderrCombo.SelectedItem);

            _script.Name = _nameText.Text;
            _script.Command = _commandText.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void _findFileButton_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();

            d.Multiselect = false;
            d.RestoreDirectory = true;

            if (d.ShowDialog() == DialogResult.OK)
            {
                string quote = "";
                if (d.FileName.IndexOf(' ') >= 0)
                    quote = "\"";

                _commandText.SelectedText = quote + d.FileName + quote;
            }
        }
    }
}
