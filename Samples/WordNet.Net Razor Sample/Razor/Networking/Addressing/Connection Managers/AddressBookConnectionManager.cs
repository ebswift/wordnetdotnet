using System;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using Razor.MultiThreading;
using Razor.Networking.Http;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookConnectionManager.
	/// </summary>
	public class AddressBookConnectionManager : IDisposable
	{
		protected bool _disposed;
		protected bool _enabled;
		protected BackgroundThreadPool _threadPool;
		protected AddressBookItemConnectionManagerList _sessionManagers;
		protected AddressBook _addressBook;
		protected Hashtable _contextLookupTable;

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
		/// Initializes a new instance of the AddressBookConnectionManager class
		/// </summary>
		public AddressBookConnectionManager()
		{
			/*
			 * create the thread pool that will help us during connection and disconnection attempts
			 * */
			_threadPool = new BackgroundThreadPool();

			/*
			 * create a new list for storing connection managers
			 * */
			_sessionManagers = new AddressBookItemConnectionManagerList();

			/*
			 * create a new context lookup table to allow pre-binding of contexts to items
			 * */
			_contextLookupTable = new Hashtable();
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookConnectionManager class
		/// </summary>
		/// <param name="addressBook"></param>
		public AddressBookConnectionManager(AddressBook addressBook) : this()
		{								
			// we require a valid address book
			if (addressBook == null)
				throw new ArgumentNullException("addressBook");

			/*
			 * wire up to the events of the address book and monitor it for changes
			 * */
			_addressBook = addressBook;

			// determines whether the manager intercepts the events from the address book to control the connection managers
			this.InterceptAddressBookEvents = false;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					this.DisconnectAll();
					_threadPool = null;
					_sessionManagers = null;
					_contextLookupTable = null;
				}
				_disposed = true;
			}
		}

		#endregion

		#region My Public Methods

		/// <summary>
		/// Creates a queued thread pool job that will connect a connection for every address book item in the address book
		/// </summary>
		public virtual void ConnectAll(bool verboseSessions, bool autoRecv)
		{
			foreach(AddressBookItem item in _addressBook.Items)
			{
				this.Connect(item, false /* don't try and disconnect first */, verboseSessions, autoRecv);
			}			
		}	

		/// <summary>
		/// Creates a gueued thread pool job that will disconnect a connection for every address book item in the address book
		/// </summary>
		public virtual void DisconnectAll()
		{
			foreach(AddressBookItem item in _addressBook.Items)
			{
				this.Disconnect(item);
			}						
		}	

		/// <summary>
		/// Creates a queued thread pool job that will disconnect and reconnect a connection for every address book item in the address book
		/// </summary>
		public virtual void ReconnectAll(bool verboseSessions, bool autoRecv)
		{
			foreach(AddressBookItem item in _addressBook.Items)
			{
				this.Connect(item, true /*diconnect first*/, verboseSessions, autoRecv);
			}
		}

		/// <summary>
		/// Creates a queued thread pool job that will connect a connection for the address book item
		/// </summary>
		/// <param name="item"></param>
		/// <param name="disconnectFirst"></param>
		public virtual void Connect(AddressBookItem item, bool disconnectFirst, bool verboseSession, bool autoRecv)
		{
			BackgroundThreadPoolJob job = new BackgroundThreadPoolJob(
				item.Id, 
				true, 
				this, 
				new object[] {item, disconnectFirst, verboseSession, autoRecv}, 
				new BackgroundThreadStartEventHandler(OnConnectSessionForAddressBookItem), 
				null);

			_threadPool.QueueJob(job);
		}

		/// <summary>
		/// Creates a queued thread pool job that will disconnect a connection for the address book item
		/// </summary>
		/// <param name="item"></param>
		public virtual void Disconnect(AddressBookItem item)
		{
			BackgroundThreadPoolJob job = new BackgroundThreadPoolJob(
				item.Id, 
				true, 
				this, 
				new object[] {item}, 
				new BackgroundThreadStartEventHandler(OnDisconnectSessionForAddressBookItem), 
				null);

			_threadPool.QueueJob(job);
		}

		/// <summary>
		/// Allows a context to be pre-bound to an item so that the estabilish context event will already contain the context
		/// </summary>
		/// <param name="item"></param>
		/// <param name="context"></param>
		public virtual void BindContextToItem(AddressBookItem item, IAddressBookItemContext context)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (context == null)
				throw new ArgumentNullException("context");

			lock(_contextLookupTable)
			{
				if (_contextLookupTable.ContainsKey(item))
					_contextLookupTable[item] = context;
				else
					_contextLookupTable.Add(item, context);
			}
		}

		/// <summary>
		/// Unbinds a context from an item
		/// </summary>
		/// <param name="item"></param>
		public virtual void UnbindContextFromItem(AddressBookItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			lock(_contextLookupTable)
			{
				if (_contextLookupTable.ContainsKey(item))
					_contextLookupTable.Remove(item);
			}
		}

		/// <summary>
		/// Looks up the context currently bound to an item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual IAddressBookItemContext LookupContextForItem(AddressBookItem item)
		{
			lock(_contextLookupTable)
			{
				if (_contextLookupTable.ContainsKey(item))
					return _contextLookupTable[item] as IAddressBookItemContext;
			}
			return null;
		}

		#endregion 

		#region My Public Properties

		/// <summary>
		/// Returns the address book being managed
		/// </summary>
		public AddressBook AddressBook
		{
			get
			{
				return _addressBook;
			}

			set
			{
				if (value == null)
					throw new ArgumentNullException("AddressBook");

				bool temp = _enabled;

				this.EnableAddressBookEvents(false);

				_addressBook = value;

				this.EnableAddressBookEvents(temp);
			}
		}

		/// <summary>
		/// Returns the list of connection managers being used to manage the individual sessions connected to the remote hosts defined by each address book item
		/// </summary>
		public AddressBookItemConnectionManagerList ConnectionManagers
		{
			get
			{
				return _sessionManagers;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the events of the address book should control the sessions connected to each address book item
		/// </summary>
		public bool InterceptAddressBookEvents
		{
			get
			{
				return _enabled;
			}
			set
			{
				this.EnableAddressBookEvents(value);
			}
		}

		/// <summary>
		/// Returns the internal thread pool used to connect and disconnect the sessions
		/// </summary>
		public BackgroundThreadPool ThreadPool
		{
			get
			{
				return _threadPool;
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
				// first check and see if we've already got a context bound to this item
				IAddressBookItemContext context = this.LookupContextForItem(e.Manager.AddressBookItem);
				
				// if we have a pre-bound context put it in the event args!
				if (context != null)
					e.Context = context;

				// if there are any event handler's to fire this event to
				if (this.ConnectionEstablishContext != null)
				{
					// raise the event
					this.ConnectionEstablishContext(sender, e);
				
					// if a context was returned to us, bind the item to the context
					if (e.Context != null)
						this.BindContextToItem(e.Manager.AddressBookItem, e.Context);					
				}
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
		/// Raises the ConnectionResolvingAddress event
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
				e.Manager.ConnectionEstablishContext -= new AddressBookItemConnectionManagerContextEventHandler(OnConnectionEstablishContext);
				e.Manager.BeforeConnectionOpened -= new AddressBookItemConnectionManagerCancelEventHandler(OnBeforeConnectionOpened);
				e.Manager.ConnectionResolvingAddress -= new AddressBookItemConnectionManagerResolvingAddressEventHandler(OnConnectionResolvingAddress);
				e.Manager.BeforeConnectionClosed -= new AddressBookItemConnectionManagerCancelEventHandler(OnBeforeConnectionClosed);
				e.Manager.ConnectionOpened -= new AddressBookItemConnectionManagerEventHandler(OnConnectionOpened);
				e.Manager.ConnectionClosed -= new AddressBookItemConnectionManagerEventHandler(OnConnectionClosed);
				e.Manager.ConnectionException -= new AddressBookItemConnectionManagerExceptionEventHandler(OnConnectionException);
				
				this.UnbindContextFromItem(e.Manager.AddressBookItem);

				lock(_sessionManagers)
					_sessionManagers.Remove(e.Manager);				

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
		/// Creates a connection manager for the address book item, and connects the connection to the remote host specified by the address book item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectSessionForAddressBookItem(object sender, BackgroundThreadStartEventArgs e)
		{			
			// retrieve the address book item that is requiring attention
			AddressBookItem item = (AddressBookItem)e.Args[0];
			bool disconnectFirst = (bool)e.Args[1];
			bool verboseSession	 = (bool)e.Args[2];
			bool autoRecv		 = (bool)e.Args[3];

			try
			{  
				/*
				 * if we are supposed to disconnect the current connection first
				 * */
				if (disconnectFirst)
				{
					lock(_sessionManagers)
					{
						// look up the item to see if an existing connection is already in use
						AddressBookItemConnectionManager prevManager = _sessionManagers[item];

						// if a connection manager is found for the item then disconnect it
						if (prevManager != null)
							prevManager.Disconnect();
					}
				}

				// create a new tcp connection in verbose mode
				HttpConnection connection = new HttpConnection(verboseSession);       
            
				// create a new connection manager to manage the connection
				AddressBookItemConnectionManager manager = new AddressBookItemConnectionManager(item, connection);

				// wire up to the manager's events
				manager.ConnectionEstablishContext += new AddressBookItemConnectionManagerContextEventHandler(OnConnectionEstablishContext);
				manager.ConnectionResolvingAddress += new AddressBookItemConnectionManagerResolvingAddressEventHandler(OnConnectionResolvingAddress);
				manager.BeforeConnectionOpened += new AddressBookItemConnectionManagerCancelEventHandler(OnBeforeConnectionOpened);
				manager.BeforeConnectionClosed += new AddressBookItemConnectionManagerCancelEventHandler(OnBeforeConnectionClosed);
				manager.ConnectionOpened += new AddressBookItemConnectionManagerEventHandler(OnConnectionOpened);
				manager.ConnectionClosed += new AddressBookItemConnectionManagerEventHandler(OnConnectionClosed);
				manager.ConnectionException += new AddressBookItemConnectionManagerExceptionEventHandler(OnConnectionException);
				
				// create the thread context that will enable us to determine the background thread upon which this connection manager is operating
				manager.ThreadContext = new AddressBookItemBackgroundThreadContext((BackgroundThread)sender, item);

				// instruct the manager to connect the connection to the remote host
				// pass it instructions on whether it should begin receiving automatically or not
				// NOTE: In almost every case from a client created connection this will be false. 
				//		 Only server created sessions will be set to automatically receive.
				manager.Connect(autoRecv, manager.ThreadContext);
            
				// add the connection manager to our list of connection managers
				lock(_sessionManagers)
					_sessionManagers.Add(manager);				
			}
			catch(ThreadAbortException)
			{
				// ignore thread abort exceptions
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);				
			}
		}

		/// <summary>
		/// Looks up the connection manager responsible for the address book item and disconnects it from the remote host
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDisconnectSessionForAddressBookItem(object sender, BackgroundThreadStartEventArgs e)
		{
			// retrieve the address book item that is requiring attention
			AddressBookItem item = (AddressBookItem)e.Args[0];

			try
			{  
				lock(_sessionManagers)
				{
					AddressBookItemConnectionManager manager = _sessionManagers[item];
					if (manager != null)
						manager.Disconnect();
				}
			}
			catch(ThreadAbortException)
			{
				// ignore thread abort exceptions
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);				
			}
		}

		/// <summary>
		/// Intercepts the events coming from the address book in order to manage the sessions related to each item in the book
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddressBookChanged(object sender, AddressingEventArgs e)
		{
			/*
			 * watch for changes to the address book and intercept them here
			 * if an item is added then we'll connect them
			 * and if an item is removed then we'll disconnect it
			 * but if the entire book is removed then we'll disconnect them all
			 * */
			switch(e.Action)
			{
				case AddressingActions.Added:
					break;

				case AddressingActions.Changed:
					break;

				case AddressingActions.Removed:
					break;
			};
		}

		/// <summary>
		/// Enables or disables the events for the address book
		/// </summary>
		/// <param name="enabled"></param>
		private void EnableAddressBookEvents(bool enabled)
		{
			if (_addressBook == null)
				return;

			if (enabled && !_enabled)
				_addressBook.Changed += new AddressingEventHandler(OnAddressBookChanged);
			else
				_addressBook.Changed -= new AddressingEventHandler(OnAddressBookChanged);			

			_enabled = enabled;
		}

		#endregion
	}
}
