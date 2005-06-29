using System;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// EventArgs class for the PortEventHandler delegate.
	/// </summary>
	public class PortEventArgs: System.EventArgs 
	{
		private int _port = 0;

		/// <summary>
		/// Initializes a new instance of the PortEventArgs class
		/// </summary>
		/// <param name="port">A port number</param>
		public PortEventArgs(int port)
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Gets the port number 
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
		}
	}

	/// <summary>
	/// Delegate for the PortEventArgs class
	/// </summary>
	public delegate void PortEventHandler(object sender, PortEventArgs e);
}
