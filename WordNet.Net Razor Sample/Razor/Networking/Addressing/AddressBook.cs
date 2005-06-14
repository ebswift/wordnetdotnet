using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBook.
	/// </summary>
	[Serializable()]
	public class AddressBook : ISerializable, IDeserializationCallback, ICloneable
	{
		protected string _id;
		protected string _name;
		protected string _description;
		protected AddressBookItemList _items;
		protected AddressBookDirectory _parent;

		#region My Public Events

		/// <summary>
		/// Occurs when a change is made to the address book
		/// </summary>
		public event AddressingEventHandler Changed;

		/// <summary>
		/// Occurs before the name is changed
		/// </summary>
		public event NameChangeEventHandler BeforeNameChanged;

		#endregion

		/// <summary>
		/// Initializes a new instance of the AddressBook class
		/// </summary>
		public AddressBook()
		{
			_id = Guid.NewGuid().ToString();
			_items = new AddressBookItemList();
			_items.Changed += new AddressingEventHandler(this.OnChanged);
			_items.Parent = this;
		}
		
		/// <summary>
		/// Initializes a new instance of the AddressBook class
		/// </summary>
		/// <param name="name">The name to give the address book</param>
		public AddressBook(string name) : this()
		{
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBook class
		/// </summary>
		/// <param name="name">The name to give the address book</param>
		/// <param name="description">A description to give the address book</param>
		public AddressBook(string name, string description) : this(name)
		{
			_description = description;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBook class
		/// </summary>
		/// <param name="name">The name to give the address book</param>
		/// <param name="description">A description to give the address book</param>
		/// <param name="addresses">A list of addresses to give the address</param>
		public AddressBook(string name, string description, AddressBookItemList addresses) : this(name, description)
		{
			foreach(AddressBookItem address in this.Items)
				this.Items.Add(address);
		}

		/// <summary>
		/// Initializes a new instance of the AddressBook class
		/// </summary>
		/// <param name="addressBook">The address book to copy</param>
		public AddressBook(AddressBook addressBook) : this(addressBook.Name, addressBook.Description, addressBook.Items)
		{			
			_id = addressBook.Id;
		}
		
		/// <summary>
		/// Writes this instances properties to the item using it's public properties (Only data related fields are assigned)
		/// </summary>
		/// <param name="item">The item to write properties to</param>
		public void WriteProperties(AddressBook book)
		{
			book.Name = _name;			
			book.Description = _description;			
		}

		/// <summary>
		/// Creates a new unique identifier for the book
		/// </summary>
		internal void GetNewId()
		{
			_id = Guid.NewGuid().ToString();
		}

		#region My Public Properties

		/// <summary>
		/// Gets the unique identifier for this address book
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets or sets the name given to this AddressBook
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				// ignore it if it's the same
				if (string.Compare(_name, value, true) == 0)
					return;

				// use the name change event arts to validate the name of the address
				NameChangeEventArgs e = new NameChangeEventArgs(_name, value);
				this.OnBeforeNameChanged(this, e);
				if (e.Cancel)
					return;

				_name = value;
				this.OnChanged(this, new AddressBookEventArgs(this, AddressingActions.Changed));
			}
		}
		
		/// <summary>
		/// Gets or sets the description given to this address book
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				// ignore it if it's the same
				if (string.Compare(_description, value, true) == 0)
					return;

				_description = value;
				this.OnChanged(this, new AddressBookEventArgs(this, AddressingActions.Changed));
			}
		}

		/// <summary>
		/// Returns the list of address book items stored in this address book
		/// </summary>
		public AddressBookItemList Items
		{
			get
			{
				return _items;
			}
		}
		
		/// <summary>
		/// Returns the address book directory that owns this address book
		/// </summary>
		public AddressBookDirectory Parent
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

		/// <summary>
		/// Orphans the item, killing it's parent and wiping out any attached event handlers
		/// </summary>
		public virtual void Orphan()
		{
			this.Changed = null;
			this.BeforeNameChanged = null;
			_parent = null;
		}

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

		#region ICloneable Members

		public object Clone()
		{
			return new AddressBook(this);
		}

		#endregion						

		#region ISerializable Members

		/// <summary>
		/// Deserializes the address book
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AddressBook(SerializationInfo info, StreamingContext context) : this()
		{			
			_id = (string)info.GetValue("Id", typeof(string));
			_name = (string)info.GetValue("Name", typeof(string));
			_description = (string)info.GetValue("Description", typeof(string));
			_items = (AddressBookItemList)info.GetValue("Items", typeof(AddressBookItemList));			
		}

		/// <summary>
		/// Serializes the address book 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Id", _id, typeof(string));
			info.AddValue("Name", _name, typeof(string));
			info.AddValue("Description", _description, typeof(string));
			info.AddValue("Items", _items, typeof(AddressBookItemList));
		}

		#endregion

		#region IDeserializationCallback Members

		/// <summary>
		/// Occurs when the address book's deserialization is complete
		/// </summary>
		/// <param name="sender"></param>
		public void OnDeserialization(object sender)
		{						
			// wire up to the events of the items list here
			_items.Changed += new AddressingEventHandler(this.OnChanged);
			_items.Parent = this;
		}

		#endregion
	}
}
