using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Provides a description, key, and port description's for all the ports used by a particular protocol.
	/// </summary>
	[System.Serializable()]
	public class PortMap: ISerializable, IDeserializationCallback
	{
		private string _key = null;
		private string _description = null;
		private PortDescriptorCollection _portDescriptors = null;

		public event PortDescriptorEventHandler PortDescriptorChanged;

		/// <summary>
		/// Initializes a new instance of the PortMap class
		/// </summary>
		public PortMap()
		{

		}

		/// <summary>
		/// Gets or sets the key for the PortMap
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
		/// Gets or sets the description for the PortMap
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
		/// Gets or sets the collection of ports for the PortMap
		/// </summary>
		public PortDescriptorCollection PortDescriptors
		{
			get
			{
				if (_portDescriptors == null)
				{
					_portDescriptors = new PortDescriptorCollection();
					_portDescriptors.Parent = this;
					_portDescriptors.PortDescriptorChanged += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
				}
				return _portDescriptors;
			}
			set
			{
				_portDescriptors = value;
			}
		}

		#region Protected Methods

		protected virtual void OnPortDescriptorChanged(object sender, PortDescriptorEventArgs e)
		{
			try
			{
				if (this.PortDescriptorChanged != null)
					this.PortDescriptorChanged(sender, e);
			}
			catch(Exception ex)
			{		
				Trace.WriteLine(ex);
			}
		}

		#endregion

		#region ISerializable Members

		public PortMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_key = info.GetString("Key");
			_description = info.GetString("Description");
			_portDescriptors = (PortDescriptorCollection)info.GetValue("PortDescriptors", typeof(PortDescriptorCollection));
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Key", _key);
			info.AddValue("Description", _description);
			info.AddValue("PortDescriptors", _portDescriptors, typeof(PortDescriptorCollection));
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			if (_portDescriptors != null)
			{
				_portDescriptors.Parent = this;
				_portDescriptors.PortDescriptorChanged += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
			}
		}

		#endregion
	}

	public class PortMapEventArgs : System.EventArgs 
	{
		private PortMap _portmap;

		public PortMapEventArgs(PortMap portmap) : base()
		{
			_portmap = portmap;
		}

		public PortMap PortMap
		{
			get
			{
				return _portmap;
			}
		}
	}

	public delegate void PortMapEventHandler(object sender, PortMapEventArgs e);
}
