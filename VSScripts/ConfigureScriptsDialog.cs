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
    public partial class ConfigureScriptsDialog : Form
    {
        List<Script> _scripts;

        public List<Script> Scripts
        {
            get { return _scripts; }
        }

        private void RefreshScriptsList()
        {
            _scriptsList.Clear();

            for (int i = 0; i < _scripts.Count; ++i)
            {
                if (_scripts[i] != null)
                {
                    ListViewItem lvi = _scriptsList.Items.Add(string.Format("{0:D2} - {1}", i, _scripts[i].Caption));

                    lvi.Tag = i;
                }
            }
        }

        public ConfigureScriptsDialog(List<Script> scripts)
        {
            InitializeComponent();

            _scripts = new List<Script>(scripts);

            RefreshScriptsList();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            int index = -1;

            for (int i = 0; i < _scripts.Count; ++i)
            {
                if (_scripts[i] == null)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                MessageBox.Show(string.Format("This build has a limit of {0} scripts.\n\nRecompile the package if this isn't enough...", PkgCmdIDList.numScripts), "Script limit");
                return;
            }

            var d = new EditScriptDialog(new Script(), "Add", index);

            d.ShowDialog();

            if (d.DialogResult == DialogResult.OK)
            {
                _scripts[index] = d.Script;

                RefreshScriptsList();
            }
        }

        private void EditSelectedItem()
        {
            if (_scriptsList.SelectedItems.Count == 1)
            {
                ListViewItem lvi = _scriptsList.SelectedItems[0];
                var index = (int)lvi.Tag;

                var d = new EditScriptDialog(_scripts[index], "Edit", index);

                d.Text = string.Format("Edit Script {0}", (int)index);

                d.ShowDialog();

                if (d.DialogResult == DialogResult.OK)
                {
                    _scripts[index] = d.Script;

                    RefreshScriptsList();
                }
            }
        }

        private void _editButton_Click(object sender, EventArgs e)
        {
            EditSelectedItem();
        }

        private void _removeButton_Click(object sender, EventArgs e)
        {
            if (_scriptsList.SelectedItems.Count == 1)
            {
                ListViewItem lvi = _scriptsList.SelectedItems[0];

                if (MessageBox.Show("Remove this script from the menu?", "Remove?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _scripts[(int)lvi.Tag] = null;

                    RefreshScriptsList();
                }
            }
        }

        private void _scriptsList_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedItem();
        }
    }
}
