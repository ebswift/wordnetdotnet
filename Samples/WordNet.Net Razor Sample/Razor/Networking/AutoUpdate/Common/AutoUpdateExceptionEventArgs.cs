using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateExceptionEventArgs.
	/// </summary>
	public class AutoUpdateExceptionEventArgs : EventArgs
	{
		protected Exception _ex;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateExceptionEventArgs class
		/// </summary>
		/// <param name="ex"></param>
		public AutoUpdateExceptionEventArgs(Exception ex) : base()
		{
			_ex = ex;
		}

		/// <summary>
		/// Returns the exception that was encountered
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _ex;
			}
		}
	}

	public delegate void AutoUpdateExceptionEventHandler(object sender, AutoUpdateExceptionEventArgs e);
}
