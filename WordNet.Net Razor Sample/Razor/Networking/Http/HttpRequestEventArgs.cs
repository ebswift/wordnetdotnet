using System;

namespace Razor.Networking.Http
{
	#region HttpRequestEventArgs

	/// <summary>
	/// Defines an EventArgs class for the HttpRequest class
	/// </summary>
	[Serializable()]
	public class HttpRequestEventArgs : HttpMessageEventArgs 
	{
		protected HttpResponse _response;

		/// <summary>
		/// Initializes a new instance of the HttpRequestEventArgs class
		/// </summary>
		/// <param name="request">The message request context</param>
		public HttpRequestEventArgs(HttpRequest request) : base((HttpMessage)request)
		{
			
		}

		/// <summary>
		/// Returns the message request context
		/// </summary>
		public HttpRequest Request
		{
			get
			{
				return (HttpRequest)base.Context;
			}
		}

		/// <summary>
		/// Gets or sets the response that will be sent to the user-agent of this request.
		/// </summary>
		public HttpResponse Response
		{
			get
			{
				return _response;
			}
			set
			{
				_response = value;
			}
		}
	}

	public delegate void HttpRequestEventHandler(object sender, HttpRequestEventArgs e);

	#endregion

	#region HttpRequestCancelEventArgs

	/// <summary>
	/// Defines an EventArgs class for the HttpMessage class that is cancellable
	/// </summary>
	[Serializable()]
	public class HttpRequestCancelEventArgs : HttpMessageCancelEventArgs 
	{		
		protected HttpResponse _response;

		/// <summary>
		/// Initializes a new instance of the HttpMessageCancelEventArgs class
		/// </summary>
		/// <param name="message">The message context</param>
		/// <param name="cancel">A flag that indicates whether this event will be cancelled</param>
		public HttpRequestCancelEventArgs(HttpRequest request, bool cancel) : base((HttpMessage)request, cancel)
		{
			
		}

		/// <summary>
		/// Returns the message request context
		/// </summary>
		public HttpRequest Request
		{
			get
			{
				return (HttpRequest)base.Context;
			}
		}

		public override bool Cancel
		{
			get
			{
				return base.Cancel;
			}
			set
			{
				base.Cancel = value;

				// if this event is cancelled, there will be not response sent automatically
				if (_cancel)
					_response = null;
			}
		}

		/// <summary>
		/// Gets or sets the response that will be sent to the user-agent of this request.
		/// </summary>
		public HttpResponse Response
		{
			get
			{
				return _response;
			}
			set
			{
				_response = value;

				// you cannot assign the event an response, and then cancel it
				// the only way to cancel it is to cancel the event and respond manually
				if (_response != null)
					_cancel = false;
			}
		}
	}

	public delegate void HttpRequestCancelEventHandler(object sender, HttpRequestCancelEventArgs e);

	#endregion
}
