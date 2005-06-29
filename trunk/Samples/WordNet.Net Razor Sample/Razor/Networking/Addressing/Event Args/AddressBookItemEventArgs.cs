using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Defines an event args class for the AddressBookItem class
	/// </summary>
	public class AddressBookItemEventArgs : AddressingEventArgs
	{				
		/// <summary>
		/// Initializes a new instance of the AddressBookItemEventArgs class
		/// </summary>
		/// <param name="addressBook">The context of the event</param>
		/// <param name="action">The action taken on the context</param>
		public AddressBookItemEventArgs(AddressBookItem item, AddressingActions action) : base(item, action)
		{

		}

		/// <summary>
		/// Returns the context for the event
		/// </summary>
		public new AddressBookItem Context
		{
			get
			{				
				return _context as AddressBookItem;
			}
		}
	}

	/// <summary>
	/// Defines a delegate for the AddressBookItemEventArgs class
	/// </summary>
	public delegate void AddressBookItemEventHandler(object sender, AddressBookItemEventArgs e);
}
