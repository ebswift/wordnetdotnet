using System;
using System.Drawing;
using System.Windows.Forms;

namespace Razor.SnapIns.WindowPositioningEngine
{
	/// <summary>
	/// Summary description for IWindowPositioningEngineFeaturable.
	/// </summary>
	public interface IWindowPositioningEngineFeaturable
	{
		Size GetDefaultSize();
		Point GetDefaultLocation();
		FormWindowState GetDefaultWindowState();
	}
}
