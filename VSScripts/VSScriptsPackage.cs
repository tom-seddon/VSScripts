using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Collections.Generic;

namespace Company.VSScripts
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSScriptsPkgString)]
    public sealed class VSScriptsPackage : Package
    {
        private List<Script> _scripts;
        private int _numCommands;

        private void LoadScriptsList()
        {
            _scripts = new List<Script>();

            {
                Script s = new Script();

                s.Command = @"C:\bin\gnuwin32\bin\unixsort.exe";
                s.Name = "Sort Lines";
                s.StdinMode = Script.InputMode.Selection;
                s.StdoutMode = Script.OutputMode.ReplaceSelection;
                s.StderrMode = Script.OutputMode.Discard;

                _scripts.Add(s);
            }

            {
                Script s = new Script();

                s.Command = @"C:\bin\uuidgen.py";
                s.Name = "Generate GUID";
                s.StdinMode = Script.InputMode.None;
                s.StdoutMode = Script.OutputMode.ReplaceSelection;
                s.StderrMode = Script.OutputMode.Discard;

                _scripts.Add(s);
            }
        }

        public VSScriptsPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            _scripts = null;
        }

        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVSScriptsCmdSet, (int)PkgCmdIDList.cmdidConfigureScripts);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);

                LoadScriptsList();

                // 50 = an arbitrary value. (Visual Studio is lame.)
                _numCommands = 50;

                for (int i = 0; i < _numCommands; ++i)
                {
                    var cmdID = new CommandID(GuidList.guidVSScriptsCmdSet, PkgCmdIDList.cmdidScript + i);
                    var cmd = new OleMenuCommand(new EventHandler(HandleScriptMenuItem), cmdID);
                    cmd.BeforeQueryStatus += new EventHandler(HandleScriptBeforeQueryStatus);
                    cmd.Visible = true;

                    mcs.AddCommand(cmd);

                    //HandleScriptBeforeQueryStatus(cmd, null);
                }
            }
        }

        private void InitScriptsList(OleMenuCommandService mcs)
        {
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private bool GetMenuItemObjects(object sender, out OleMenuCommand cmd, out Script script)
        {
            script = null;

            cmd = sender as OleMenuCommand;
            if (cmd == null)
                return false;

            int index = cmd.CommandID.ID - PkgCmdIDList.cmdidScript;
            if (index < 0 || index >= _numCommands)
                return false;//?!

            if (index >= _scripts.Count)
                return false;

            script = _scripts[index];
            return true;
        }

        private void HandleScriptMenuItem(object sender, EventArgs e)
        {
            OleMenuCommand cmd;
            Script script;
            if (!GetMenuItemObjects(sender, out cmd, out script))
                return;

            IVsStatusbar sb = GetService(typeof(IVsStatusbar)) as IVsStatusbar;

            DTE dte = GetService(typeof(DTE)) as DTE;
            if (dte == null)
                sb.SetText("DTE unavailable.");
            else
                sb.SetText(script.Command);
        }

        private void HandleScriptBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd;
            Script script;
            if (!GetMenuItemObjects(sender, out cmd, out script))
            {
//                 if (cmd != null)
//                     cmd.Visible = false;

                return;
            }

            cmd.Visible = true;
            cmd.Text = script.Name;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE dte = (DTE)GetService(typeof(DTE));

            Runner r = new Runner("cmd", "/c C:\\tom\\VSScripts\\tests\\test.py A B C D");

            r.Run();

            string statusText = string.Format("Exit code: {0}", r.ExitCode);

            if (r.StdErr.Length > 0)
            {
                string[] lines = r.StdErr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length > 0)
                    statusText = string.Format("{0}: {1}", r.ExitCode, lines[lines.Length - 1].Trim());
            }

            dte.StatusBar.Text = statusText;

            if (r.StdOut.Length > 0)
            {
                TextSelection ts = (TextSelection)dte.ActiveDocument.Selection;

                ts.Insert(r.StdOut);
            }

            //             Document doc=dte.ActiveDocument;
            // 
            //             // Show a Message Box to prove we were here
            //             IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            //             Guid clsid = Guid.Empty;
            //             int result;
            //             Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //                        0,
            //                        ref clsid,
            //                        "VSScripts",
            //                        string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
            //                        string.Empty,
            //                        0,
            //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //                        OLEMSGICON.OLEMSGICON_INFO,
            //                        0,        // false
            //                        out result));
        }

    }
}
