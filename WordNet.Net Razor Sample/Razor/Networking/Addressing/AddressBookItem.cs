using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Provides a means of describing a remote EndPoint using address, port, nickname, location, description, and unique identifier.
	/// </summary>
	[Serializable()]
	public class AddressBookItem : ISerializable, ICloneable	
	{		
		protected string _id;
		protected string _address;
		protected int _port;
		protected string _name;
		protected string _description;
		protected AddressBook _parent;

		#region My Public Events

		/// <summary>
		/// Occurs when the address changes
		/// </summary>
		public event AddressingEventHandler Changed;

		/// <summary>
		/// Occurs before the name is changed
		/// </summary>
		public event NameChangeEventHandler BeforeNameChanged;
		
		#endregion

		#region My Explicit Operators

		/// <summary>
		/// Explicit operator for casting AddressBookItem instances to IPEndPoint instances (NOTE: Interally calls AddressBookItem.Resolve() which in turn uses DNS)
		/// </summary>
		/// <param name="address">The AddressBookItem to cast to an IPEndPoint</param>
		/// <returns></returns>
		public static explicit operator IPEndPoint(AddressBookItem item)
		{
			if (item == null)
				throw new ArgumentNullException("AddressBookItem", "A reference to a null AddressBookItem cannot be cast to an IPEndPoint.");

			try
			{
				// first try and parse the address out
				// it may be a IPv4 dotted quad or in IPv6 colon-hex notation
				IPAddress address = IPAddress.Parse(item.Address);

				// return a new end point without ever hitting dns
				return new IPEndPoint(address, item.Port);
			}
			catch(Exception)
			{
				// try first then fall back on dns because connecting via ip's should be faster and try to bypass dns all together
			}

			// resolve the address using DNS
			IPHostEntry he = item.Resolve();

			// create and return a new IP end point based on the address and port
			return new IPEndPoint(he.AddressList[0], item.Port);
		}
				
		#endregion

		/// <summary>
		/// Initializes a new instance of the AddressBookItem class
		/// </summary>
		public AddressBookItem()
		{
			_id = Guid.NewGuid().ToString();
			_port = DefaultNetworkOptions.BASEPORT;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookItem class
		/// </summary>
		/// <param name="address">The IP address or Dns hostname of the remote end point (ie. 127.0.0.1 or Localhost)</param>
		/// <param name="port">The port number to use when negotiating a connection to the remote end point</param>
		public AddressBookItem(string address, int port) : this()
		{
			_address = address;
			_port = port;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookItem class
		/// </summary>
		/// <param name="address">The IP address or Dns hostname of the remote end point (ie. 127.0.0.1 or Localhost)</param>
		/// <param name="port">The port number to use when negotiating a connection to the remote end point</param>
		/// <param name="nickname">The nickname that will be displayed in place of the address for the remote end point</param>
		/// <param name="location">The location where the remote end point can be found</param>
		public AddressBookItem(string address, int port, string name) : this(address, port)
		{
			_name = name;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookItem class
		/// </summary>
		/// <param name="address">The IP address or Dns hostname of the remote end point (ie. 127.0.0.1 or Localhost)</param>
		/// <param name="port">The port number to use when negotiating a connection to the remote end point</param>
		/// <param name="nickname">The nickname that will be displayed in place of the address for the remote end point</param>
		/// <param name="location">The location where the remote end point can be found</param>
		/// <param name="description">A description for the remote end point</param>
		public AddressBookItem(string address, int port, string name, string description) : this(address, port, name)
		{
			_description = description;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookItem class
		/// </summary>
		/// <param name="address">Another AddressBookItem instance to copy properties from (a cloning constructor)</param>
		public AddressBookItem(AddressBookItem item)
		{
			_id = item.Id;
			_address = item.Address;
			_port = item.Port;
			_name = item.Name;
			_description = item.Description;
		}		
		
		#region My Public Methods

		/// <summary>
		/// Resolves the address from a DNS hostname or IP address to an System.Net.IPHostEntry instance
		/// </summary>
		/// <returns></returns>
		public IPHostEntry Resolve()
		{
			return Dns.Resolve(_address);
		}		

		/// <summary>
		/// Writes this instances properties to the item using it's public properties (Only data related fields are assigned)
		/// </summary>
		/// <param name="item">The item to write properties to</param>
		public void WriteProperties(AddressBookItem item)
		{
			item.Name = _name;
			item.Port = _port;
			item.Description = _description;
			item.Address = _address;
		}

		/// <summary>
		/// Creates a new unique identifier for the item
		/// </summary>
		internal void GetNewId()
		{
			_id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Orphans the item, killing it's parent and wiping out any attached event handlers
		/// </summary>
		public virtual void Orphan()
		{
			this.Changed = null;
			this.BeforeNameChanged = null;
			_parent = null;
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Gets the unique identifier for this address book item
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets or sets the IP address or Dns hostname of the remote end point (ie. 127.0.0.1 or Localhost)
		/// </summary>
		public string Address
		{
			get
			{
				return _address;
			}
			set
			{
				// ignore the change if it's the same
				if (string.Compare(_address, value, true) == 0)
					return;

				// preliminarily validate the address (save netbios & dns confusion by eliminating invalid characters now)
				if (!AddressValidator.IsValid(value))
					throw new AddressNotValidException(value);

				_address = value;
				this.OnChanged(this, new AddressBookItemEventArgs(this, AddressingActions.Changed));
			}
		}
        
		/// <summary>
		/// Gets or sets the port number to use when negotiating a connection to the remote end point
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				// ignore the change if it's the same
				if (_port == value)
					return;

				if (!PortValidator.IsValid(value))
					throw new PortOutOfRangeException(value);

				_port = value;
				this.OnChanged(this, new AddressBookItemEventArgs(this, AddressingActions.Changed));
			}
		}

		/// <summary>
		/// Gets or sets the name that will be displayed in place of the address for the remote end point
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{		
				// ignore the change if it's the same
				if (string.Compare(_name, value, true) == 0)
					return;

				// use the name change event arts to validate the name of the address
				NameChangeEventArgs e = new NameChangeEventArgs(_name, value);
				this.OnBeforeNameChanged(this, e);
				if (e.Cancel)
					return;

				_name = value;
				this.OnChanged(this, new AddressBookItemEventArgs(this, AddressingActions.Changed));
			}
		}

		/// <summary>
		/// Gets or sets the description of the remote end point
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				// ignore the change if it's the same
				if (string.Compare(_description, value, true) == 0)
					return;

				_description = value;
				this.OnChanged(this, new AddressBookItemEventArgs(this, AddressingActions.Changed));
			}
		}


		
		/// <summary>
		/// Returns the address book that owns this address book item
		/// </summary>
		public AddressBook Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}	
		
		#endregion

		#region My Event Raising Methods

		/// <summary>
		/// Raises the Changed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected internal virtual void OnChanged(object sender, AddressingEventArgs e)
		{
			try
			{
				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);	
			}
		}
		
		/// <summary>
		/// Raises the BeforeNameChangedEvent event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeNameChanged(object sender, NameChangeEventArgs e)
		{			
			if (!NameValidator.IsValid(e.NameAfter))
				throw new NameNotValidException(e.NameAfter);

			if (this.BeforeNameChanged != null)
				this.BeforeNameChanged(sender, e);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Deserializes the address book item
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AddressBookItem(SerializationInfo info, StreamingContext context)
		{			
			_id = (string)info.GetValue("Id", typeof(string));
			_address = (string)info.GetValue("Address", typeof(string));
			_port = (int)info.GetValue("Port", typeof(int));
			_name = (string)info.GetValue("Name", typeof(string));
			_description = (string)info.GetValue("Description", typeof(string));
		}

		/// <summary>
		/// Serializes the address book item
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Id", _id, typeof(string));
			info.AddValue("Address", _address, typeof(string));
			info.AddValue("Port", _port, typeof(int));
			info.AddValue("Name", _name, typeof(string));
			info.AddValue("Description", _description, typeof(string));
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new AddressBookItem(this);
		}

		#endregion
	}
}
