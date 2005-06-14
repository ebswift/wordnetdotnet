using System;
using Razor;

namespace BootStrapping
{
	/// <summary>
	/// This is the bootstrap for the Razor hosting engine.
	/// </summary>
	public class Startup
	{
		[STAThread]
		static void Main(string[] args) 
		{
			VersioningBootStrap bootStrap = new VersioningBootStrap();
			bootStrap.Run(args, System.Reflection.Assembly.GetExecutingAssembly());
		}	
	}
}