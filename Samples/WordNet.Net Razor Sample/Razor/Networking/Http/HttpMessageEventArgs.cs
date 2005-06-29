using System;

namespace Razor.Networking.Http
{
	#region HttpMessageEventArgs

	/// <summary>
	/// Defines an EventArgs class for the HttpMessage class
	/// </summary>
	[Serializable()]
	public class HttpMessageEventArgs : EventArgs
	{
		protected HttpMessage _context;

		/// <summary>
		/// Initializes a new instance of the HttpMessageEventArgs class
		/// </summary>
		/// <param name="message">The message context</param>
		public HttpMessageEventArgs(HttpMessage message) : base()
		{
			_context = message;
		}

		/// <summary>
		/// Returns the message context for this event
		/// </summary>
		protected HttpMessage Context
		{
			get
			{
				return _context;
			}
		}
	}

	public delegate void HttpMessageEventHandler(object sender, HttpMessageEventArgs e);

	#endregion

	#region HttpMessageCancelEventArgs

	/// <summary>
	/// Defines an EventArgs class for the HttpMessage class that is cancellable
	/// </summary>
	[Serializable()]
	public class HttpMessageCancelEventArgs : HttpMessageEventArgs 
	{
		protected bool _cancel;

		/// <summary>
		/// Initializes a new instance of the HttpMessageCancelEventArgs class
		/// </summary>
		/// <param name="message">The message context</param>
		/// <param name="cancel">A flag that indicates whether this event will be cancelled</param>
		public HttpMessageCancelEventArgs(HttpMessage message, bool cancel) : base(message)
		{
			_cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether this event will be cancelled
		/// </summary>
		public virtual bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}
	}

	public delegate void HttpMessageCancelEventHandler(object sender, HttpMessageCancelEventArgs e);

	#endregion
}
