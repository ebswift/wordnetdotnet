using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpChunkedBody.
	/// </summary>
	internal class HttpChunkedBody
	{
		protected HttpChunkList _chunks;
		protected HttpHeaderList _trailer;

		/// <summary>
		/// Initializes a new instance of the HttpChunkedBody class
		/// </summary>
		public HttpChunkedBody()
		{
			_chunks = new HttpChunkList();
			_trailer = new HttpHeaderList();
		}

		/// <summary>
		/// Returns the list of chunks contained in this entity body
		/// </summary>
		public HttpChunkList Chunks
		{
			get
			{
				return _chunks;
			}
		}

		/// <summary>
		/// Returns the list of trailer headers contained in this entity body
		/// </summary>
		public HttpHeaderList TrailerHeaders
		{
			get
			{
				return _trailer;
			}
		}

		private int GetTotalChunkDataSize()
		{
			int size = 0;
			foreach(HttpChunk chunk in _chunks)
				size += chunk.Size;
			return size;
		}

		/// <summary>
		/// Returns a byte array representing this chunked entity body
		/// </summary>
		/// <returns></returns>
		public virtual byte[] ToByteArray()
		{
			byte[] buffer = new byte[this.GetTotalChunkDataSize()];

			int offset = 0;
			foreach(HttpChunk chunk in _chunks)
			{
				Buffer.BlockCopy(chunk.Data, 0, buffer, offset, chunk.Size);
				offset += chunk.Size;
			}

			return buffer;
		}
	}
}
