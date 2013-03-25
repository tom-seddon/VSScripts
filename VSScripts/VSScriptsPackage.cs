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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Linq;

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
        private bool _enableDebugOutput;
        private List<Script> _scripts;

        public VSScriptsPackage()
        {
            //Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
#if DEBUG
            Debug.WriteLine("VSScripts: DEBUG.");
#else
            Debug.WriteLine("VSScripts: Not DEBUG.");
#endif

            _scripts = null;

#if DEBUG
            _enableDebugOutput = true;
#else//DEBUG
            _enableDebugOutput=false;
#endif//DEBUG
        }

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

        private void SendToStatusBar(DTE2 dte, string output, bool first)
        {
            List<string> lines = new List<string>(from x in output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None) where !string.IsNullOrWhiteSpace(x) select x.Trim());

            if (lines.Count > 0)
            {
                int index;
                if (first)
                    index = 0;
                else
                    index = lines.Count - 1;

                dte.StatusBar.Text = lines[index].Trim();
            }
        }

        private string ToString(EditPoint e)
        {
            return string.Format("AbsoluteCharOffset={0} AtEndOfDocument={1} AtEndOfLine={2} AtStartOfDocument={3} AtStartOfLine={4} DisplayColumn={5} Line={6} LineCharOffset={7} LineLength={8}",
                e.AbsoluteCharOffset, e.AtEndOfDocument, e.AtEndOfLine, e.AtStartOfDocument, e.AtStartOfLine, e.DisplayColumn, e.Line, e.LineCharOffset, e.LineLength);
        }

        private string ToString(TextPoint e)
        {
            return string.Format("AbsoluteCharOffset={0} AtEndOfDocument={1} AtEndOfLine={2} AtStartOfDocument={3} AtStartOfLine={4} DisplayColumn={5} Line={6} LineCharOffset={7} LineLength={8}",
                e.AbsoluteCharOffset, e.AtEndOfDocument, e.AtEndOfLine, e.AtStartOfDocument, e.AtStartOfLine, e.DisplayColumn, e.Line, e.LineCharOffset, e.LineLength);
        }

        // http://stackoverflow.com/questions/2413530/find-an-ivstextview-or-iwpftextview-for-a-given-projectitem-in-vs-2010-rc-exten
        private IWpfTextView GetWPFTextView(IVsTextView vsTextView)
        {
            var userData = vsTextView as IVsUserData;
            if (userData == null)
                return null;

            Guid guid = DefGuidList.guidIWpfTextViewHost;
            object holder;
            userData.GetData(ref guid, out holder);

            if (holder == null)
                return null;

            var host = holder as IWpfTextViewHost;
            if (host == null)
                return null;

            return host.TextView;
        }

        private void ReplaceBoxSelection(DTE2 dte, string text)
        {
            StreamWriter debug = new StreamWriter(_enableDebugOutput ? new OutputWindowStream(dte, "VSScriptsDebug") : Stream.Null);

            int result;

            var textManager = GetService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null)
                return;

            IVsTextView textView;
            result = textManager.GetActiveView(1, null, out textView);
            if (result != VSConstants.S_OK)
                return;

            IWpfTextView wpfTextView = GetWPFTextView(textView);
            if (wpfTextView == null)
                return;

            ITextBuffer buffer = wpfTextView.TextBuffer;

            ITextEdit edit = wpfTextView.TextBuffer.CreateEdit();


            ITextSelection selection = wpfTextView.Selection;

            NormalizedSnapshotSpanCollection spans = selection.SelectedSpans;
            ReadOnlyCollection<VirtualSnapshotSpan> vspans = selection.VirtualSelectedSpans;

            debug.Write("Spans:\n");

            for (int i = 0; i < spans.Count; ++i)
            {
                SnapshotSpan span = spans[i];

                debug.WriteLine("    [{0}]: {1}", i, span);//Start={1} End={2} Length={3} IsEmpty={4}\n",i,span.Start,
            }

            debug.WriteLine("Virtual Spans:\n");

            for (int i = 0; i < vspans.Count; ++i)
            {
                VirtualSnapshotSpan vspan = vspans[i];
                SnapshotSpan span = vspan.SnapshotSpan;

                debug.WriteLine("    [{0}]: {1} (IsInVirtualSpace={2}) (Start VirtualSpaces={3} End VirtualSpaces={4})\n", i, vspan, vspan.IsInVirtualSpace, vspan.Start.VirtualSpaces, vspan.End.VirtualSpaces);
            }

            bool good = true;

            foreach (VirtualSnapshotSpan vspan in vspans)
            {
                if (vspan.Start.VirtualSpaces > 0)
                {
                    // *sigh* - not sure why Visual Studio can't just do this
                    // for you. Then maybe it could respect the tabs/spaces
                    // option.
                    string spaces = new string(' ', vspan.Start.VirtualSpaces);

                    if (!edit.Insert(vspan.Start.Position, spaces))
                        good = false;

                    // Having inserted a few spaces, that's enough for the
                    // virtual span's snapshot span to be valid as something to
                    // replace.
                }

                if (!edit.Replace(vspan.SnapshotSpan, text))
                    good = false;
            }

            if (good)
                edit.Apply();
            else
                edit.Cancel();

            debug.Flush();
        }

        private void SendToOutputWindow(DTE2 dte, string output, bool clearFirst)
        {
            var stream = new OutputWindowStream(dte, "VSScripts");

            if (clearFirst)
                stream.ClearPane();

            var writer = new StreamWriter(stream);

            writer.Write(output);
            writer.Flush();
        }

        private void DoOutput(DTE2 dte, Script.OutputMode mode, string output)
        {
            switch (mode)
            {
                case Script.OutputMode.Discard:
                    break;

                case Script.OutputMode.ReplaceOutputWindow:
                    SendToOutputWindow(dte, output, true);
                    break;

                case Script.OutputMode.AppendToOutputWindow:
                    SendToOutputWindow(dte, output, false);
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

                        if (ts.Mode == vsSelectionMode.vsSelectionModeStream)
                            ts.DestructiveInsert(output);
                        else
                            ReplaceBoxSelection(dte, output);
                    }
                    break;
            }
        }

        private void FillEnvironment(DTE2 dte, Runner r)
        {
            r.AddEnv("FullPath", dte.ActiveDocument.FullName);
            r.AddEnv("Filename", Misc.GetPathFileNameWithoutExtension(dte.ActiveDocument.FullName));
            r.AddEnv("Extension", Misc.GetPathExtension(dte.ActiveDocument.FullName));
            r.AddEnv("Directory", Misc.GetPathDirectoryName(dte.ActiveDocument.FullName));
            r.AddEnv("RootDir", Misc.GetPathRoot(dte.ActiveDocument.FullName));
            r.AddEnv("TabSize", dte.ActiveDocument.TabSize.ToString());
        }

        private void RunScript(DTE2 dte, Script script)
        {
            string stdin = GetStdin(dte, script.StdinMode);

            Runner r = new Runner("cmd", "/c " + script.Command);

            FillEnvironment(dte, r);

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
