using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectoryEventArgs.
	/// </summary>
	public class AddressBookDirectoryEventArgs : AddressingEventArgs 
	{
		public AddressBookDirectoryEventArgs(AddressBookDirectory directory, AddressingActions action) : base(directory, action)
		{
			
		}

		/// <summary>
		/// Returns the context of the event
		/// </summary>
		public new AddressBookDirectory Context
		{
			get
			{
				return _context as AddressBookDirectory;
			}
		}
	}
}
