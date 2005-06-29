using System;
using System.Collections;
using System.Diagnostics;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpConnectionList.
	/// </summary>
	public class HttpConnectionList : CollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the HttpConnectionList class
		/// </summary>
		public HttpConnectionList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Returns a thread safe sychronized array list 
		/// </summary>
		public new ArrayList InnerList
		{
			get
			{
				return ArrayList.Synchronized(base.InnerList);
			}
		}

		/// <summary>
		/// Adds the connection to the list
		/// </summary>
		/// <param name="connection">The connection to add</param>
		/// <returns></returns>
		public bool Add(HttpConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection", "A null connection cannot be added to the list.");

			if (this.Contains(connection.Id))
				throw new Exception(string.Format("A connection already exists in the list with an id of {0}", connection.Id.ToString()));

			base.InnerList.Add(connection);

			return true;
		}

		/// <summary>
		/// Removes the connection from the list
		/// </summary>
		/// <param name="connection">The connection to remove</param>
		/// <returns></returns>
		public bool Remove(HttpConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection", "A null connection cannot be removed from the list.");

			if (this.Contains(connection.Id))
				base.InnerList.Remove(connection);

			return true;
		}

		/// <summary>
		/// Determines if the list contains a connection with the specified id
		/// </summary>
		/// <param name="id">The id of the connection to check</param>
		/// <returns></returns>
		public bool Contains(Guid id)
		{
			foreach(HttpConnection connection in base.InnerList)
				if (Guid.Equals(connection.Id, id))
					return true;
			return false;
		}

		/// <summary>
		/// Returns the connection at the specified index
		/// </summary>
		public HttpConnection this[int index]
		{
			get
			{
				return base.InnerList[index] as HttpConnection;
			}
		}

		/// <summary>
		/// Returns the connection with the specified id
		/// </summary>
		public HttpConnection this[Guid id]
		{
			get
			{
				foreach(HttpConnection connection in base.InnerList)
					if (Guid.Equals(connection.Id, id))
						return connection;
				return null;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the object
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return base.InnerList.SyncRoot;
			}
		}
	}
}
