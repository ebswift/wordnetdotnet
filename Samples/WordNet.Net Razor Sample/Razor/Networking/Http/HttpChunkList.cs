using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpChunkList.
	/// </summary>
	internal class HttpChunkList : CollectionBase
	{
		/// <summary>
		/// Initializes a new 
		/// </summary>
		public HttpChunkList()
		{
			
		}

		/// <summary>
		/// Adds a chunk to the list
		/// </summary>
		/// <param name="chunk"></param>
		public void Add(HttpChunk chunk)
		{
			base.InnerList.Add(chunk);
		}

		/// <summary>
		/// Adds an array of chunks to the list
		/// </summary>
		/// <param name="chunks"></param>
		public void AddRange(HttpChunk[] chunks)
		{
			foreach(HttpChunk chunk in chunks)
				this.Add(chunk);
		}

		/// <summary>
		/// Removes a chunk from the list
		/// </summary>
		/// <param name="chunk"></param>
		public void Remove(HttpChunk chunk)
		{

		}		

		/// <summary>
		/// Returns a chunk from the specified index in the list
		/// </summary>
		public HttpChunk this[int index]
		{
			get
			{
				return (HttpChunk)base.InnerList[index];
			}
		}

		/// <summary>
		/// Returns a byte array representation of the chunk list
		/// </summary>
		/// <returns></returns>
		public virtual byte[] ToByteArray()
		{
			byte[] buffer = null;

			// create a stream
			using (MemoryStream stream = new MemoryStream())
			{
				// create a writer
				using (BinaryWriter writer = new BinaryWriter(stream, HttpUtils.Encoding))
				{
					// write each chunk to the stream
					foreach(HttpChunk chunk in base.InnerList)
						writer.Write(chunk.ToByteArray());

					// retrieve the buffer from the stream
					buffer = stream.GetBuffer();

					// close the writer and the underlying stream
					writer.Close();
				}
			}

			return buffer;
		}

		/// <summary>
		/// Returns a string repsentation of the chunk list
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			// append each chunk
			foreach(HttpChunk chunk in base.InnerList)
				sb.Append(chunk.ToString());
			
			return sb.ToString();
		}

	}
}
