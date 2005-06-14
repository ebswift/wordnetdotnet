using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpErrorSuccessException.
	/// </summary>
	public class HttpErrorSuccessException : Exception
	{
		public HttpErrorSuccessException() : base(@"The operation completed successfully.")
		{
			
		}
	}
}
