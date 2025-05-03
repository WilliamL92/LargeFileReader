using BLL.Models;

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
