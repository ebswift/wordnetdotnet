using System;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Provides a means to describe an icmp echo packet
	/// </summary>
	public class IcmpEchoPacket : IcmpPacket
	{
		#region My Constants

		public const int DefaultType = 8;
		public const int DefaultSubCode = 0;
		public const int DefaultChecksum = 0;
		public const int DefaultIdentifier = 45;
		public const int DefaultSequenceNumber = 0;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of the IcmpEchoPacket class
		/// </summary>
		public IcmpEchoPacket() : base()
		{
			// set the basic icmp properties to create an icmp echo packet
			_type				= DefaultType;
			_code				= DefaultSubCode;
			_checksum			= DefaultChecksum;
			_identifier			= DefaultIdentifier;
			_sequenceNumber		= DefaultSequenceNumber;			
		}
	}
}
