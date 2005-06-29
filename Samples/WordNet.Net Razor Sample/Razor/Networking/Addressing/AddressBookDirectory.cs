using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectory.
	/// </summary>
	[Serializable()]
	public class AddressBookDirectory : ISerializable, IDeserializationCallback
	{
		protected string _name;
		protected AddressBookList _books;
		protected bool _importInProgress;

		#region My Public Events

		/// <summary>
		/// Occurs when a change is made to an AddressBook or AddressBookItem in the directory
		/// </summary>
		public event AddressingEventHandler Changed;

		/// <summary>
		/// Occurs after a change is made to an AddressBookItem in the directory
		/// </summary>
		public event AddressBookItemEventHandler AddressBookItemChanged;

		/// <summary>
		/// Occurs after a change is made to an AddressBook in the directory
		/// </summary>
		public event AddressBookEventHandler AddressBookChanged;

		/// <summary>
		/// Occurs before the name is changed
		/// </summary>
		public event NameChangeEventHandler BeforeNameChanged;

		#endregion

		/// <summary>
		/// Initializes a new instance of the AddressBookDirectory class
		/// </summary>
		public AddressBookDirectory()
		{
			_books = new AddressBookList();	
			_books.Changed += new AddressingEventHandler(this.OnChanged);
			_books.Parent = this;
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookDirectory class
		/// </summary>
		public AddressBookDirectory(string name) : this()
		{
			_name = name;			
		}

		#region My Public Properties

		/// <summary>
		/// Returns the list of address books maintained by this directory
		/// </summary>
		public AddressBookList Books
		{
			get
			{
				return _books;
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
				// use the name change event arts to validate the name of the address
				NameChangeEventArgs e = new NameChangeEventArgs(_name, value);
				this.OnBeforeNameChanged(this, e);
				if (e.Cancel)
					return;

				_name = value;
				this.OnChanged(this, new AddressBookDirectoryEventArgs(this, AddressingActions.Changed));
			}
		}
		
		/// <summary>
		/// Returns a flag that indicates whether an import is in progress
		/// </summary>
		public bool ImportInProgress
		{
			get
			{
				return _importInProgress;
			}
			set
			{
				_importInProgress = value;
			}
		}

		#endregion

		#region My Event Raising Methods

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

		/// <summary>
		/// Raises the Changed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected internal virtual void OnChanged(object sender, AddressingEventArgs e)
		{
			try
			{
				/*
				 * always call the universal changed event
				 * 
				 * the parameter's are weak-typed, but they can be explicitly cast 
				 * if you want the strongly typed event args
				 * wait and respond to the next set ... see below
				 * */

				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}

			try
			{
				/*
				 * determine the type of event args
				 * */

				// address book changes
				if (e.GetType() == typeof(AddressBookEventArgs))
				{			
					this.OnAddressBookChanged(sender, (AddressBookEventArgs)e);
					return;
				}
				
				// address book item changes
				if (e.GetType() == typeof(AddressBookItemEventArgs))
				{
					this.OnAddressBookItemChanged(sender, (AddressBookItemEventArgs)e);
					return;
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
		
		/// <summary>
		/// Raises the AddressBookChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnAddressBookChanged(object sender, AddressBookEventArgs e)
		{
			try
			{
				if (this.AddressBookChanged != null)
					this.AddressBookChanged(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the AddressBookItemChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnAddressBookItemChanged(object sender, AddressBookItemEventArgs e)
		{
			try
			{
				if (this.AddressBookItemChanged != null)
					this.AddressBookItemChanged(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
		
		#endregion

		#region ISerializable Members

		/// <summary>
		/// Deserializes the address book directory
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AddressBookDirectory(SerializationInfo info, StreamingContext context) : this()
		{			
			_name = (string)info.GetValue("Name", typeof(string));
			_books = (AddressBookList)info.GetValue("Books", typeof(AddressBookList));
		}

		/// <summary>
		/// Serializes the address book directory
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", _name, typeof(string));
			info.AddValue("Books", _books, typeof(AddressBookList));			
		}

		#endregion
		
		#region IDeserializationCallback Members

		/// <summary>
		/// Occurs when the address book directory's deserialization is complete
		/// </summary>
		/// <param name="sender"></param>
		public void OnDeserialization(object sender)
		{			
			// wire up to the events of the book list here
			_books.Changed += new AddressingEventHandler(this.OnChanged);
			_books.Parent = this;
		}

		#endregion
	}
}
