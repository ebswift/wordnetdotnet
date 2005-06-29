using System;

namespace Razor.Networking.PortMaps.WellknownPortMaps
{
	/// <summary>
	/// Summary description for ClassplusPortMap.
	/// </summary>
	[System.Serializable()]
	public class ClassplusPortMap : PortMap 
	{
		public enum ClassplusPortDescriptorKeys
		{
			ServerPort
		}

		public const string KEY = "{0a0caa61-2c62-488a-8c2d-4b8edef6733b}";

		public ClassplusPortMap() : base()
		{
			base.Key = KEY;
			base.Description = "ClassPLUS";
			base.PortDescriptors.Add(new PortDescriptor(ClassplusPortDescriptorKeys.ServerPort.ToString(), "Server Port", +2));
		}

		public ClassplusPortMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{

		}
	}
}
