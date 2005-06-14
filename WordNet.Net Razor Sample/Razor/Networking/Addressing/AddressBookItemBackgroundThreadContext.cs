using System;
using Razor.MultiThreading;
		  
namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Provides a means of combining an AddressBookItem with a BackgroundThread for the purposes of holding state information where contexts need determining.
	/// </summary>
	public class AddressBookItemBackgroundThreadContext
	{
		protected BackgroundThread _thread;
		protected AddressBookItem _item;

		/// <summary>
		/// Initializes a new instance of the AddressBookItemBackgroundThreadContext class
		/// </summary>
		/// <param name="thread"></param>
		/// <param name="item"></param>
		public AddressBookItemBackgroundThreadContext(BackgroundThread thread, AddressBookItem item)
		{
			_thread = thread;
			_item = item;
		}

		/// <summary>
		/// Returns the background thread around which this context is based
		/// </summary>
		public BackgroundThread Thread
		{
			get
			{
				return _thread;
			}
			set
			{
				_thread = value;
			}
		}

		/// <summary>
		/// Returns the address book item around which this context is based
		/// </summary>
		public AddressBookItem Item
		{
			get
			{
				return _item;
			}
			set
			{
				_item = value;
			}
		}
	}
}
