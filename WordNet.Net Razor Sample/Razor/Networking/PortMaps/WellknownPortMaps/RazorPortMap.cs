using System;

namespace Razor.Networking.PortMaps.WellknownPortMaps
{
	/// <summary>
	/// Summary description for RazorPortMap.
	/// </summary>
	[System.Serializable()]
	public class RazorPortMap : PortMap 
	{
		public enum RazorPortDescriptorKeys
		{
			ServerPort
		}

		public const string KEY = "{346949b3-aa26-4bbe-9a5e-6f066dea5e50}";

		public RazorPortMap() : base()
		{
			base.Key = KEY;
			base.Description = "Razor";
			base.PortDescriptors.Add(new PortDescriptor(RazorPortDescriptorKeys.ServerPort.ToString(), "Server Port", +1));
		}

		public RazorPortMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{

		}
	}
}
