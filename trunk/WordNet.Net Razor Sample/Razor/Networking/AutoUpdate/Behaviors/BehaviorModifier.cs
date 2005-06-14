using System;
using Razor.Networking.AutoUpdate;

namespace Razor.Networking.AutoUpdate.Behaviors
{
	/// <summary>
	/// Provides the basic definitions of methods that will be used to modify the behavior of an AutoUpdateManager
	/// </summary>
	public abstract class BehaviorModifier
	{				
		public abstract bool BindTo(AutoUpdateManager autoUpdateManager);
		public abstract bool Release(AutoUpdateManager autoUpdateManager);
	}
}
