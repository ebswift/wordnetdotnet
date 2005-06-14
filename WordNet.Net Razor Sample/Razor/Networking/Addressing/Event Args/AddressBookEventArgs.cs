using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookEventArgs.
	/// </summary>
	public class AddressBookEventArgs : AddressingEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the AddressBookEventArgs class
		/// </summary>
		/// <param name="addressBook">The context of the event</param>
		/// <param name="action">The action taken on the context</param>
		public AddressBookEventArgs(AddressBook addressBook, AddressingActions action) : base(addressBook, action)
		{
			
		}		

		/// <summary>
		/// Returns the context of the event
		/// </summary>
		public new AddressBook Context
		{
			get
			{
				return _context as AddressBook;
			}
		}
	}

	/// <summary>
	/// Defines an event delegate for the AddressBookEventArgs class
	/// </summary>
	public delegate void AddressBookEventHandler(object sender, AddressBookEventArgs e);
}
