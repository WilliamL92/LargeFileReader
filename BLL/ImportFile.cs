using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ImportFile
    {
        public OpenFileDialog? OpenFileBox()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return null;
            return dlg;
        }
    }
}
