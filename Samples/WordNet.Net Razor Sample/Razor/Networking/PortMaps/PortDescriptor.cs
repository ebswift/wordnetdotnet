using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Provides a desciption of a port number offset from a base port. This object can be keyed in a PortDescriptorCollection.
	/// </summary>
	[Serializable()]
	public class PortDescriptor: ISerializable
	{
		private string _key = null;
		private string _description = null;
		private int _offset = 0;
		private int _port = 0;
		private PortMap _parent;

		public event PortDescriptorEventHandler Changed;

		/// <summary>
		/// Initializes a new instance of the PortDescriptor class
		/// </summary>
		public PortDescriptor()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the PortDescriptor class
		/// </summary>
		/// <param name="key">The key this descriptor will be stored by</param>
		/// <param name="description">A description of this port</param>
		/// <param name="offset">The offset from the base port</param>
		public PortDescriptor(string key, string description, int offset)
		{
			_key = key;
			_description = description;
			_offset = offset;
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the key for this PortDescriptor
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		/// <summary>
		/// Gets or sets the description for this PortDescriptor
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Gets or sets the offset for this PortDescriptor from the Port Authority's base port
		/// </summary>
		public int Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
				this.OnChanged(this, new PortDescriptorEventArgs(this));
			}
		}

		/// <summary>
		/// Gets or sets the actual port number after calculations have been made
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}

		public PortMap Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		#endregion

		#region ISerializable Members

		public PortDescriptor(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_key = info.GetString("Key");
			_description = info.GetString("Description");
			_offset = info.GetInt32("Offset");
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Key", _key);
			info.AddValue("Description", _description);
			info.AddValue("Offset", _offset);
		}

		#endregion

		#region Protected Methods

		protected virtual void OnChanged(object sender, PortDescriptorEventArgs e)
		{
			try
			{
				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(Exception ex)
			{		
				Trace.WriteLine(ex);
			}
		}

		#endregion
	}

	public class PortDescriptorEventArgs : System.EventArgs 
	{
		private PortDescriptor _descriptor;

		public PortDescriptorEventArgs(PortDescriptor descriptor) : base()
		{
			_descriptor = descriptor;
		}

		public PortDescriptor Descriptor
		{
			get
			{
				return _descriptor;
			}
		}
	}

	public delegate void PortDescriptorEventHandler(object sender, PortDescriptorEventArgs e);
}
