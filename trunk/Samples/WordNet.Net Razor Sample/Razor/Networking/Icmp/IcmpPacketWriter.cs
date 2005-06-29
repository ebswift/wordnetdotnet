using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Summary description for IcmpPacketWriter.
	/// </summary>
	public class IcmpPacketWriter
	{
		/// <summary>
		/// Initializes a new instance of the IcmpPacketWriter class
		/// </summary>
		public IcmpPacketWriter()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Writes the IcmpPacket to the wire over the specified socket to the specified end point
		/// </summary>
		/// <param name="socket">The socket to write to</param>
		/// <param name="packet">The packet to write</param>
		/// <param name="ep">The end point to write to</param>
		/// <returns></returns>
		public virtual int Write(Socket socket, IcmpPacket packet, EndPoint ep)
		{
			/*
			 * check the parameters
			 * */

			if (socket == null)
				throw new ArgumentNullException("socket");

			if (socket == null)
				throw new ArgumentNullException("packet");

			if (socket == null)
				throw new ArgumentNullException("ep");

			// convert the packet to a byte array
			byte[] bytes = IcmpPacket.GetBytes(packet);

			// send the data using the specified socket, returning the number of bytes sent
			int bytesSent = socket.SendTo(bytes, bytes.Length, SocketFlags.None, ep);

			/*
			 * validate bytes sent
			 * */

			return bytesSent;
		}
	}
}
