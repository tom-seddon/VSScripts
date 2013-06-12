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
        private ProcessStartInfo _startInfo;
        private StringBuilder _stdoutBuilder, _stderrBuilder;
        private string _stdout, _stderr;
        private int _exitCode;

        public string StdOut { get { return _stdout; } }
        public string StdErr { get { return _stderr; } }
        public int ExitCode { get { return _exitCode; } }

        public Runner(string cmd, string args)
        {
            _stdoutBuilder = new StringBuilder();
            _stderrBuilder = new StringBuilder();

            _exitCode = -1;

            _startInfo = new ProcessStartInfo();
            
            _startInfo.FileName = cmd;

            if (args != null)
                _startInfo.Arguments = args;

            _startInfo.UseShellExecute = false;
            _startInfo.CreateNoWindow = true;

            _startInfo.RedirectStandardInput = true;

            _startInfo.RedirectStandardError = true;

            _startInfo.RedirectStandardOutput = true;
        }

        public void AddEnv(string key, string value)
        {
            if (key != null && value != null)
                _startInfo.EnvironmentVariables.Add(key, value);
        }

        public bool Run(string stdin)
        {
            bool good = false;

            using (var process = new Process())
            {
                process.StartInfo = _startInfo;

                process.EnableRaisingEvents = true;
                process.ErrorDataReceived += this.OnErrorDataReceived;
                process.OutputDataReceived += this.OnOutputDataReceived;

                try
                {
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (stdin != null)
                        process.StandardInput.Write(stdin);

                    process.StandardInput.Close();//^Z

                    if (process.WaitForExit(2500))
                        good = true;
                    else
                        process.Kill();
                }
                catch (System.Exception ex)
                {
                    _stderrBuilder.Clear();
                    _stderrBuilder.Append(ex.ToString());
                }

                _stdout = _stdoutBuilder.ToString();
                _stderr = _stderrBuilder.ToString();

                _exitCode = process.ExitCode;
            }

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
