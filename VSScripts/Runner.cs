using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.VSScripts
{
    class Runner
    {
        private Process _process;
        private StringBuilder _stdoutBuilder, _stderrBuilder;
        private string _stdout, _stderr;
        private int _exitCode;

        public string StdOut { get { return _stdout; } }
        public string StdErr { get { return _stderr; } }
        public int ExitCode { get { return _exitCode; } }

        public Runner(string cmd, string args)
        {
            _process = new Process();
            _stdoutBuilder = new StringBuilder();
            _stderrBuilder = new StringBuilder();

            _exitCode = -1;

            _process.StartInfo.FileName = cmd;

            if (args != null)
                _process.StartInfo.Arguments = args;

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.EnableRaisingEvents = true;

            _process.StartInfo.RedirectStandardInput = true;

            _process.StartInfo.RedirectStandardError = true;
            _process.ErrorDataReceived += this.OnErrorDataReceived;

            _process.StartInfo.RedirectStandardOutput = true;
            _process.OutputDataReceived += this.OnOutputDataReceived;
        }

        public void AddEnv(string key, string value)
        {
            if (key != null && value != null)
                _process.StartInfo.EnvironmentVariables.Add(key, value);
        }

        public bool Run(string stdin)
        {
            bool good = false;

            try
            {
                _process.Start();

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                if (stdin != null)
                    _process.StandardInput.Write(stdin);

                _process.StandardInput.Close();//^Z

                if (_process.WaitForExit(2500))
                    good = true;
                else
                    _process.Kill();

                _process.WaitForExit();
            }
            catch (System.Exception ex)
            {
                _stderrBuilder.Clear();
                _stderrBuilder.Append(ex.ToString());
            }

            _stdout = _stdoutBuilder.ToString();
            _stderr = _stderrBuilder.ToString();

            _exitCode = _process.ExitCode;
            _process.Close();
            _process = null;

            return good;
        }

        private void OnDataReceived(DataReceivedEventArgs e, StringBuilder b, string name)
        {
            if (e.Data == null)
            {
                //Debug.WriteLine(name + " OnDataReceived: got: null");
            }
            else
            {
                //Debug.WriteLine(name + " OnDataReceived: got: \"" + e.Data + "\"");

                if (b.Length > 0)
                    b.Append(Environment.NewLine);

                b.Append(e.Data);
            }
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(e, _stderrBuilder, "STDERR");
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(e, _stdoutBuilder, "STDOUT");
        }
    }
}
