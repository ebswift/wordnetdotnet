using System;

namespace Razor.Networking.PortMaps.WellknownPortMaps
{
	/// <summary>
	/// Summary description for AssistantPortMap.
	/// </summary>
	[System.Serializable()]
	public class AssistantPortMap: PortMap 
	{
		public enum AssistantPortDescriptorKeys
		{
			ServerPort
		}

		public const string KEY = "{9b9b265d-b519-4f52-89b8-485522ec8a1c}";

		public AssistantPortMap() : base()
		{
			base.Key = KEY;
			base.Description = "Assistant";
			base.PortDescriptors.Add(new PortDescriptor(AssistantPortDescriptorKeys.ServerPort.ToString(), "Server Port", +3));
		}

		public AssistantPortMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{

		}
	}
}
