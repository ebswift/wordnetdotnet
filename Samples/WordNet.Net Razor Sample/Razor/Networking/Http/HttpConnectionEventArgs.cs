using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpConnectionEventArgs.
	/// </summary>
	public class HttpConnectionEventArgs : EventArgs
	{
		protected HttpConnection _connection;

		/// <summary>
		/// Initializes a new instance of the HttpConnectionEventArgs class
		/// </summary>
		/// <param name="connection">The connection responsible for the event</param>
		public HttpConnectionEventArgs(HttpConnection connection) : base()
		{
			_connection = connection;
		}

		/// <summary>
		/// Returns the connection responsible for the event
		/// </summary>
		public HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}
	}
	
	public delegate void HttpConnectionEventHandler(object sender, HttpConnectionEventArgs e);
}
