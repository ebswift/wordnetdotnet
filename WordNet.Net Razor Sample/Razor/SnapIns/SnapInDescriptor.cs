/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;
using Razor.Attributes;

namespace Razor.SnapIns
{
	/// <summary>
	/// Contains methods and properties that describe the state and condition of a SnapIn, it's Type, and it's MetaData.
	/// </summary>
	public class SnapInDescriptor
	{
		private readonly ISnapIn _snapIn;
		private readonly Type _type;		
		private readonly Type[]	_dependencies;
		internal bool _isMissingDependency;
		internal bool _isCircularlyDependent;
		internal bool _isDependentOnTypeThatIsCircularlyDependent;
		internal bool _isDependentOnTypeThatIsMissingDependency;
		internal bool _isStarted;
		internal bool _isUninstalled;
		internal DateTime _installDate;
		internal int _runCount;
		private SnapInMetaData _metaData;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the SnapInDescriptor class
		/// </summary>
		/// <param name="type">The Type from which the ISnapIn instance was created</param>
		/// <param name="snapIn">The instance of the class that implemented the ISnapIn interface</param>
		public SnapInDescriptor(Type type, ISnapIn snapIn)
		{
			_type = type;
			_snapIn = snapIn;
			_dependencies = this.ExtractTypesThatThisTypeDependsOn(type);	
			_metaData = new SnapInMetaData(type);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines if this descriptor depends upon the Type described by the specified descriptor
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public bool DependsOn(SnapInDescriptor descriptor)
		{
//			string info = string.Format("Determining if Type '{0}' depends on Type '{1}'", _type.Name, descriptor.Type.Name);
//			System.Diagnostics.Trace.WriteLine(info);

			foreach(Type t in _dependencies)
				if (Type.Equals(t, descriptor.Type))
					return true;
			return false;
		}

		/// <summary>
		/// Allows the descriptor to determine if any of it's dependencies are missing, whether it be the assembly that is missing, or just the Type not being listed as another SnapIn
		/// </summary>
		/// <param name="snapInDescriptors">The descriptors to check against</param>
		public void DetermineIfAnyDependenciesAreMissing(SnapInDescriptor[] descriptors)
		{
			_isMissingDependency = this.DetermineIfAnyDependenciesAreMissing(_dependencies, descriptors);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the Type from which the depencies where read
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}
		
		/// <summary>
		/// Gets the instance of the class implementing the ISnapIn interface
		/// </summary>
		public ISnapIn SnapIn
		{
			get
			{
				return _snapIn;
			}
		}

		/// <summary>
		/// Gets an array of Types upon which the specified Type depends
		/// </summary>
		public Type[] Dependencies
		{
			get
			{
				return _dependencies;
			}
		}

		/// <summary>
		/// Determines if any Type that this Type depends on is missing.
		/// </summary>
		public bool IsMissingDependency
		{
			get
			{
				return _isMissingDependency;
			}
		}

		/// <summary>
		/// Gets or sets whether this descriptor is circularly dependent upon another descriptor (aka contains a dependency to another type, that in turn depends on this descriptor's type)
		/// </summary>
		public bool IsCircularlyDependent
		{
			get
			{
				return _isCircularlyDependent;
			}
		}

		/// <summary>
		/// Gets or sets whether this descriptor is dependent on a Type that is itself circularly dependent on something else
		/// </summary>
		public bool IsDependentOnTypeThatIsCircularlyDependent
		{
			get
			{
				return _isDependentOnTypeThatIsCircularlyDependent;
			}
		}

		/// <summary>
		/// Gets or sets whether this descriptor is dependent on a Type that is itself missing a dependency
		/// </summary>
		public bool IsDependentOnTypeThatIsMissingDependency
		{
			get
			{
				return _isDependentOnTypeThatIsMissingDependency;
			}
		}

		/// <summary>
		/// Gets whether the SnapIn is started
		/// </summary>
		public bool IsStarted
		{
			get
			{
				return _isStarted;
			}
		}

		/// <summary>
		/// Gets whether the SnapIn is uninstalled
		/// </summary>
		public bool IsUninstalled
		{
			get
			{
				return _isUninstalled;
			}
		}

		/// <summary>
		/// Gets the Date and Time that the SnapIn was installed
		/// </summary>
		public DateTime InstallDate
		{
			get
			{
				return _installDate;
			}
		}

		/// <summary>
		/// Gets the run count for the SnapIn
		/// </summary>
		public int RunCount
		{
			get
			{
				return _runCount;
			}
		}

		/// <summary>
		/// Exposes the information exracted from attributes in the SnapIn's meta data
		/// </summary>
		public SnapInMetaData MetaData
		{
			get
			{
				return _metaData;
			}
		}

		public static void AdviseOnActionsThatCanBeTaken(
			SnapInDescriptor descriptor, 
			out bool canStart, 
			out bool canStop, 
			out bool canReinstall, 
			out bool canUninstall)
		{			
			// if there is a snapin selected
			if (descriptor != null)
			{			
				if (descriptor.IsUninstalled)
				{
					canStart = false;
					canStop  = false;
					canReinstall = true;
					canUninstall = false;
				}
				else
				{
					canStart = !descriptor.IsStarted;
					canStop  = descriptor.IsStarted;
					canReinstall = false;
					canUninstall = true;
				}
			}
			else
			{
				// but you can't do anything without a snapin selected
				canStart = false;
				canStop = false;
				canReinstall = false;
				canUninstall = false;
			}		
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Extracts the Types that the specified as dependencies via attributes on the specified Type.
		/// </summary>
		/// <param name="type">The Type that contains the attributes to read</param>
		/// <returns></returns>
		private Type[] ExtractTypesThatThisTypeDependsOn(Type type)
		{
			try
			{
				if (type != null)
				{
					object[] attributes = type.GetCustomAttributes(typeof(SnapInDependencyAttribute), true);
					if (attributes != null)
					{
						Type[] dependencies = new Type[attributes.Length];
						for(int i = 0; i < attributes.Length; i++)
						{
							SnapInDependencyAttribute da = (SnapInDependencyAttribute)attributes[i];
							dependencies[i] = this.GetTypeFromAttribute(da);
						}
						return dependencies;
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new Type[] {};
		}	
	
		/// <summary>
		/// Returns the Type from the attribute. 
		/// </summary>
		/// <param name="attribute">The attribute to read</param>
		/// <returns></returns>
		private Type GetTypeFromAttribute(SnapInDependencyAttribute attribute)
		{
			try
			{
				return attribute.Type;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);				
			}
			return (Type)Type.Missing;
		}

		/// <summary>
		/// Determines if any depdencies are missing
		/// </summary>
		/// <param name="dependencies">The array of Types that are the dependencies</param>
		/// <param name="snapIns">The list of SnapIns to confirm against</param>
		/// <returns></returns>
		private bool DetermineIfAnyDependenciesAreMissing(Type[] dependencies, SnapInDescriptor[] descriptors)
		{
			// if any type is missing, that means that it's assembly wasn't loaded, and therefore the type
			// cannot be created. 
			foreach(Type type in dependencies)
			{
				if ((object)type == Type.Missing)
					return true;
			}
			
			// now that we know no types are missing, they are all loaded, we need to make sure that all of the 
			// classes that were snapins are loaded, if one of our dependencies isn't in the list of snapins
			// we can't load because a dependency is still missing
			foreach(Type type in dependencies)
			{
				bool found = false;
				foreach(SnapInDescriptor descriptor in descriptors)
				{
					if (!descriptor.IsUninstalled)
					{
						if (descriptor.Type == type)
						{
							found = true;
							break;
						}
					}
				}
				
				if (!found)
					return true;
			}

			return false;
		}
		
		

		#endregion

		#region Static Methods	

		/// <summary>
		/// Sorts an array of SnapInDescriptors from least dependent first to most dependent last, or vice versa
		/// </summary>
		/// <param name="descriptors">The array of descriptors to sort</param>
		/// <param name="leastDependentFirst">The order in which to sort them</param>
		/// <returns></returns>
		public static bool Sort(SnapInDescriptor[] descriptors, bool leastDependentFirst)
		{	
			try
			{
				// front to back - 1 
				for(int i = 0; i < descriptors.Length - 1; i++)
				{
					// front + 1 to back
					for(int j = i + 1; j < descriptors.Length; j++)
					{				
						bool dependsOn = descriptors[i].DependsOn(descriptors[j]);
						if ((leastDependentFirst ? dependsOn : !dependsOn))
						{											 
							// swap i with j, where i=1 and j=2
							SnapInDescriptor descriptor = descriptors[j];
							descriptors[j] = descriptors[i];
							descriptors[i] = descriptor;
						}													
					}
				}
//				foreach(SnapInDescriptor descriptor in descriptors)
//					System.Diagnostics.Trace.WriteLine(descriptor.Type.Name);
				return true;
			}
			catch(System.Exception systemException)
			{	
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Determines if one descriptor is a direct dependency of another descriptor 
		/// </summary>
		/// <param name="descriptorA">The Type to look for in B's dependencies</param>
		/// <param name="descriptorB">The descriptor in whose dependencies to look for A</param>
		/// <returns></returns>
		public static bool IsDirectDependencyOf(SnapInDescriptor descriptorA, SnapInDescriptor descriptorB)
		{
			foreach(Type type in descriptorB.Dependencies)
				if (descriptorA.Type == type)
					return true;
			return false;
		}
		
		/// <summary>
		/// Flags all descriptors that are missing dependencies
		/// </summary>
		/// <param name="descriptors"></param>
		public static void MarkDescriptorsThatAreMissingDependencies(SnapInDescriptor[] descriptors)
		{
			// allow each descriptor to determine if any dependencies are missing
			foreach(SnapInDescriptor descriptor in descriptors)
				descriptor.DetermineIfAnyDependenciesAreMissing(descriptors);
		}

		/// <summary>
		/// Flags all descriptors that are circularly dependent
		/// </summary>
		/// <param name="descriptors"></param>
		public static void MarkDescriptorsThatAreCircularlyDependent(SnapInDescriptor[] descriptors)
		{
			// check each descriptor's dependencies
			foreach(SnapInDescriptor descriptor in descriptors)
			{
				// check each dependency in that descriptor
				foreach(Type a in descriptor.Dependencies)
				{
					// against all the other descriptors
					foreach(SnapInDescriptor otherDescriptor in descriptors)
					{
						// if the descriptor describes the type that is the dependency of the descriptor in question
						if (otherDescriptor.Type == a)
						{
							// that the other descriptor does not contain a direct dependency of the first descriptor
							foreach(Type b in otherDescriptor.Dependencies)
							{
								// if the two descriptors, have a dependency that point to one another, then it is considered circularly dependent
								if (Type.Equals(descriptor.Type, b))
								{
									// mark the descriptor as circularly dependent, and stop looking at this particular other descriptor
									descriptor._isCircularlyDependent = true;
									break;
								}
							}
							
							// if the descriptor is circularly dependent, then we can stop comparing it to the other descriptors
							if (descriptor.IsCircularlyDependent)
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Flags all descriptors that have dependencies that are circularly dependent
		/// </summary>
		/// <param name="descriptors"></param>
		public static void MarkDescriptorsThatHaveDependenciesThatAreCircularlyDependent(SnapInDescriptor[] descriptors)
		{
			foreach(SnapInDescriptor descriptor in descriptors)
			{
				foreach(Type a in descriptor.Dependencies)
				{
					foreach(SnapInDescriptor otherDescriptor in descriptors)
					{
						if (otherDescriptor.Type == a)
						{
							if (otherDescriptor.IsCircularlyDependent)
							{
								descriptor._isDependentOnTypeThatIsCircularlyDependent = true;								
							}

							// stop looking at the other descriptors
							if (descriptor.IsDependentOnTypeThatIsCircularlyDependent)
								break;
						}
					}

					// stop looking at the dependencies, it's already done
					if (descriptor.IsDependentOnTypeThatIsCircularlyDependent)
						break;
				}
			}
		}

		/// <summary>
		/// Flags all descriptors that have dependencies that are missing dependencies
		/// </summary>
		/// <param name="descriptors"></param>
		public static void MarkDescriptorsThatHaveDependenciesThatAreMissingADependency(SnapInDescriptor[] descriptors)
		{
			foreach(SnapInDescriptor descriptor in descriptors)
			{
				foreach(Type a in descriptor.Dependencies)
				{
					foreach(SnapInDescriptor otherDescriptor in descriptors)
					{
						if (otherDescriptor.Type == a)
						{
							if (otherDescriptor.IsMissingDependency)
							{
								descriptor._isDependentOnTypeThatIsMissingDependency = true;							
							}

							// stop looking at the other descriptors
							if (descriptor.IsDependentOnTypeThatIsMissingDependency)
								break;
						}
					}

					// stop looking at the dependencies, it's already done
					if (descriptor.IsDependentOnTypeThatIsMissingDependency)
						break;
				}
			}
		}

		#endregion
	}
}
