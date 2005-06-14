using System;
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for ProgressWindowThread.
	/// </summary>
	public class CancellableProgressWindowThread : WindowThread
	{
		public CancellableProgressWindowThread() : base()
		{
			
		}

		public new CancellableProgressWindow Window
		{
			get
			{
				return this._window as CancellableProgressWindow;
			}
		}

		protected override void GetWindowType(out Form window)
		{
			window = new CancellableProgressWindow();
		}

	}
}
