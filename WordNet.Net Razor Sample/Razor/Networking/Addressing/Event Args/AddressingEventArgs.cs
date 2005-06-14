using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressingEventArgs.
	/// </summary>
	public class AddressingEventArgs : EventArgs
	{
		protected object _context;
		protected AddressingActions _action;
		
		/// <summary>
		/// Initializes a new instance of the X class
		/// </summary>
		/// <param name="addressBook">The context of the event</param>
		/// <param name="action">The action taken on the context</param>
		public AddressingEventArgs(object context, AddressingActions action) : base()
		{
			_context = context;
			_action = action;
		}
		
		/// <summary>
		/// Returns the context of the event
		/// </summary>
		public object Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Returns the action taken on the context
		/// </summary>
		public AddressingActions Action
		{
			get
			{
				return _action;
			}
		}
	}

	/// <summary>
	/// Defines an event delegate for the AddressingEventArgs class
	/// </summary>
	public delegate void AddressingEventHandler(object sender, AddressingEventArgs e);
}
