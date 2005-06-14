using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Windows.Forms;
using Razor.Networking.Http.Hosting;

namespace Razor.Networking.Http
{	
	/// <summary>
	/// Provides a mechanism to register for HttpRequest Method notification and processing accross multiple HttpConnections.
	/// </summary>
	public class HttpRequestDispatcher : IDisposable
	{
		protected bool _disposed;
		protected Hashtable _requestHandlerListLookupTable;
		protected AspHost _aspHost;

		/// <summary>
		/// Defines the index/keys of the lists of handlers that store request handler callbacks for this dispatcher
		/// </summary>
		internal enum HttpRequestHookPoints
		{
			BeforeHttpRuntimeProcessing,
			AfterHttpRuntimeProcessing
		}

		/// <summary>
		/// Initializes a new instance of the HttpRequestDispatcher class
		/// </summary>
		public HttpRequestDispatcher(bool aspNetCapable)
		{
			_requestHandlerListLookupTable = new Hashtable();
			_requestHandlerListLookupTable.Add(HttpRequestHookPoints.BeforeHttpRuntimeProcessing, new Hashtable()); // all of the cancellable handlers
			_requestHandlerListLookupTable.Add(HttpRequestHookPoints.AfterHttpRuntimeProcessing, new Hashtable()); // all of the non-cancellable handlers

			// WARNING: 9x/NT Incompatiblity			
			if (aspNetCapable)
				this.CreateAspHost();	
		}

