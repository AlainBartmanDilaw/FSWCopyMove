using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSWCopyMove
{
    class FSWExtended : FileSystemWatcher
    {
        public delegate void DlgWriteMsg(string pFmt, params object[] args);
        public event DlgWriteMsg WriteMsg = null;
        public FSWExtended() : base()
        {
            _Write("Test {0}", DateTime.Now);
            base.Changed += new FileSystemEventHandler(_Changed);
            base.Created += new FileSystemEventHandler(_Created);
            base.Deleted += new FileSystemEventHandler(_Deleted);
            base.Renamed += new RenamedEventHandler(_Rename);
        }

        private void _Rename(object sender, RenamedEventArgs e)
        {
            _Write(e.OldFullPath + " is renamed to " + e.FullPath);
        }

        private void _Deleted(object sender, FileSystemEventArgs e)
        {
            _Write(e.FullPath.ToString() + " is deleted");
        }

        private void _Created(object sender, FileSystemEventArgs e)
        {
            _Write(e.FullPath.ToString() + " is created");
        }

        private void _Changed(object sender, FileSystemEventArgs e)
        {
            //lblMessage.Text = e.FullPath.ToString() + " is changed!";
            _Write(e.FullPath.ToString() + " is changed");
        }

        private void _Write(string pFmt, params object[] args)
        {
            if (WriteMsg != null)
            {
                WriteMsg(pFmt, args); //e.FullPath.ToString() + " is changed!");
            }
        }

    }
}
