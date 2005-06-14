using System;

namespace Razor.Networking.Http
{
	#region HttpResponseEventArgs

	/// <summary>
	/// Defines an EventArgs class for the HttpResponse class
	/// </summary>
	[Serializable()]
	public class HttpResponseEventArgs : HttpMessageEventArgs 
	{
		/// <summary>
		/// Initializes a new instance of the HttpResponseEventArgs class
		/// </summary>
		/// <param name="response">The message response context</param>
		public HttpResponseEventArgs(HttpResponse response) : base((HttpMessage)response)
		{
			
		}

		/// <summary>
		/// Returns the message response context
		/// </summary>
		public HttpResponse Response
		{
			get
			{
				return (HttpResponse)base.Context;
			}
		}
	}

	public delegate void HttpResponseEventHandler(object sender, HttpResponseEventArgs e);

	#endregion
	
//	#region HttpResponseCancelEventArgs
//
//	/// <summary>
//	/// Defines an EventArgs class for the HttpMessage class that is cancellable
//	/// </summary>
//	public class HttpResponseCancelEventArgs : HttpMessageCancelEventArgs 
//	{
//		/// <summary>
//		/// Initializes a new instance of the HttpMessageCancelEventArgs class
//		/// </summary>
//		/// <param name="message">The message context</param>
//		/// <param name="cancel">A flag that indicates whether this event will be cancelled</param>
//		public HttpResponseCancelEventArgs(HttpResponse response, bool cancel) : base((HttpMessage)response, cancel)
//		{
//			
//		}
//
//		/// <summary>
//		/// Returns the message response context
//		/// </summary>
//		public new HttpResponse Context
//		{
//			get
//			{
//				return (HttpResponse)base.Context;
//			}
//		}
//	}
//
//	public delegate void HttpResponseCancelEventHandler(object sender, HttpResponseCancelEventArgs e);
//
//	#endregion
}
