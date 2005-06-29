using System;
using System.Net;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for NetworkOptionNames.
	/// </summary>
	public enum NetworkOptionNames
	{
		Id,
		BasePort		
	}

	/// <summary>
	/// Summary description for NetworkOptions.
	/// </summary>
	public class NetworkOptions
	{
		protected Guid _id;
		protected int _basePort;

		/// <summary>
		/// Initializes a new instance of the NetworkOptions class
		/// </summary>
		public NetworkOptions()
		{
			
		}

		/// <summary>
		/// Gets or sets the globally unique identifier that applies to this application as an end point
		/// </summary>
		public Guid Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets or sets the base port number that will be combined with a port map to determine a specific application's server port
		/// </summary>
		public int BasePort
		{
			get
			{
				return _basePort;
			}
			set
			{
				_basePort = value;
			}
		}
	}

	/// <summary>
	/// Summary description for DefaultNetworkOptions.
	/// </summary>
	public class DefaultNetworkOptions : NetworkOptions
	{
		public const int BASEPORT = 12300;

		/// <summary>
		/// Initializes a new instance of the DefaultNetworkOptions class
		/// </summary>
		public DefaultNetworkOptions()
		{			
			base.Id = Guid.NewGuid();			
			base.BasePort = 12300; 

			/*
			 * the base port number is unassigned as of 9/28/2004
			 * refer to the url http://www.iana.org/assignments/port-numbers
			 * to apply for port registration go here http://www.iana.org/cgi-bin/usr-port-number.pl
			 * */
		}
	}
}
