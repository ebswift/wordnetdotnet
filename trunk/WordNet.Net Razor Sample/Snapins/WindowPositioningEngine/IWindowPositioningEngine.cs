using System;
using System.Windows.Forms;

namespace Razor.SnapIns.WindowPositioningEngine
{
	/// <summary>
	/// Summary description for IWindowPositioningEngine.
	/// </summary>
	public interface IWindowPositioningEngine
	{
		bool Manage(Form f, string key);
		bool Manage(Form f, string key, bool restore);
	}
}
