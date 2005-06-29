using System;
using System.Diagnostics;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Razor.Networking;
using Razor.Networking.Http;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookItemConnectionManager.
	/// </summary>
	public class AddressBookItemConnectionManager : IDisposable
	{
		protected bool _disposed;
		protected AddressBookItem _item;
		protected IAddressBookItemContext _itemContext;
		protected HttpConnection _connection;      
		protected Hashtable _propertyBag;
		protected AddressBookItemBackgroundThreadContext _threadContext;

		#region My Public Events

		/// <summary>
		/// Fires when the connection manager attempts to manipulate the address book item and connection. Allowing clients to realize the context of the call and supply a user defined state object as the context for this connection manager.
		/// </summary>
		public event AddressBookItemConnectionManagerContextEventHandler ConnectionEstablishContext;

		/// <summary>
		/// Fires before the manager attempts to connect the connection to the remote host defined by the address book item
		/// </summary>
		public event AddressBookItemConnectionManagerCancelEventHandler BeforeConnectionOpened;

		/// <summary>
		/// Fires as the connection manager resolves an address
		/// </summary>
		public event AddressBookItemConnectionManagerResolvingAddressEventHandler ConnectionResolvingAddress;

		/// <summary>
		/// Fires after the manager has connected the connection to the remote host defined by the address book item
		/// </summary>
		public event AddressBookItemConnectionManagerEventHandler ConnectionOpened;  

		/// <summary>
		/// Fires before the manager attempts to disconnect the connection from the remote host
		/// </summary>
		public event AddressBookItemConnectionManagerCancelEventHandler BeforeConnectionClosed;

		/// <summary>
		/// Fires after the manager has disconnected the connection from the remote host
		/// </summary>
		public event AddressBookItemConnectionManagerEventHandler ConnectionClosed;  

		/// <summary>
		/// Fires when an exception is encountered during the course of connecting and disconnecting the connection to and from the remote host
		/// </summary>
		public event AddressBookItemConnectionManagerExceptionEventHandler ConnectionException;

		#endregion

		/// <summary>
		/// Initializes a new instance of the AddressBookItemConnectionManager class
		/// </summary>
		/// <param name="item">The address book item that defines the remote host to connect to</param>
		/// <param name="connection">The Tcp connection that will be used throughout the lifetime of the connection</param>
		public AddressBookItemConnectionManager(AddressBookItem item, HttpConnection connection)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (connection == null)
				throw new ArgumentNullException("connection");

			_propertyBag = new Hashtable();
			_item = item;
			_connection = connection;
			_connection.Opened += new HttpConnectionEventHandler(OnInternalConnectionOpened);
			_connection.Closed += new HttpConnectionEventHandler(OnInternalConnectionClosed);
			_connection.Exception += new ExceptionEventHandler(OnInternalConnectionException);
		}      
      
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					this.Disconnect();
				}
				_disposed = true;
			}
		}

		#endregion      
    
		#region My Public Methods

		/// <summary>
		/// Allow the client to realize the context for this address book item (aka, manifest the item visually and store some state information in our context for later reference)
		/// </summary>
		private void InternalEstablishContext()
		{	
			/*
			 * Raise the realize context event, and save the context supplied to us by the client
			 * */
			AddressBookItemConnectionManagerContextEventArgs e = new AddressBookItemConnectionManagerContextEventArgs(this);			
			this.OnConnectionEstablishContext(this, e);
			
			// save the supplied context
			_itemContext = e.Context;
			
			// if the context is supplied, then set the context's item property reference to the address book item we are managing
			if (_itemContext != null)
				_itemContext.AddressBookItem = _item;
		}

		/// <summary>
		/// Connects a connection to the address and port specified by the address book item
		/// </summary>
		/// <param name="autoRecv">A flag that indicates whether the connection should begin receiving data as soon as it is connected. Some sessions may not need to receive immediately</param>
		/// <returns></returns>
		public bool Connect(bool autoRecv, object stateObject)
		{
			try
			{				
				this.InternalEstablishContext();

				/*
				 * raise the before connect event 
				 * */
				AddressBookItemConnectionManagerCancelEventArgs e = new AddressBookItemConnectionManagerCancelEventArgs(this, false);
				this.OnBeforeConnectionOpened(this, e);

				// bail on the connection if we have been told to cancel
				if (e.Cancel)
					return false;

				// resolve the address book item to a remote host
				IPEndPoint ep = HttpUtils.Resolve(_item.Address, _item.Port, this, new AddressResolutionEventHandler(this.OnInternalConnectionResolvingAddress), stateObject);						

				// connect the connection to the remote host
				_connection.Open(ep, autoRecv);

				return true;
			}
			catch(Exception ex)
			{
				this.OnConnectionException(this, new AddressBookItemConnectionManagerExceptionEventArgs(this, ex));
			}
			return false;
		}

		/// <summary>
		/// Disconnects the previously connected connection
		/// </summary>
		/// <returns></returns>
		public bool Disconnect()
		{
			try
			{
				// the connection may have already been disconnected
				if (!_connection.IsAlive)
					return false;

				/*
				 * raise the before disconnect event 
				 * */
				AddressBookItemConnectionManagerCancelEventArgs e = new AddressBookItemConnectionManagerCancelEventArgs(this, false);
				this.OnBeforeConnectionClosed(this, e);

				// bail on the connection if we have been told to cancel
				if (e.Cancel)
					return false;

				// disconnect the connection from the remote host
				_connection.Close();

				return true;
			}
			catch(Exception ex)
			{
				this.OnConnectionException(this, new AddressBookItemConnectionManagerExceptionEventArgs(this, ex));
			}
			return false;
		}

		/// <summary>
		/// Determines whether the property bag contains the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsProperty(object key)
		{
			return _propertyBag.ContainsKey(key);
		}

		/// <summary>
		/// Removes the property using the specified key from the property bab
		/// </summary>
		/// <param name="key"></param>
		public void RemoveProperty(object key)
		{
			if (_propertyBag.ContainsKey(key))
				_propertyBag.Remove(key);
		}

		/// <summary>
		/// Reads a value from the PropertyBag. Returns null if the key does not exist or is null.
		/// </summary>
		/// <param name="key">The key by which the value was stored</param>
		/// <returns></returns>
		public object ReadProperty(object key)
		{
			if (key != null)
				if (_propertyBag.ContainsKey(key))
					return _propertyBag[key];
			return null;
		}

		/// <summary>
		/// Writes a value to the PropertyBag using the specified key, or adds it using the key if the key does not exist. Does not do anything if the key is null.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void WriteProperty(object key, object value)
		{
			if (key != null)
			{
				if (_propertyBag.ContainsKey(key))
				{
					if (value != null)
						_propertyBag[key] = value;
					else
						_propertyBag.Remove(key);
				}
				else
				{
					_propertyBag.Add(key, value);				
				}
			}			
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns the address book item which defines the remote host to which the connection will be connected
		/// </summary>
		public AddressBookItem AddressBookItem
		{
			get
			{
				return _item;
			}
		}

		/// <summary>
		/// Returns the context
		/// </summary>
		public IAddressBookItemContext AddressBookItemContext
		{
			get
			{
				return _itemContext;
			}
		}

		/// <summary>
		/// Returns the thread context upon which this address book item connection manager may or may not be operating
		/// </summary>
		public AddressBookItemBackgroundThreadContext ThreadContext
		{
			get
			{
				return _threadContext;
			}
			set
			{
				_threadContext = value;
			}
		}
		
		/// <summary>
		/// Returns the Tcp connection that is being used to maintain the connection with the remote host
		/// </summary>
		public HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Returns the Hashtable that serves as the property bag
		/// </summary>
		public Hashtable PropertyBag
		{
			get
			{
				return _propertyBag;
			}
		}

		#endregion

		#region My Protected Methods

		/// <summary>
		/// Raises the RealizeContext event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnConnectionEstablishContext(object sender, AddressBookItemConnectionManagerContextEventArgs e)
		{
			try
			{
				if (this.ConnectionEstablishContext != null)
					this.ConnectionEstablishContext(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the BeforeConnected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeConnectionOpened(object sender, AddressBookItemConnectionManagerCancelEventArgs e)
		{
			try
			{
				if (this.BeforeConnectionOpened != null)
					this.BeforeConnectionOpened(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the ResolvingAddress event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnConnectionResolvingAddress(object sender, AddressBookItemConnectionManagerResolvingAddressEventArgs e)
		{
			try
			{
				if (this.ConnectionResolvingAddress != null)
					this.ConnectionResolvingAddress(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the Connected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnConnectionOpened(object sender, AddressBookItemConnectionManagerEventArgs e)
		{
			try
			{
				if (this.ConnectionOpened != null)
					this.ConnectionOpened(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the BeforeDisconnected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeConnectionClosed(object sender, AddressBookItemConnectionManagerCancelEventArgs e)
		{
			try
			{
				if (this.BeforeConnectionClosed != null)
					this.BeforeConnectionClosed(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the Disconnected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnConnectionClosed(object sender, AddressBookItemConnectionManagerEventArgs e)
		{
			try
			{
				if (this.ConnectionClosed != null)
					this.ConnectionClosed(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the Exception event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnConnectionException(object sender, AddressBookItemConnectionManagerExceptionEventArgs e)
		{
			try
			{
				if (this.ConnectionException != null)
					this.ConnectionException(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion

		#region My Private Methods

		/// <summary>
		/// Intercepts the connection event from the connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInternalConnectionOpened(object sender, HttpConnectionEventArgs e)
		{
			// forward this event on through the connection manager
			this.OnConnectionOpened(sender, new AddressBookItemConnectionManagerEventArgs(this));			
		}

		/// <summary>
		/// Intercepts the resolving address event from the connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInternalConnectionResolvingAddress(object sender, AddressResolutionEventArgs e)
		{
			// forward thsi event on through the connection manager
			this.OnConnectionResolvingAddress(sender, new AddressBookItemConnectionManagerResolvingAddressEventArgs(this, e.Address, e.Port, e.StateObject));
		}

		/// <summary>
		/// Intercepts the diconnection event from the connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInternalConnectionClosed(object sender, HttpConnectionEventArgs e)
		{
			// forward this event on through the connection manager
			this.OnConnectionClosed(sender, new AddressBookItemConnectionManagerEventArgs(this));
		}

		/// <summary>
		/// Intercepts the exception event from the connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInternalConnectionException(object sender, ExceptionEventArgs e)
		{
			// forward this event on through the connection manager
			this.OnConnectionException(sender, new AddressBookItemConnectionManagerExceptionEventArgs(this, e.Exception));
		}

		#endregion
	}

	#region AddressBookItemConnectionManagerEventArgs

	/// <summary>
	/// Provides an EventArgs class which encapsulates data for events from an AddressBookItemConnectionManager
	/// </summary>
	public class AddressBookItemConnectionManagerEventArgs : EventArgs
	{
		protected AddressBookItemConnectionManager _manager;

		/// <summary>
		/// Initializes a new instance of the AddressBookItemConnectionManagerEventArgs class
		/// </summary>
		/// <param name="manager">The manager instance that is causing the event</param>
		public AddressBookItemConnectionManagerEventArgs(AddressBookItemConnectionManager manager) : base()
		{
			_manager = manager;
		}

		/// <summary>
		/// Returns the AddressBookItemConnectionManager instance about which this event is based
		/// </summary>
		public AddressBookItemConnectionManager Manager
		{
			get
			{
				return _manager;
			}
		}
	}

	public delegate void AddressBookItemConnectionManagerEventHandler(object sender, AddressBookItemConnectionManagerEventArgs e);

	#endregion

	#region AddressBookItemConnectionManagerResolvingAddressEventArgs

	public class AddressBookItemConnectionManagerResolvingAddressEventArgs : AddressBookItemConnectionManagerEventArgs
	{
		protected string _address;
		protected int _port;
		protected object _stateObject;

		public AddressBookItemConnectionManagerResolvingAddressEventArgs(AddressBookItemConnectionManager manager, string address, int port, object stateObject) : base(manager)
		{
			_address = address;
			_port = port;
			_stateObject = stateObject;
		}

		/// <summary>
		/// The address being resolved (one of the following, IPv4 dotted quad, IPv6 colon separated hex, or Dns name)
		/// </summary>
		public string Address
		{
			get
			{
				return _address;
			}
		}
		
		/// <summary>
		/// The port number used to during resolution
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
		}

		/// <summary>
		/// Returns a user defined state object that can be used to determine the context of the event
		/// </summary>
		public object StateObject
		{
			get
			{
				return _stateObject;
			}
		}
	}

	public delegate void AddressBookItemConnectionManagerResolvingAddressEventHandler(object sender, AddressBookItemConnectionManagerResolvingAddressEventArgs e);

	#endregion

	#region AddressBookItemConnectionManagerCancelEventArgs

	/// <summary>
	/// Provides an EventArgs class that can cancel events coming from an AddressBookItemConnectionManager
	/// </summary>
	public class AddressBookItemConnectionManagerCancelEventArgs : AddressBookItemConnectionManagerEventArgs
	{
		protected bool _cancel;

		/// <summary>
		/// Initializes a new instance of the X class
		/// </summary>
		/// <param name="manager">The manager instance raising the event</param>
		/// <param name="cancel">A flag that controls whether the event is cancelled or not</param>
		public AddressBookItemConnectionManagerCancelEventArgs(AddressBookItemConnectionManager manager, bool cancel) : base(manager)
		{
			_cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a flag that determines whether the event should be cancelled
		/// </summary>
		public bool Cancel
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

	public delegate void AddressBookItemConnectionManagerCancelEventHandler(object sender, AddressBookItemConnectionManagerCancelEventArgs e);

	#endregion

	#region AddressBookItemConnectionManagerExceptionEventArgs

	/// <summary>
	/// Defines an EventArgs class for exceptions raised by an AddressBookItemConnectionManager
	/// </summary>
	public class AddressBookItemConnectionManagerExceptionEventArgs : AddressBookItemConnectionManagerEventArgs
	{
		protected Exception _ex;

		/// <summary>
		/// Initializes a new instance of the AddressBookItemConnectionManagerExceptionEventArgs class
		/// </summary>
		/// <param name="manager">The manager encountering the exception</param>
		/// <param name="ex">The exception encountered</param>
		public AddressBookItemConnectionManagerExceptionEventArgs(AddressBookItemConnectionManager manager, Exception ex) : base(manager)
		{
			_ex = ex;
		}

		/// <summary>
		/// Returns the exception encountered by the manager
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _ex;
			}
		}
	}

	public delegate void AddressBookItemConnectionManagerExceptionEventHandler(object sender, AddressBookItemConnectionManagerExceptionEventArgs e);

	#endregion

	#region AddressBookItemConnectionManagerContextEventArgs

	/// <summary>
	/// Defines an EventArgs class that can assertain the context for this connection manager
	/// </summary>
	public class AddressBookItemConnectionManagerContextEventArgs : AddressBookItemConnectionManagerEventArgs
	{
		protected IAddressBookItemContext _context;

		/// <summary>
		/// Initializes a new instance of the AddressBookItemConnectionManagerContextEventArgs class
		/// </summary>
		/// <param name="manager"></param>
		public AddressBookItemConnectionManagerContextEventArgs(AddressBookItemConnectionManager manager) : base(manager)
		{

		}

		/// <summary>
		/// Gets or sets the address book item context for this connection manager
		/// </summary>
		public IAddressBookItemContext Context
		{
			get
			{
				return _context;
			}
			set
			{
				_context = value;
			}
		}
	}

	public delegate void AddressBookItemConnectionManagerContextEventHandler(object sender, AddressBookItemConnectionManagerContextEventArgs e);

	#endregion

	#region IAddressBookItemContext

	/// <summary>
	/// Provides a simple interface to associate an address book item with a context object
	/// </summary>
	public interface IAddressBookItemContext
	{
		/// <summary>
		/// Gets or sets the address book item that defines the context for this item
		/// </summary>
		AddressBookItem AddressBookItem {get; set;}
	}

	#endregion
}
