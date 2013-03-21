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
using EnvDTE80;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Company.VSScripts
{
    // (Regarding UICONTEXT_NoSolution - if a SLN is supplied on the command
    // line, the IDE appears to go through the NoSolution state before it
    // loads the SLN, so the package is autoloaded.)

    [ProvideLoadKey("Standard", "0.1", "VSScripts", "Tom Seddon", 104)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSScriptsPkgString)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]//UICONTEXT_NoSolution
    public sealed class VSScriptsPackage : Package
    {
        private List<Script> _scripts;

        private void SaveScriptsList()
        {
            Misc.SaveXml(GetScriptsListFileName(), _scripts);
        }

        private void LoadScriptsList()
        {
            _scripts = Misc.LoadXmlOrCreateDefault<List<Script>>(GetScriptsListFileName());

            // Ensure there are always as many entries as there are possible
            // scripts - this isn't strictly necessary but it simplifies some of
            // the maintenance code.
            while (_scripts.Count < PkgCmdIDList.numScripts)
                _scripts.Add(null);
        }

        private string GetScriptsListFileName()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VSScripts.xml");
        }

        //             _scripts = new List<Script>();
        // 
        //             {
        //                 Script s = new Script();
        // 
        //                 s.Command = @"C:\bin\gnuwin32\bin\unixsort.exe";
        //                 s.Name = "Sort Lines";
        //                 s.StdinMode = Script.InputMode.Selection;
        //                 s.StdoutMode = Script.OutputMode.ReplaceSelection;
        //                 s.StderrMode = Script.OutputMode.Discard;
        // 
        //                 _scripts.Add(s);
        //             }
        // 
        //             {
        //                 Script s = new Script();
        // 
        //                 s.Command = @"C:\bin\uuidgen.py";
        //                 s.Name = "Generate GUID";
        //                 s.StdinMode = Script.InputMode.None;
        //                 s.StdoutMode = Script.OutputMode.ReplaceSelection;
        //                 s.StderrMode = Script.OutputMode.Discard;
        // 
        //                 _scripts.Add(s);
        //             }
        // 
        //             {
        //                 Script s = new Script();
        // 
        //                 s.Command = @"C:\tom\VSScripts\examples\InsertArrow.bat";
        //                 s.Name = "Insert \"->\"";
        //                 s.StdinMode = Script.InputMode.None;
        //                 s.StdoutMode = Script.OutputMode.ReplaceSelection;
        //                 s.StderrMode = Script.OutputMode.Discard;
        // 
        //                 _scripts.Add(s);
        //             }
        // 
        //             {
        //                 Script s = new Script();
        // 
        //                 s.Command = @"C:\tom\VSScripts\examples\InsertThisArrow.bat";
        //                 s.Name = "Insert \"this->\"";
        //                 s.StdinMode = Script.InputMode.None;
        //                 s.StdoutMode = Script.OutputMode.ReplaceSelection;
        //                 s.StderrMode = Script.OutputMode.Discard;
        // 
        //                 _scripts.Add(s);
        //             }
        // 
        //             {
        //                 Script s = new Script();
        // 
        //                 s.Command = @"SET";
        //                 s.Name = "Show environment";
        //                 s.StdinMode = Script.InputMode.None;
        //                 s.StdoutMode = Script.OutputMode.ReplaceOutputWindow;
        //                 s.StderrMode = Script.OutputMode.Discard;
        // 
        //                 _scripts.Add(s);
        //             }
        //         }

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

                for (int i = 0; i < PkgCmdIDList.numScripts; ++i)
                {
                    var cmdID = new CommandID(GuidList.guidVSScriptsCmdSet, PkgCmdIDList.cmdidScript0 + i);
                    var cmd = new OleMenuCommand(new EventHandler(HandleScriptMenuItem), cmdID);
                    cmd.BeforeQueryStatus += new EventHandler(HandleScriptBeforeQueryStatus);
                    cmd.Visible = true;

                    mcs.AddCommand(cmd);
                }
            }
        }

        private bool GetMenuItemObjects(object sender, out OleMenuCommand cmd, out Script script)
        {
            script = null;

            cmd = sender as OleMenuCommand;
            if (cmd == null)
                return false;

            int index = cmd.CommandID.ID - PkgCmdIDList.cmdidScript0;
            if (index < 0 || index >= PkgCmdIDList.numScripts)
                return false;//?!

            if (index >= _scripts.Count)
                return false;

            script = _scripts[index];

            if (script == null)
                return false;

            return true;
        }

        private void HandleScriptMenuItem(object sender, EventArgs e)
        {
            OleMenuCommand cmd;
            Script script;
            if (!GetMenuItemObjects(sender, out cmd, out script))
                return;

            IVsStatusbar sb = GetService(typeof(IVsStatusbar)) as IVsStatusbar;

            DTE2 dte = GetService(typeof(DTE)) as DTE2;
            if (dte == null)
                sb.SetText("Internal error - DTE unavailable.");
            else if (dte.ActiveDocument == null)
                sb.SetText("There is no active document.");
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
            cmd.Text = script.Caption;
        }

        private string GetStdin(DTE2 dte, Script.InputMode mode)
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

        private OutputWindowPane FindOrCreateOutputWindowPane(DTE2 dte, string name)
        {
            foreach (OutputWindowPane pane in dte.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Name == name)
                    return pane;
            }

            return dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(name);
        }

        private void SendToStatusBar(DTE2 dte, string output, bool first)
        {
            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 0)
            {
                int index;
                if (first)
                    index = 0;
                else
                    index = lines.Length - 1;

                dte.StatusBar.Text = lines[index].Trim();
            }
        }

        private void DoOutput(DTE2 dte, Script.OutputMode mode, string output)
        {
            switch (mode)
            {
                case Script.OutputMode.Discard:
                    break;

                case Script.OutputMode.ReplaceOutputWindow:
                    {
                        OutputWindowPane pane = FindOrCreateOutputWindowPane(dte, "VSScripts");

                        pane.Clear();

                        goto case Script.OutputMode.AppendToOutputWindow;
                    }

                case Script.OutputMode.AppendToOutputWindow:
                    {
                        OutputWindowPane pane = FindOrCreateOutputWindowPane(dte, "VSScripts");

                        pane.OutputString(output);
                    }
                    break;

                case Script.OutputMode.FirstLineToStatusBar:
                    SendToStatusBar(dte, output, true);
                    break;

                case Script.OutputMode.LastLineToStatusBar:
                    SendToStatusBar(dte, output, false);
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

        private void RunScript(DTE2 dte, Script script)
        {
            string stdin = GetStdin(dte, script.StdinMode);

            Runner r = new Runner("cmd", "/c " + script.Command);

            r.AddEnv("FullPath", dte.ActiveDocument.FullName);
            r.AddEnv("Filename", Misc.GetPathFileNameWithoutExtension(dte.ActiveDocument.FullName));
            r.AddEnv("Extension", Misc.GetPathExtension(dte.ActiveDocument.FullName));
            r.AddEnv("Directory", Misc.GetPathDirectoryName(dte.ActiveDocument.FullName));
            r.AddEnv("RootDir", Misc.GetPathRoot(dte.ActiveDocument.FullName));

            r.Run(stdin);

            dte.StatusBar.Text = string.Format("{0} {1} with exit code {2}", script.Name, r.ExitCode == 0 ? "succeeded" : "failed", r.ExitCode);

            dte.UndoContext.Open(script.Name, false);

            try
            {
                DoOutput(dte, script.StdoutMode, r.StdOut);
                DoOutput(dte, script.StderrMode, r.StdErr);
            }
            finally
            {
                dte.UndoContext.Close();
            }

            //             IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
        }

        private void HandleScriptsCmd(object sender, EventArgs e)
        {
            var d = new ConfigureScriptsDialog(_scripts);

            d.ShowDialog();

            if (d.DialogResult == DialogResult.OK)
            {
                _scripts = d.Scripts;

                SaveScriptsList();
            }
        }
    }
}
