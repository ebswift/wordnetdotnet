using System;
using System.Diagnostics;
using System.Collections;
using Razor.Networking.Http;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookItemConnectionManagerList.
	/// </summary>
	public class AddressBookItemConnectionManagerList : CollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the AddressBookItemConnectionManagerList class
		/// </summary>
		public AddressBookItemConnectionManagerList()
		{
			
		}

		/// <summary>
		/// Adds a connection manager to the list
		/// </summary>
		/// <param name="manager"></param>
		public void Add(AddressBookItemConnectionManager manager)
		{
			if (!this.Contains(manager))
				base.InnerList.Add(manager);
		}

		/// <summary>
		/// Removes a connection manager from the list
		/// </summary>
		/// <param name="manager"></param>
		public void Remove(AddressBookItemConnectionManager manager)
		{
			if (this.Contains(manager))
				base.InnerList.Remove(manager);			
		}

		/// <summary>
		/// Removes the connection manager at the specified index from the list
		/// </summary>
		/// <param name="index"></param>
		public new void RemoveAt(int index)
		{
			base.InnerList.RemoveAt(index);
		}

		/// <summary>
		/// Returns a flag that indicates whether the sesion manager exists in the list
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public bool Contains(AddressBookItemConnectionManager manager)
		{
			return base.InnerList.Contains(manager);
		}

		/// <summary>
		/// Returns the connection manager at the specified index
		/// </summary>
		public AddressBookItemConnectionManager this[int index]
		{
			get
			{
				return (AddressBookItemConnectionManager)base.InnerList[index];
			}
		}

		/// <summary>
		/// Returns the connection manager responsible for the address book item
		/// </summary>
		public AddressBookItemConnectionManager this[AddressBookItem item]
		{
			get
			{
				foreach(AddressBookItemConnectionManager manager in base.InnerList)
					if (manager.AddressBookItem == item)
						return manager;
				return null;
			}
		}

		public AddressBookItemConnectionManager this[HttpConnection connection]
		{
			get
			{
				foreach(AddressBookItemConnectionManager manager in base.InnerList)
					if (manager.Connection.Id == connection.Id)
						return manager;
				return null;
			}
		}

		/// <summary>
		/// Returns an object that can be used to synchronize access to the list
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return base.InnerList.SyncRoot;
			}
		}
	}
}
