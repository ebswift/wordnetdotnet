using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for PathOptionEditor.
	/// </summary>
	public class PathOptionEditor : UITypeEditor
	{
		public PathOptionEditor()
		{
			
		}

        private string EditPath(string path)
        {            
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = path;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return path;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return this.EditPath(value.ToString());
        }
 

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

	}
}
