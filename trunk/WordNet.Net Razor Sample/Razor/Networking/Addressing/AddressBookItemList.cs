using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Provides a collection of AddressBookItem objects
	/// </summary>
	[Serializable()]
	public class AddressBookItemList : CollectionBase, ISerializable, IDeserializationCallback, ICloneable
	{
		[NonSerialized()]
		protected AddressBook _parent;
		protected ArrayList _array;

		#region My Public Events

		/// <summary>
		/// Occurs when the item list changes
		/// </summary>
		public event AddressingEventHandler Changed;

		#endregion

		#region My Explicit Operators

		/// <summary>
		/// Explicitly converts a list of AddressBookItem instances to an array of AddressBookItem instances
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static explicit operator AddressBookItem[](AddressBookItemList list)
		{
			return list.InnerList.ToArray(typeof(AddressBookItem)) as AddressBookItem[];
		}
        
		#endregion

		/// <summary>
		/// Initializes a new instance of the AddressBookItemList class
		/// </summary>
		public AddressBookItemList()
		{

		}
		
		/// <summary>
		/// Initializes a new instance of the AddressBookItemList class
		/// </summary>
		/// <param name="list">The list to copy</param>
		public AddressBookItemList(AddressBookItemList list) : this()
		{
			if (list == null)
				throw new ArgumentNullException("AddressBookItemList", "A null reference to a AddressBookItemList cannot be used to create a new list.");
						
			foreach(AddressBookItem item in list)
				this.Add(item);
		}

		/// <summary>
		/// Returns the inner array list used to hold the items internally
		/// </summary>
		public new ArrayList InnerList
		{
			get
			{
				return base.InnerList;
			}
		}

		/// <summary>
		/// Adds a AddressBookItem to the list
		/// </summary>
		/// <param name="item"></param>
		public void Add(AddressBookItem item)
		{
			if (item == null)
				throw new ArgumentNullException("AddressBookItem", "A null reference to a AddressBookItem cannot be added to the list.");
			
			if (this.Contains(item))
				throw new NameNotUniqueException(item.Name);

			// bind to the end point's events
			item.Changed += new AddressingEventHandler(this.OnChanged);
            item.BeforeNameChanged += new NameChangeEventHandler(this.OnBeforeAddressNameAddressChanged);

			// add the item
			base.InnerList.Add(item);			
			
			// set the item's parent
			item.Parent = _parent;

			/*
			 * raise the event
			 * 
			 * starting it at the object level, which will trickle all the way up through the object heirarchies
			 * normally, the "this" pointer would be the context of the call, but because we want the object
			 * to get the add and remove events as well, we start the call with it
			 * */
			AddressBookItemEventArgs e = new AddressBookItemEventArgs(item, AddressingActions.Added);
			item.OnChanged(this, e); 
			//this.OnChanged(this, e);
		}
		
		public void Add(AddressBookItem item, bool overwrite)
		{
			// if we're not overwriting, just add it like normal
			if (!overwrite)
			{
				this.Add(item);
				return;
			}

			// again we can't add null references
			if (item == null)
				throw new ArgumentNullException("AddressBookItem", "A null reference to an AddressBookItem cannot be added to the list.");

			// if there is a collision, go ahead and overwrite the existing one
			if (this.Contains(item))
			{
				// find the existing one
				AddressBookItem existingItem = this[item.Name];				

				// write the properties from the one coming in, to the existing one
				item.WriteProperties(existingItem);
				return;
			}
			else
			{
				// try and find it by id
				AddressBookItem existingItem = this.FindById(item.Id);

				// if we have a hit, that means this is the same object, just renamed
				if (existingItem != null)
					// so give it a new id
					item.GetNewId();	
			}

			// bind to the end point's events
			item.Changed += new AddressingEventHandler(this.OnChanged);
			item.BeforeNameChanged += new NameChangeEventHandler(this.OnBeforeAddressNameAddressChanged);

			// add the item
			base.InnerList.Add(item);			
			
			// set the item's parent
			item.Parent = _parent;

			/*
			 * raise the event
			 * 
			 * starting it at the object level, which will trickle all the way up through the object heirarchies
			 * normally, the "this" pointer would be the context of the call, but because we want the object
			 * to get the add and remove events as well, we start the call with it
			 * */
			AddressBookItemEventArgs e = new AddressBookItemEventArgs(item, AddressingActions.Added);
			item.OnChanged(this, e);
//			this.OnChanged(this, e);
		}

		/// <summary>
		/// Removes a AddressBookItem from the list
		/// </summary>
		/// <param name="item"></param>
		public void Remove(AddressBookItem item)
		{
			if (this.Contains(item))
			{				
				/*
				 * raise the event
				 * 
				 * starting it at the object level, which will trickle all the way up through the object heirarchies
				 * normally, the "this" pointer would be the context of the call, but because we want the object
				 * to get the add and remove events as well, we start the call with it
				 * */
				AddressBookItemEventArgs e = new AddressBookItemEventArgs(item, AddressingActions.Removed);				
				item.OnChanged(this, e);
//				this.OnChanged(this, e);

				item.Changed -= new AddressingEventHandler(this.OnChanged);
				item.BeforeNameChanged -= new NameChangeEventHandler(this.OnBeforeAddressNameAddressChanged);

				// remove the item 
				base.InnerList.Remove(item);			

				// orphan the item
				item.Parent = null;
			}
		}

		/// <summary>
		/// Determines if the list contains a AddressBookItem with the same id
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(AddressBookItem item)
		{
			if (item == null)
				throw new ArgumentNullException("AddressBookItem", "A null reference to a AddressBookItem cannot be compared to items in the list.");

			return this.Contains(item.Name);
		}

		/// <summary>
		/// Determines if the list contains a AddressBookItem with the same name
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			foreach(AddressBookItem item in base.InnerList)
				if (string.Compare(item.Name, name, true) == 0)
					return true;
			return false;
		}

		/// <summary>
		/// Returns the AddressBookItem stored at the specified index
		/// </summary>
		public AddressBookItem this[int index]
		{
			get
			{
				return base.InnerList[index] as AddressBookItem;
			}
		}

		/// <summary>
		/// Returns the AddressBookItem that has the specified name
		/// </summary>
		public AddressBookItem this[string name]
		{
			get
			{
				foreach(AddressBookItem item in base.InnerList)
					if (string.Compare(item.Name, name, true) == 0)
						return item;
				return null;
			}
		}	
		
		/// <summary>
		/// Returns the AddressBookItem that has the specified id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public AddressBookItem FindById(string id)
		{
			foreach(AddressBookItem item in base.InnerList)
				if (string.Compare(item.Id, id, true) == 0)
					return item;
			return null;
		}

		/// <summary>
		/// Returns the next suggested new item name
		/// </summary>
		/// <returns></returns>
		public string GetNextNewItemName()
		{			
			return this.GetNextNewItemName(1);
		}

		public const string DefaultName = "New Address Book Item";

		/// <summary>
		/// Returns the next suggested new item name
		/// </summary>
		/// <param name="startingNumber"></param>
		/// <returns></returns>
		public string GetNextNewItemName(int startingNumber)
		{
			string name = (startingNumber > 1 ? string.Format("{0} {1}", DefaultName, startingNumber) : DefaultName);

			foreach(AddressBookItem abi in base.InnerList)
			{
				if (string.Compare(abi.Name, name, true) == 0)
				{
					name = this.GetNextNewItemName(++startingNumber);
					break;
				}
			}
			
			return name;
		}

		/// <summary>
		/// Returns the address book that owns this address book item list
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

				// correct all of our item's parent pointers too
				foreach(AddressBookItem abi in base.InnerList)
					abi.Parent = _parent;
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
				if (sender.GetType() == typeof(AddressBookItem) && e.Action == AddressingActions.Changed)
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
		/// Captures and handles the BeforeAddressNameAddressChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeAddressNameAddressChanged(object sender, NameChangeEventArgs e)
		{	
			try
			{
				foreach(AddressBookItem item in base.InnerList)
					if (string.Compare(item.Name, e.NameAfter, true) == 0)
						throw new NameNotUniqueException(item.Name);
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
			return new AddressBookItemList(this);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Occurs when the object is being created from a serialized state
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AddressBookItemList(SerializationInfo info, StreamingContext context) : this()
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
			foreach(AddressBookItem abi in _array)
				this.Add(abi);
		}

		#endregion	
	}
}
