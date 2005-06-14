using System;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Provides a means of filtering or cancelling events which involve name changes.
	/// </summary>
	public class NameChangeEventArgs : EventArgs
	{
		protected string _nameBefore;
		protected string _nameAfter;
		protected bool _cancel;

		/// <summary>
		/// Initializes a new instance of the N class
		/// </summary>
		/// <param name="nameBefore">The name as it is currently</param>
		/// <param name="nameAfter">The name as it will be after the desired change</param>
		public NameChangeEventArgs(string nameBefore, string nameAfter) : base()
		{
			_nameBefore = nameBefore;
			_nameAfter = nameAfter;			
		}

		/// <summary>
		/// Returns the name as it currently appears before the change is applied
		/// </summary>
		public string NameBefore
		{
			get
			{
				return _nameBefore;
			}
		}

		/// <summary>
		/// Returns the name as it would appear after the change is applied
		/// </summary>
		public string NameAfter
		{
			get
			{
				return _nameAfter;
			}
		}
		
		/// <summary>
		/// Gets or sets a flag that indicates whether the event should be cancelled
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}
	}
	
	/// <summary>
	/// Defines an event delegate for the NameChangeEventArgs class
	/// </summary>
	public delegate void NameChangeEventHandler(object sender, NameChangeEventArgs e);
}
