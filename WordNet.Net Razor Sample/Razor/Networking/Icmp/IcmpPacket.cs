using System;
using System.Diagnostics;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Provides an means to describe an Icmp packet
	/// </summary>
	public class IcmpPacket
	{
		protected byte _type;
		protected byte _code;
		protected ushort _checksum;
		protected ushort _identifier;
		protected ushort _sequenceNumber;
		protected byte[] _payload;

		public const int DefaultPayloadLength = 32;

		/// <summary>
		/// Initializes a new instance of the IcmpEchoPacket class
		/// </summary>
		public IcmpPacket()
		{
			_payload = new byte[DefaultPayloadLength];
            
			// initialize the payload of the packet with some eroneous data
			for(int i = 0; i < DefaultPayloadLength; i++)
				_payload[i] = (byte)'#';
		}

		#region My Public Properties
		
		/// <summary>
		/// Gets or sets the Icmp message type
		/// </summary>
		public byte Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the Icmp code for the type specified
		/// </summary>
		public byte Code
		{
			get
			{
				return _code;
			}
			set
			{
				_code = value;
			}
		}

		/// <summary>
		/// Gets or sets the checksum for the packet
		/// </summary>
		public ushort Checksum
		{
			get
			{
				return _checksum;
			}
			set
			{
				_checksum = value;
			}
		}

		/// <summary>
		/// Gets or sets the identifier for the packet
		/// </summary>
		public ushort Identifier
		{
			get
			{
				return _identifier;
			}
			set
			{
				_identifier = value;
			}
		}

		/// <summary>
		/// Gets or sets the sequence number for the packet
		/// </summary>
		public ushort SequenceNumber
		{
			get
			{
				return _sequenceNumber;
			}
			set
			{
				_sequenceNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the binary payload for the packet
		/// </summary>
		public byte[] Payload
		{
			get
			{
				return _payload;
			}
			set
			{
				_payload = value;
			}
		}

		#endregion	

		/// <summary>
		/// Returns an IcmpPacket from an array of bytes
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static IcmpPacket FromBytes(byte[] bytes)
		{
			Debug.Assert(bytes != null);

			IcmpPacket packet = new IcmpPacket();
			
			packet.Type = bytes[0];
			packet.Code = bytes[1];
			packet.Checksum = BitConverter.ToUInt16(bytes, 2);
			packet.Identifier = BitConverter.ToUInt16(bytes, 4);
			packet.SequenceNumber = BitConverter.ToUInt16(bytes, 6);
			packet.Payload = new byte[bytes.Length - (bytes.Length - 8)];
			Array.Copy(bytes, 8, packet.Payload, bytes.Length - 8, packet.Payload.Length);
			
			return packet;
		}

		/// <summary>
		/// Returns an array of bytes from an IcmpPacket
		/// </summary>
		/// <returns></returns>
		public static byte[] GetBytes(IcmpPacket packet)
		{
			Debug.Assert(packet != null);

			int index = 0;
			byte[] bytes = new byte[packet.Payload.Length + 8];
			
			byte[] typeBytes = new byte[] {packet.Type};
			byte[] codeBytes = new byte[] {packet.Code};
			byte[] checksumBytes = BitConverter.GetBytes(packet.Checksum);
			byte[] idBytes = BitConverter.GetBytes(packet.Identifier);
			byte[] sequenceBytes = BitConverter.GetBytes(packet.SequenceNumber);

			// type
			Array.Copy(typeBytes, 0, bytes, index, typeBytes.Length);
			index += typeBytes.Length;

			// code
			Array.Copy(codeBytes, 0, bytes, index, codeBytes.Length);
			index += codeBytes.Length;

			// checksum
			Array.Copy(checksumBytes, 0, bytes, index, checksumBytes.Length);
			index += checksumBytes.Length;

			// identifier
			Array.Copy(idBytes, 0, bytes, index, idBytes.Length);
			index += idBytes.Length;

			// sequence number
			Array.Copy(sequenceBytes, 0, bytes, index, sequenceBytes.Length);
			index += sequenceBytes.Length;
			
			// payload
			Array.Copy(packet.Payload, 0, bytes, index, packet.Payload.Length);
			index += packet.Payload.Length;

			return bytes;
		}
		
		/// <summary>
		/// Creates a one's compliment checksum of the data in the byte array
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static ushort CreateChecksum(byte[] bytes)
		{
			// figure the high side of half of the size of the bytes
			int size = (int)Math.Ceiling((double)bytes.Length / 2d);
			
			// create an array to hold the checksum values
			ushort[] checksums = new ushort[size];

			// initialize the checksums
			int index = 0;
			for(int i = 0; i < size; i++)
			{				
				checksums[i] = BitConverter.ToUInt16(bytes, index);
				index += 2;
			}

			int checksum = 0;
			for(int i = 0; i < size; i++)
			{
				checksum += Convert.ToInt32(checksums[i]);			
			}

			checksum = (checksum >> 16) + (checksum & 0xFFFF);
			checksum += (checksum >> 16);
			return (ushort)(~checksum);
		}

	}
}
