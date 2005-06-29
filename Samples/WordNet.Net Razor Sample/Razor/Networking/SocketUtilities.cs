using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Razor.Networking
{
	/// <summary>
	/// Utility functions for common methods performed with sockets
	/// </summary>
	public class SocketUtilities
	{				
		/// <summary>
		/// Reads an int from the specified socket
		/// </summary>
		/// <exception cref="ConnectionClosedByPeerException">Thrown when the connection is closed by the remote peer</exception>
		/// <param name="socket"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ReceiveInt32(Socket socket, out int value)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");

			value = 0;
			int result = 0;
			byte[] buffer = new byte[4];

			result = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);			
			value = BitConverter.ToInt32(buffer, 0);
			
			if (result == 0)
				throw new ConnectionClosedByPeerException();
			
			return result;
		}

		/// <summary>
		/// Writes an int to the specified socket
		/// </summary>
		/// <exception cref="ConnectionClosedByPeerException">Thrown when the connection is closed by the remote peer</exception>
		/// <param name="socket"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int SendInt32(Socket socket, int value)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");

			int result = 0;			
			byte[] buffer = System.BitConverter.GetBytes(value);
			result = socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
			
			if (result == 0)
				throw new ConnectionClosedByPeerException();

			return result;
		}
		
		/// <summary>
		/// Reads data into the byte array using the specified socket up to the array's length
		/// </summary>
		/// <exception cref="ConnectionClosedByPeerException">Thrown when the connection is closed by the remote peer</exception>
		/// <param name="socket"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static int ReceiveBytes(Socket socket, byte[] bytes)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");

			int result = 0;
			result = socket.Receive(bytes, 0, bytes.Length, SocketFlags.None);

			if (result == 0)
				throw new ConnectionClosedByPeerException();

			return result;
		}

		/// <summary>
		/// Writes the bytes to the specified socket
		/// </summary>
		/// <exception cref="ConnectionClosedByPeerException">Thrown when the connection is closed by the remote peer</exception>
		/// <param name="socket"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static int SendBytes(Socket socket, byte[] bytes)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");

			int result = 0;
			result = socket.Send(bytes, 0, bytes.Length, SocketFlags.None);

			if (result == 0)
				throw new ConnectionClosedByPeerException();

			return result;
		}
	}
}
