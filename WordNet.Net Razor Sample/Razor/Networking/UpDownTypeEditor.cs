using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for UpDownTypeEditor.
	/// </summary>
	public class UpDownTypeEditor : UITypeEditor	
	{
		public UpDownTypeEditor() : base()
		{
			
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					System.Windows.Forms.NumericUpDown editor = new NumericUpDown();
					editor.DecimalPlaces = 0;
					decimal v = System.Convert.ToDecimal(value);
					editor.Minimum = 0;
					editor.Maximum = Decimal.MaxValue;
					editor.Value = v;										
					edSvc.DropDownControl(editor);
					value = editor.Value;
					return value;
				}
			}	

			return base.EditValue (context, provider, value);
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{			
			base.PaintValue (e);
		}


	}
}
