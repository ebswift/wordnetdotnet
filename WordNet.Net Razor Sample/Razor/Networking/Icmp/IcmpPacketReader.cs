using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Summary description for IcmpPacketReader.
	/// </summary>
	public class IcmpPacketReader
	{
		/// <summary>
		/// Initializes a new instance of the IcmpPacketReader class
		/// </summary>
		public IcmpPacketReader()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Reads an IcmpPacket from the wire using the specified socket from the specified end point
		/// </summary>
		/// <param name="socket">The socket to read</param>
		/// <param name="packet">The packet read</param>
		/// <param name="ep">The end point read from</param>
		/// <returns></returns>
		public virtual bool Read(Socket socket, EndPoint ep, int timeout, out IcmpPacket packet, out int bytesReceived)
		{
			const int MAX_PATH = 256;
			packet = null;
			bytesReceived = 0;

			/*
			 * check the parameters
			 * */

			if (socket == null)
				throw new ArgumentNullException("socket");
			
			if (socket == null)
				throw new ArgumentNullException("ep");		
						
			// see if any data is readable on the socket
			bool success = socket.Poll(timeout * 1000, SelectMode.SelectRead);

			// if there is data waiting to be read
			if (success)
			{
				// prepare to receive data
				byte[] bytes = new byte[MAX_PATH];
				
				bytesReceived = socket.ReceiveFrom(bytes, bytes.Length, SocketFlags.None, ref ep);

				/*
				 * convert the bytes to an icmp packet
				 * */
//				packet = IcmpPacket.FromBytes(bytes);
			}						

			return success;
		}
	}
}