		/// <summary>
		/// Creates an internal AspHost instance to allow the dispatcher to send requests off to the HttpRuntime
		/// </summary>
		internal void CreateAspHost()
		{
			try
			{
				string physicalDirectory = Application.StartupPath; // Path.Combine(Application.StartupPath, @"wwwroot");
				string configurationFile = Path.Combine(physicalDirectory, @"web.config");

				_aspHost = AspRuntime.CreateAspHost(@"/", physicalDirectory, string.Empty, configurationFile);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
		
		/// <summary>
		/// Looks up the hashtable of handlers for the specified hook point
		/// </summary>
		internal Hashtable this[HttpRequestHookPoints hookPoint]
		{
			get
			{
				return (Hashtable)_requestHandlerListLookupTable[hookPoint];
			}
		}
		
		/// <summary>
		/// Registers for notification for when the specified method is received in a HttpRequest
		/// </summary>
		/// <param name="method">The method notification will be registered for</param>
		/// <param name="onRequest">The callback to be notified when/if the method is received</param>
		public void RegisterForRequestMethodNotification(string method, HttpRequestCancelEventHandler onRequest)
		{
			// we must have a valid method
			HttpUtils.ValidateToken(@"method", method);	
			
			// and we must have a valid callback
			if (onRequest == null)
				throw new ArgumentNullException("onRequest");
			
			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
			Hashtable requestHandlers = this[HttpRequestHookPoints.BeforeHttpRuntimeProcessing];

			// receive the list of handlers that is handling callback notification for methods in that list
			HttpRequestCancelEventHandler handlers = requestHandlers[method] as HttpRequestCancelEventHandler;

			// if the handler list is null
			if (handlers == null)
				// add the handlers into the list by the method we're registering for
				requestHandlers.Add(method, handlers);
				
			// now combine the callback with all of the existing handlers 
			handlers = (HttpRequestCancelEventHandler)Delegate.Combine(handlers, onRequest);

			// and finally store that list of callbacks using the method as the key in the list from which we retrieved this initially
			requestHandlers[method] = handlers;
		}
		
//		/// <summary>
//		/// Registers for notification for when the specified method is received in a HttpRequest
//		/// </summary>
//		/// <param name="method">The method notification will be registered for</param>
//		/// <param name="onRequest">The callback to be notified when/if the method is received</param>
//		public void RegisterForRequestMethodNotification(string method, HttpRequestEventHandler onRequest)
//		{
//			// we must have a valid method
//			HttpUtils.ValidateToken(@"method", method);	
//			
//			// and we must have a valid callback
//			if (onRequest == null)
//				throw new ArgumentNullException("onRequest");
//			
//			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
//			Hashtable requestHandlers = this[HttpRequestHookPoints.AfterHttpRuntimeProcessing];
//
//			// receive the list of handlers that is handling callback notification for methods in that list
//			HttpRequestEventHandler handlers = requestHandlers[method] as HttpRequestEventHandler;
//
//			// if the handler list is null
//			if (handlers == null)
//				// add the handlers into the list by the method we're registering for
//				requestHandlers.Add(method, handlers);
//				
//			// now combine the callback with all of the existing handlers 
//			handlers = (HttpRequestEventHandler)Delegate.Combine(handlers, onRequest);
//
//			// and finally store that list of callbacks using the method as the key in the list from which we retrieved this initially
//			requestHandlers[method] = handlers;
//		}
		
		/// <summary>
		/// Unregisters for notification for when the specified method is received in a HttpRequest
		/// </summary>
		/// <param name="method">The method notification was registered for</param>
		/// <param name="onRequest">The callback to be notified when/if the method is received</param>
		public void UnregisterForRequestMethodNotification(string method, HttpRequestCancelEventHandler onRequest)
		{
			// we must have a valid method
			HttpUtils.ValidateToken(@"method", method);	
			
			// and we must have a valid callback
			if (onRequest == null)
				throw new ArgumentNullException("onRequest");

			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
			Hashtable requestHandlers = this[HttpRequestHookPoints.BeforeHttpRuntimeProcessing];

			// receive the list of handlers that is handling callback notification for methods in that list
			HttpRequestCancelEventHandler handlers = requestHandlers[method] as HttpRequestCancelEventHandler;

			// if the handler list is null
			if (handlers != null)
				// add the handlers into the list by the method we're registering for
				handlers = (HttpRequestCancelEventHandler)Delegate.Remove(handlers, onRequest);

			// if there are still some handlers, and the list is zero length, remove the list entirely
			if (handlers != null)
				if (handlers.GetInvocationList().Length == 0)
				{
					requestHandlers.Remove(method);
					return;
				}
			
			// save the list of handlers for the next guy
			requestHandlers[method] = handlers;
		}

//		/// <summary>
//		/// Unregisters for notification for when the specified method is received in a HttpRequest
//		/// </summary>
//		/// <param name="method">The method notification was registered for</param>
//		/// <param name="onRequest">The callback to be notified when/if the method is received</param>
//		public void UnregisterForRequestMethodNotification(string method, HttpRequestEventHandler onRequest)
//		{
//			// we must have a valid method
//			HttpUtils.ValidateToken(@"method", method);	
//			
//			// and we must have a valid callback
//			if (onRequest == null)
//				throw new ArgumentNullException("onRequest");
//
//			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
//			Hashtable requestHandlers = this[HttpRequestHookPoints.AfterHttpRuntimeProcessing];
//
//			// receive the list of handlers that is handling callback notification for methods in that list
//			HttpRequestEventHandler handlers = requestHandlers[method] as HttpRequestEventHandler;
//
//			// if the handler list is null
//			if (handlers != null)
//				// add the handlers into the list by the method we're registering for
//				handlers = (HttpRequestEventHandler)Delegate.Remove(handlers, onRequest);
//
//			// if there are still some handlers, and the list is zero length, remove the list entirely
//			if (handlers != null)
//				if (handlers.GetInvocationList().Length == 0)
//				{
//					requestHandlers.Remove(method);
//					return;
//				}
//			
//			// save the list of handlers for the next guy
//			requestHandlers[method] = handlers;
//		}

		/// <summary>
		/// Dispatches the HttpRequest to the registered handlers for processing. If no response is created or sent, then a default response will be prepared.
		/// </summary>
		/// <param name="dispatcher"></param>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void DispatchRequest(object sender, ref HttpRequestCancelEventArgs e)
		{
//			Debug.WriteLine(string.Format("Dispatching request '{0}'.", e.Request.Method));

			// internally dispatch the request to the registered handler(s) until someone responds to the request
			Delegate[] delegates = this.InternalDispatchRequest(sender, ref e);
						
			if (!e.Cancel && e.Response == null)
			{			
				#region Asp Processing

				// if we have an asp host
				if (_aspHost != null)
				{
					bool alreadyRetried = false;
				
				RetryAspProcessing:
				
					try
					{
						// use the asp host to process the request
						_aspHost.ProcessRequest((HttpConnection)sender, ref e);
					}
					catch(AppDomainUnloadedException)
					{						
//						Debug.WriteLine(ex);

						// it might have unloaded on us, so we can try once more
						if (!alreadyRetried)
						{
							// recreate the host
							this.CreateAspHost();
							alreadyRetried = true;

							// jump back and try again
							goto RetryAspProcessing;
						}
					}
				}

				#endregion

				// after the asp processing, it might have already sent the response so watch out that it didn't cancel 
				// the remaining processing we had to do
				if (e.Cancel)
					return;

				// and finally if we still have no response for the request, let's fill in which ever is appropriate
				if (e.Response == null)
				{
					// if there are no handlers that were registered for the request's method
					if (delegates.Length == 0)
					{
						// then the method is not allowed
						e.Response = new HttpResponse(new MethodNotAllowedStatus());
						e.Response.SetBodyFromString(string.Format("405 - The '{0}' method is not allowed.\nThe request for the '{1}' resource cannot be processed.", e.Request.Method, e.Request.RequestUri), HttpUtils.Encoding, MIME.Text.Plain);
					}
					else
					{
						// resource not found (there was a handler but it didn't respond) so obviously the resource couldn't be found
						e.Response = new HttpResponse(new NotFoundStatus());
						e.Response.SetBodyFromString(string.Format("404 - The resource '{0}' could not be found.", e.Request.RequestUri), HttpUtils.Encoding, MIME.Text.Plain);
					}
				}
			}			
		}

		/// <summary>
		/// Dispatches the request to each handler registered to receive notification of this request's method
		/// </summary>
		/// <param name="dispatcher"></param>
		/// <param name="request"></param>
		/// <param name="sender"></param>
		/// <param name="cancel"></param>
		private Delegate[] InternalDispatchRequest(object sender, ref HttpRequestCancelEventArgs e)
		{
			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
			Hashtable requestHandlers = this[HttpRequestHookPoints.BeforeHttpRuntimeProcessing];

			// receive the list of handlers that is handling callback notification for methods in that list
			HttpRequestCancelEventHandler handlers = requestHandlers[e.Request.Method] as HttpRequestCancelEventHandler;

			// there is a list of handlers waiting to be notified
			if (handlers == null)				
				return new Delegate[] {};
						
			// extract the list of handlers to be notified when this method is received
			Delegate[] delegates = handlers.GetInvocationList();
			if (delegates == null)
				return new Delegate[] {};
									
			// enumerate each handler
			foreach(Delegate d in delegates)
			{
				try
				{
					// notify the handler
					((HttpRequestCancelEventHandler)d)(sender, e);

					// someone may have created a response that is ready to be sent for this event so no further processing is required
					if (e.Response != null)
						return delegates;					

					// someone may have handled the request manually and there for this event is cancelled and no further processing is required
					if (e.Cancel)					
						return delegates;										
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}

			return delegates;
		}

//		/// <summary>
//		/// Dispatches the request to each handler registered to receive notification of this request's method
//		/// </summary>
//		/// <param name="dispatcher"></param>
//		/// <param name="request"></param>
//		/// <param name="sender"></param>
//		private static void DispatchRequestAfterHttpRuntimeProcessing(
//			HttpRequestDispatcher dispatcher, 
//			HttpRequest request,
//			object sender)
//		{
//			// look up the list of handlers for the specified hook point, and then look up the method in that list to 
//			Hashtable requestHandlers = dispatcher[HttpRequestHookPoints.AfterHttpRuntimeProcessing];
//
//			// receive the list of handlers that is handling callback notification for methods in that list
//			HttpRequestCancelEventHandler handlers = requestHandlers[request.Method] as HttpRequestCancelEventHandler;
//
//			// there is a list of handlers waiting to be notified
//			if (handlers != null)
//			{
//				// extract the list of handlers to be notified when this method is received
//				Delegate[] delegates = handlers.GetInvocationList();
//				if (delegates != null)
//				{
//					// construct a new cancellable event args param
//					HttpRequestEventArgs e = new HttpRequestEventArgs(request);
//
//					// enumerate each handler
//					foreach(Delegate d in delegates)
//					{
//						try
//						{
//							// notify the handler
//							((HttpRequestEventHandler)d)(sender, e);
//						}
//						catch(Exception ex)
//						{
//							Debug.WriteLine(ex);
//						}
//					}
//				}
//			}
//		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				if (disposing)
				{
					if (_aspHost != null)
					{
						_aspHost.Dispose();
						_aspHost = null;
					}
				}
				_disposed = true;
			}
		}

		#endregion
	}
}
