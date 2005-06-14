using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookList.
	/// </summary>
	[Serializable()]
	public class AddressBookList : CollectionBase, ISerializable, IDeserializationCallback, ICloneable
	{
		[NonSerialized()]
		protected AddressBookDirectory _parent;
		protected ArrayList _array;
		
		/// <summary>
		/// Occurs when a change is made to the address book list
		/// </summary>
		public event AddressingEventHandler Changed;

		/// <summary>
		/// Explicitly converts a list of AddressBook instances to an array of AddressBook instances
		/// </summary>
		/// <param name="addressBooks">The address book list to convert</param>
		/// <returns></returns>		
		public static explicit operator AddressBook[](AddressBookList addressBooks)
		{
			return addressBooks.InnerList.ToArray(typeof(AddressBook)) as AddressBook[];
		}

		/// <summary>
		/// Initializes a new instance of the AddressBookList class
		/// </summary>
		public AddressBookList()
		{

		}

		/// <summary>
		/// Initializes a new instance of the AddressBookList class
		/// </summary>
		/// <param name="addressBooks"></param>
		public AddressBookList(AddressBookList list) : this()
		{
			if (list == null)
				throw new ArgumentNullException("AddressBookList", "A null reference to an AddressBookList cannot be used to create a new list.");

			foreach(AddressBook addressBook in list)
				this.Add(addressBook);
		}
	
		public new ArrayList InnerList
		{
			get
			{
				return base.InnerList;
			}
		}

		/// <summary>
		/// Adds the address book to the list
		/// </summary>
		/// <param name="addressBook"></param>
		public void Add(AddressBook addressBook)
		{
			if (addressBook == null)
				throw new ArgumentNullException("AddressBook", "A null reference to an AddressBook cannot be added to the list.");

			if (this.Contains(addressBook))
				throw new NameNotUniqueException(addressBook.Name);

			addressBook.Changed += new AddressingEventHandler(this.OnChanged);
			addressBook.BeforeNameChanged += new NameChangeEventHandler(this.OnBeforeAddressBookNameChanged);
			
			base.InnerList.Add(addressBook);

			addressBook.Parent = _parent;

			/*
			 * raise the event
			 * 
			 * starting it at the object level, which will trickle all the way up through the object heirarchies
			 * normally, the "this" pointer would be the context of the call, but because we want the object
			 * to get the add and remove events as well, we start the call with it
			 * */
			AddressBookEventArgs e = new AddressBookEventArgs(addressBook, AddressingActions.Added);
			addressBook.OnChanged(this, e);
//			this.OnChanged(this, e);
		}

		public void Add(AddressBook addressBook, bool overwrite)
		{
			if (!overwrite)
			{
				this.Add(addressBook);
				return;
			}

			if (addressBook == null)
				throw new ArgumentNullException("AddressBook", "A null reference to an AddressBook cannot be added to the list.");

			// find by name...
			if (this.Contains(addressBook))
			{
				AddressBook existingBook = this[addressBook.Name];
				addressBook.WriteProperties(existingBook);				
				return;
			}
			else 
			{
				// try and find it by id
				AddressBook existingBook = this.FindById(addressBook.Id);

				// if we have a hit, that means this is the same object, just renamed
				if (existingBook != null)
					// so give it a new id
					addressBook.GetNewId();									
			}
			
			addressBook.Changed += new AddressingEventHandler(this.OnChanged);
			addressBook.BeforeNameChanged += new NameChangeEventHandler(this.OnBeforeAddressBookNameChanged);
			
			base.InnerList.Add(addressBook);

			addressBook.Parent = _parent;

			/*
			 * raise the event
			 * 
			 * starting it at the object level, which will trickle all the way up through the object heirarchies
			 * normally, the "this" pointer would be the context of the call, but because we want the object
			 * to get the add and remove events as well, we start the call with it
			 * */
			AddressBookEventArgs e = new AddressBookEventArgs(addressBook, AddressingActions.Added);
			addressBook.OnChanged(this, e);
//			this.OnChanged(this, e);
		}

		/// <summary>
		/// Removes the address book from the list
		/// </summary>
		/// <param name="addressBook"></param>
		public void Remove(AddressBook addressBook)
		{
			if (this.Contains(addressBook))
			{
				/*
				* raise the event
				* 
				* starting it at the object level, which will trickle all the way up through the object heirarchies
				* normally, the "this" pointer would be the context of the call, but because we want the object
				* to get the add and remove events as well, we start the call with it
				* */
				AddressBookEventArgs e = new AddressBookEventArgs(addressBook, AddressingActions.Removed);
				addressBook.OnChanged(this, e);
//				this.OnChanged(this, e);						

				addressBook.Changed -= new AddressingEventHandler(this.OnChanged);
				addressBook.BeforeNameChanged -= new NameChangeEventHandler(this.OnBeforeAddressBookNameChanged);

				base.InnerList.Remove(addressBook);

				addressBook.Parent = null;
			}	
		}
		
		/// <summary>
		/// Determines if the list contains the address book
		/// </summary>
		/// <param name="addressBook"></param>
		/// <returns></returns>
		public bool Contains(AddressBook addressBook)
		{
			if (addressBook == null)
				throw new ArgumentNullException("AddressBook", "A null reference to an AddressBook cannot be compared to items in the list.");

			return this.Contains(addressBook.Name);
		}
		
		/// <summary>
		/// Determines if the list contains an address book with the same name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			foreach(AddressBook addressBook in base.InnerList)
				if (string.Compare(addressBook.Name, name, true) == 0)
					return true;
			return false;
		}

		/// <summary>
		/// Returns the AddressBook that has the specified id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public AddressBook FindById(string id)
		{
			foreach(AddressBook ab in base.InnerList)
				if (string.Compare(ab.Id, id, true) == 0)
					return ab;
			return null;
		}

		/// <summary>
		/// Returns the next suggested new address book name
		/// </summary>
		/// <returns></returns>
		public string GetNextNewItemName()
		{			
			return this.GetNextNewItemName(1);
		}

		public const string DefaultName = "New Address Book";

		/// <summary>
		/// Returns a name for the next new address book name 
		/// </summary>
		/// <returns></returns>
		public string GetNextNewItemName(int startingNumber)
		{
			string name = (startingNumber > 1 ? string.Format("{0} {1}", DefaultName, startingNumber) : DefaultName);

			foreach(AddressBook ab in base.InnerList)
			{
				if (string.Compare(ab.Name, name, true) == 0)
				{
					name = this.GetNextNewItemName(++startingNumber);
					break;
				}
			}
			
			return name;
		}

		/// <summary>
		/// Returns the AddressBook stored at the specific index
		/// </summary>
		public AddressBook this[int index]
		{
			get
			{
				return base.InnerList[index] as AddressBook;
			}
		}
		
		/// <summary>
		/// Returns the AddressBook that has the specified name
		/// </summary>
		public AddressBook this[string name]
		{
			get
			{
				foreach(AddressBook addressBook in base.InnerList)
					if (string.Compare(addressBook.Name, name, true) == 0)
						return addressBook;
				return null;
			}
		}				
		
		/// <summary>
		/// Returns the address book directory that owns this address book list
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
				
				// correct all of our item's parent pointers too
				foreach(AddressBook ab in base.InnerList)
					ab.Parent = _parent;
			}
		}

		#region My Event Raising Methods

		/// <summary>
		/// Raises the Changed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnChanged(object sender, AddressingEventArgs e)
		{
			try
			{
				if (sender.GetType() == typeof(AddressBook) && e.Action == AddressingActions.Changed)
					sender = this;

				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Captures and handles the BeforeAddressBookNameChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeAddressBookNameChanged(object sender, NameChangeEventArgs e)
		{
			try
			{
				foreach(AddressBook addressBook in base.InnerList)
					if (string.Compare(addressBook.Name, e.NameAfter, true) == 0)
						throw new NameNotUniqueException(addressBook.Name);
			}
			catch(Exception ex)
			{				
				e.Cancel = true;
				throw ex;
			}
		}
		
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new AddressBookList(this);
		}

		#endregion	

		#region ISerializable Members
		
		/// <summary>
		/// Occurs when the object is being created from a serialized state
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AddressBookList(SerializationInfo info, StreamingContext context) : this()
		{			
			_array = (ArrayList)info.GetValue("ArrayList", typeof(ArrayList));	
		}

		/// <summary>
		/// Occurs when the object is being serialized
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ArrayList", base.InnerList, typeof(ArrayList));
		}

		#endregion

		#region IDeserializationCallback Members

		/// <summary>
		/// Occurs after deserialization is complete
		/// </summary>
		/// <param name="sender"></param>
		public void OnDeserialization(object sender)
		{						
			foreach(AddressBook ab in _array)
				this.Add(ab);
		}

		#endregion
	}
}
