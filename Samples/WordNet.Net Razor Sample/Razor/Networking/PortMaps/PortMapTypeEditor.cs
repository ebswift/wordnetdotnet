using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Summary description for PortMapTypeEditor.
	/// </summary>
	public class PortMapTypeEditor : UITypeEditor	
	{
		public PortMapTypeEditor() : base()
		{
			
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					PortMapViewer viewer = new PortMapViewer();
					viewer.PortMaps = (PortMapCollection)value;					
					edSvc.DropDownControl(viewer);					
				}
			}			
			return base.EditValue (context, provider, value);
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			e.Graphics.FillRectangle(SystemBrushes.ControlDarkDark, e.Bounds);			
		}
	}
}
