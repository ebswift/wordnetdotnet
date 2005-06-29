using System;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for ExceptionEventArgs.
	/// </summary>
	public class ExceptionEventArgs : EventArgs
	{
		protected Exception _ex;

		public ExceptionEventArgs(Exception ex) : base()
		{
			_ex = ex;
		}

		public Exception Exception
		{
			get
			{				
				return _ex;
			}
		}
	}
						 
	public delegate void ExceptionCallback(object sender, ExceptionEventArgs e);
	public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
}
