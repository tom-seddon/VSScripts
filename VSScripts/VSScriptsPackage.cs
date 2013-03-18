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
    // (Regarding UICONTEXT_NoSolution - if a SLN is supplied on the command
    // line, the IDE appears to go through the NoSolution state before it
    // loads the SLN, so the package is autoloaded.)

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSScriptsPkgString)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]//UICONTEXT_NoSolution
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

            {
                Script s = new Script();

                s.Command = @"C:\tom\VSScripts\examples\InsertArrow.bat";
                s.Name = "Insert \"->\"";
                s.StdinMode = Script.InputMode.None;
                s.StdoutMode = Script.OutputMode.ReplaceSelection;
                s.StderrMode = Script.OutputMode.Discard;

                _scripts.Add(s);
            }

            {
                Script s = new Script();

                s.Command = @"C:\tom\VSScripts\examples\InsertThisArrow.bat";
                s.Name = "Insert \"this->\"";
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
                MenuCommand menuItem = new MenuCommand(HandleScriptsCmd, menuCommandID);
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
                RunScript(dte, script);
        }

        private void HandleScriptBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd;
            Script script;
            if (!GetMenuItemObjects(sender, out cmd, out script))
            {
                if (cmd != null)
                    cmd.Visible = false;

                return;
            }

            cmd.Visible = true;
            cmd.Text = script.Name;
        }

        private string GetStdin(DTE dte, Script.InputMode mode)
        {
            TextDocument td = dte.ActiveDocument.Object("TextDocument") as TextDocument;
            if (td != null)
            {
                switch (mode)
                {
                    case Script.InputMode.Selection:
                        {
                            if (td.Selection.Text.Length > 0)
                                return td.Selection.Text;
                        }
                        break;

                    case Script.InputMode.CurrentLine:
                        {
                            EditPoint a = td.CreateEditPoint(td.Selection.ActivePoint);
                            a.StartOfLine();

                            EditPoint b = td.CreateEditPoint(a);
                            a.EndOfLine();

                            return a.GetText(b);
                        }
                }
            }

            return null;
        }

        private void DoOutput(DTE dte, Script.OutputMode mode, string output)
        {
            switch (mode)
            {
                case Script.OutputMode.Discard:
                    break;

                case Script.OutputMode.StatusBar:
                    {
                        string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (lines.Length > 0)
                            dte.StatusBar.Text=lines[lines.Length-1].Trim();
                    }
                    break;

                case Script.OutputMode.ReplaceSelection:
                    {
                        TextSelection ts = dte.ActiveDocument.Selection as TextSelection;
                        if (ts == null)
                            return;

                        // http://msdn.microsoft.com/en-us/library/vstudio/envdte.vsepreplacetextoptions.aspx
                        vsEPReplaceTextOptions options = 0;

                        foreach (TextRange tr in ts.TextRanges)
                            tr.StartPoint.ReplaceText(tr.EndPoint, output, (int)options);
                    }
                    break;
            }
        }

        private void RunScript(DTE dte, Script script)
        {
            string stdin = GetStdin(dte, script.StdinMode);

            Runner r = new Runner("cmd", "/c " + script.Command);

            r.Run(stdin);

            dte.StatusBar.Text=string.Format("{0} {1} with exit code {2}",script.Name,r.ExitCode==0?"succeeded":"failed",r.ExitCode);

            DoOutput(dte, script.StdoutMode, r.StdOut);
            DoOutput(dte, script.StderrMode, r.StdErr);

            //             IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
        }

        private void HandleScriptsCmd(object sender, EventArgs e)
        {
        }

    }
}
