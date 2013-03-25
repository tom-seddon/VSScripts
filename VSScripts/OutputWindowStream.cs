using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Company.VSScripts
{
    public class OutputWindowStream : Stream
    {
        //////////////////////////////////////////////////////////////////////////

        public OutputWindowStream(DTE2 dte2, string paneName)
        {
            _dte2 = dte2;
            _paneName = paneName;
            _pane = null;
        }

        //////////////////////////////////////////////////////////////////////////

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public override void Flush()
        {
            // ...
        }

        //////////////////////////////////////////////////////////////////////////

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        //////////////////////////////////////////////////////////////////////////

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        //////////////////////////////////////////////////////////////////////////

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        //////////////////////////////////////////////////////////////////////////

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        //////////////////////////////////////////////////////////////////////////

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsurePaneCreated();

            String str = System.Text.Encoding.ASCII.GetString(buffer, offset, count);
            _pane.OutputString(str);
        }

        //////////////////////////////////////////////////////////////////////////

        public void ClearPane()
        {
            EnsurePaneCreated();

            _pane.Clear();
        }

        //////////////////////////////////////////////////////////////////////////

        private void EnsurePaneCreated()
        {
            if (_pane != null)
                return;

            foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Name == _paneName)
                {
                    _pane = pane;
                    return;
                }
            }

            _pane = _dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(_paneName);
        }

        //////////////////////////////////////////////////////////////////////////

        private readonly string _paneName;

        //////////////////////////////////////////////////////////////////////////

        private OutputWindowPane _pane;

        //////////////////////////////////////////////////////////////////////////

        private readonly DTE2 _dte2;

        //////////////////////////////////////////////////////////////////////////
    }
}
