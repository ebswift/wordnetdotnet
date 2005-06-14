/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Razor.Configuration
{
	/// <summary>
	/// An attribute applied to the Security Access Rights values that determines if the right is displayed in the Windows UI for Permisions
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
	public class WindowsUIPermission : System.Attribute
	{		
		[Flags()]
		public enum AppliesTo
		{
			Files = 1,
			Folders = 2,
			Pipes = 4,
			Any = 8,
			All = Files | Folders | Pipes
		}
		
		private AppliesTo _appliesTo;

		public WindowsUIPermission(AppliesTo appliesTo)
		{
			_appliesTo = appliesTo;
		}

		public AppliesTo AppliesToThese
		{
			get
			{
				return _appliesTo;
			}
		}
	}

	// This enumeration contains a list of standard access rights. It's
	// by no means complete because it lacks the specific access rights
	// (which don't appear to be documented).
	[Flags()]
	public enum SecurityAccessRights : uint
	{
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
		[Description("Delete")]
		DELETE                    = 0x00010000,
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
		[Description("Read Permissions")]
		READ_CONTROL              = 0x00020000,
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
		[Description("Change Permissions")]
		WRITE_DAC                 = 0x00040000,
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Any)]
		[Description("Take Ownership")]
		WRITE_OWNER               = 0x00080000,

		SYNCHRONIZE               = 0x00100000,
		STANDARD_RIGHTS_REQUIRED  = 0x000F0000,
		STANDARD_RIGHTS_READ      = READ_CONTROL,
		STANDARD_RIGHTS_WRITE     = READ_CONTROL,
		STANDARD_RIGHTS_EXECUTE   = READ_CONTROL,
		STANDARD_RIGHTS_ALL       = 0x001F0000,
		SPECIFIC_RIGHTS_ALL       = 0x0000FFFF,
		ACCESS_SYSTEM_SECURITY    = 0x01000000,
		MAXIMUM_ALLOWED           = 0x02000000,
		GENERIC_READ              = 0x80000000,
		GENERIC_WRITE             = 0x40000000,
		GENERIC_EXECUTE           = 0x20000000,
		GENERIC_ALL               = 0x10000000,

		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Pipes)]
		[Description("Read Data")]
		FILE_READ_DATA            = 0x0001,    // file & pipe		
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
		[Description("List Folder")]
		FILE_LIST_DIRECTORY       = 0x0001,    // directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Pipes)]
		[Description("Write Data")]
		FILE_WRITE_DATA           = 0x0002,    // file & pipe
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
		[Description("Create Files")]
		FILE_ADD_FILE             = 0x0002,    // directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files)]
		[Description("Append Data")]
		FILE_APPEND_DATA          = 0x0004,    // file
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
		[Description("Create Folders")]
		FILE_ADD_SUBDIRECTORY     = 0x0004,    // directory
		
		FILE_CREATE_PIPE_INSTANCE = 0x0004,    // named pipe
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Folders)]
		[Description("Read Extended Attributes")]
		FILE_READ_EA              = 0x0008,    // file & directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files | WindowsUIPermission.AppliesTo.Folders)]
		[Description("Write Extended Attributes")]
		FILE_WRITE_EA             = 0x0010,    // file & directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Files)]
		[Description("Execute File")]	
		FILE_EXECUTE              = 0x0020,    // file
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
		[Description("Traverse Folder")]
		FILE_TRAVERSE             = 0x0020,    // directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.Folders)]
		[Description("Delete Subfolders and File permissions")]
		FILE_DELETE_CHILD         = 0x0040,    // directory
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.All)]
		[Description("Read Attributes")]
		FILE_READ_ATTRIBUTES      = 0x0080,    // all
		
		[WindowsUIPermission(WindowsUIPermission.AppliesTo.All)]
		[Description("Write Attributes")]
		FILE_WRITE_ATTRIBUTES     = 0x0100,    // all
		
		[Description("All Access")]
		FILE_ALL_ACCESS			  = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF,
		
		[Description("Read")]
		FILE_GENERIC_READ         = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE,

		[Description("Write")]
		FILE_GENERIC_WRITE        = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE,

		[Description("Execute")]
		FILE_GENERIC_EXECUTE      = STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | SYNCHRONIZE
	}

	public enum SE_OBJECT_TYPE
	{
		SE_UNKNOWN_OBJECT_TYPE = 0,
		SE_FILE_OBJECT,
		SE_SERVICE,
		SE_PRINTER,
		SE_REGISTRY_KEY,
		SE_LMSHARE,
		SE_KERNEL_OBJECT,
		SE_WINDOW_OBJECT,
		SE_DS_OBJECT,
		SE_DS_OBJECT_ALL,
		SE_PROVIDER_DEFINED_OBJECT,
		SE_WMIGUID_OBJECT,
		SE_REGISTRY_WOW64_32KEY
	}

	/// <summary>
	/// The SecurityInformation type identifies the object-related security information being set or queried. This security information includes:
	/// <list type="">
	/// <item>The owner of an object</item>
	/// <item>The primary group of an object</item>
	/// <item>The discretionary access control list (DACL) of an object</item>
	/// <item>The system access control list (SACL) of an object</item>
	/// </list>
	/// </summary>
	public enum SecurityInformation : uint
	{
		Owner                = 0x00000001,
		Group                = 0x00000002,
		DACL                 = 0x00000004,
		SACL                 = 0x00000008,
		ProtectedDACL       = 0x80000000,
		ProtectedSACL       = 0x40000000,
		UnprotectedDACL     = 0x20000000,
		UnprotectedSACL     = 0x10000000
	};

	// This enumeration contains the type of SID returned by the
	// LookupAccountName() function.
	public enum SID_NAME_USE 
	{
		SidTypeUser = 1,
		SidTypeGroup,
		SidTypeDomain,
		SidTypeAlias,
		SidTypeWellKnownGroup,
		SidTypeDeletedAccount,
		SidTypeInvalid,
		SidTypeUnknown,
		SidTypeComputer
	}

	// This structure contains the trustee information for the ACE. There are
	// two forms supplied. The first form takes a name (string) as input, while
	// the second form takes an IntPtr as input. Use the second form when you
	// want to provide a pointer such as a SID.
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct TRUSTEE 
	{
		public IntPtr                      pMultipleTrustee;
		public MULTIPLE_TRUSTEE_OPERATION  MultipleTrusteeOperation;
		public TRUSTEE_FORM                TrusteeForm;
		public TRUSTEE_TYPE                TrusteeType;
		public String                      ptstrName;
	}

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct TRUSTEE2
	{
		public IntPtr                      pMultipleTrustee;
		public MULTIPLE_TRUSTEE_OPERATION  MultipleTrusteeOperation;
		public TRUSTEE_FORM                TrusteeForm;
		public TRUSTEE_TYPE                TrusteeType;
		public IntPtr                      ptstrName;
	}

	// The MULTIPLE_TRUSTEE_OPERATION enumeration determines if this
	// is a single or a multiple trustee.
	public enum MULTIPLE_TRUSTEE_OPERATION 
	{
		NO_MULTIPLE_TRUSTEE,
		TRUSTEE_IS_IMPERSONATE,
	}

	// The TRUSTEE_FORM enumeration determines what form the ACE trustee
	// takes.
	public enum TRUSTEE_FORM 
	{
		TRUSTEE_IS_SID,
		TRUSTEE_IS_NAME,
		TRUSTEE_BAD_FORM,
		TRUSTEE_IS_OBJECTS_AND_SID,
		TRUSTEE_IS_OBJECTS_AND_NAME
	}

	// The TRUSTEE_TYPE enumeration determins the type of the trustee.
	public enum TRUSTEE_TYPE 
	{
		TRUSTEE_IS_UNKNOWN,
		TRUSTEE_IS_USER,
		TRUSTEE_IS_GROUP,
		TRUSTEE_IS_DOMAIN,
		TRUSTEE_IS_ALIAS,
		TRUSTEE_IS_WELL_KNOWN_GROUP,
		TRUSTEE_IS_DELETED,
		TRUSTEE_IS_INVALID,
		TRUSTEE_IS_COMPUTER
	}

	/// <summary>
	/// Summary description for SecurityAccessRight.
	/// </summary>
	public class SecurityAccessRight : IDisposable
	{			
		private bool _disposed;
		private string _path;
		private string _accountName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
		private uint _accessGranted;
//		private uint _accessDenied;
		private IntPtr _pSecurityDescriptor;
		private IntPtr _pDacl;
		private IntPtr _pSid;
		private bool _hasDemanded;

		public SecurityAccessRight(string path)
		{
			_path = path;						
		}

		public SecurityAccessRight(string path, string accountName)
		{
			_path = path;			
			_accountName = accountName;				
		}
		
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					/// dispose of managed resources here
				}

				/// dispose of unmanaged resources here				
				
				/// free the security descriptor, the dacl, and the sid
				try
				{
					Marshal.FreeHGlobal(_pSecurityDescriptor);
				}
				catch(System.Exception /* systemException */)
				{
					//					System.Diagnostics.Trace.WriteLine(systemException);
				}

				try
				{
					Marshal.FreeHGlobal(_pDacl);
				}
				catch(System.Exception /* systemException */)
				{
					//					System.Diagnostics.Trace.WriteLine(systemException);
				}

				try
				{
					Marshal.FreeHGlobal(_pSid);
				}
				catch(System.Exception /* systemException */)
				{
					//					System.Diagnostics.Trace.WriteLine(systemException);
				}
				
				/// flag ourself
				_disposed = true;
			}	
		}

		#endregion

		public string AccountName
		{
			get
			{
				return _accountName;
			}
			set
			{
				_accountName = value;
			}
		}

		public bool AssertReadAccess()
		{
			try
			{
				if (this.PertainsToADirectory())
				{
					// just fake like we did some good.
					return true;
				}
				else
				{
					using(FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						fs.Close();					
						return true;
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
//			return this.Assert(SecurityAccessRights.FILE_GENERIC_READ);
		}

		public bool AssertWriteAccess()
		{
			try
			{
				if (this.PertainsToADirectory())
				{
					// just fake like we did some good.
					return true;
				}
				else
				{
					using(FileStream fs = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						fs.Close();					
						return true;
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
//			return this.Assert(SecurityAccessRights.FILE_GENERIC_WRITE);			
		}

		public bool PertainsToADirectory()
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(_path);				
				return ((attributes & FileAttributes.Directory) == FileAttributes.Directory);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public bool PertainsToADirectory(string path)
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(path);				
				return ((attributes & FileAttributes.Directory) == FileAttributes.Directory);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public bool Assert(SecurityAccessRights accessRequested)
		{
			/// permissions only apply to NT based systems
			if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				if (!_hasDemanded)
				{
					this.GetEffectiveSecurityAccessRights();
					_hasDemanded = true;
				}						
				return (((uint)_accessGranted & (uint)accessRequested) == (uint)accessRequested);
			}
			return true;
		}

		private bool GetEffectiveSecurityAccessRights()
		{						
			try
			{
				bool daclPresent = false;
				bool defaulted = false;										
				int sidSize = 0;					
				SID_NAME_USE usage = SID_NAME_USE.SidTypeGroup;
				StringBuilder domain = new StringBuilder(80);
				int domainSize = 80;			

				// lookup the account name, first call gets the size
				LookupAccountName(IntPtr.Zero, _accountName, IntPtr.Zero, ref sidSize, domain, ref domainSize, ref usage);

				// allocate the memory for the SID
				_pSid = Marshal.AllocHGlobal(sidSize);

				// and calling again we get the sid
				domainSize = 80;
				LookupAccountName(IntPtr.Zero, _accountName, _pSid, ref sidSize, domain, ref domainSize, ref usage);

				// Create a the Trustee data structure.
				TRUSTEE2 trustee = new TRUSTEE2();
				trustee.MultipleTrusteeOperation = MULTIPLE_TRUSTEE_OPERATION.NO_MULTIPLE_TRUSTEE;
				trustee.pMultipleTrustee = IntPtr.Zero;
				trustee.ptstrName = _pSid;
				trustee.TrusteeForm = TRUSTEE_FORM.TRUSTEE_IS_SID;
				trustee.TrusteeType = TRUSTEE_TYPE.TRUSTEE_IS_UNKNOWN;

				this.GetFileSecurityDescriptor(_path, SecurityInformation.DACL, out _pSecurityDescriptor);
				if (_pSecurityDescriptor == IntPtr.Zero)
				{
					System.Diagnostics.Trace.WriteLine("File security descriptor is null");
					return false;;
				}
														
				// get the dacl from the descriptor				
				GetSecurityDescriptorDacl(_pSecurityDescriptor, ref daclPresent, out _pDacl, ref defaulted);
																															   
				// if the dacl is null or one is not found then all access is allowed																					
				if (!daclPresent || _pDacl == IntPtr.Zero)
					return true;
									
				// get the rights for the dacl
				int result = GetEffectiveRightsFromAcl(_pDacl, ref trustee, ref _accessGranted);
//				int result = GetAuditedPermissionsFromAcl(_pDacl, ref trustee, ref _accessGranted, ref _accessDenied);

				if (result != ERROR_SUCCESS)
					throw new System.ComponentModel.Win32Exception(result);

				return true;							
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			// by default fail on the side of good
			return true;
		}

		private void GetFileSecurityDescriptor(string path, SecurityInformation requestedInformation, out IntPtr securityDescriptor)
		{
			securityDescriptor = new IntPtr(0); 
			int size = 0; 
			int sizeNeeded = 0; 
			
			// call once to get the size needed
			GetFileSecurity(path, requestedInformation, securityDescriptor, 0, ref sizeNeeded);

			// Allocate the memory required for the security descriptor.
			securityDescriptor = Marshal.AllocHGlobal(sizeNeeded);
			size = sizeNeeded;

			// call again to get the security descriptor
			if (!GetFileSecurity(path, requestedInformation, securityDescriptor, size, ref sizeNeeded))
				// Free the memory we allocated.
				Marshal.FreeHGlobal(securityDescriptor);			
		}

		#region Win32 API

		/// <summary>
		/// The GetFileSecurity function obtains specified information about the security of a file or directory. The information obtained is constrained by the caller's access rights and privileges.
		///	The GetNamedSecurityInfo function provides functionality similar to GetFileSecurity for files as well as other types of objects.
		/// Windows NT 3.51 and earlier:  The GetNamedSecurityInfo function is not supported.
		/// </summary>
		/// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies the file or directory for which security information is retrieved.</param>
		/// <param name="requestedInformation">[in] A SecurityInformation value that identifies the security information being requested. </param>
		/// <param name="securityDescriptor">[out] Pointer to a buffer that receives a copy of the security descriptor of the object specified by the lpFileName parameter. The calling process must have permission to view the specified aspects of the object's security status. The SECURITY_DESCRIPTOR structure is returned in self-relative format.</param>
		/// <param name="length">[in] Specifies the size, in bytes, of the buffer pointed to by the pSecurityDescriptor parameter.</param>
		/// <param name="lengthNeeded">[out] Pointer to the variable that receives the number of bytes necessary to store the complete security descriptor. If the returned number of bytes is less than or equal to nLength, the entire security descriptor is returned in the output buffer; otherwise, none of the descriptor is returned.</param>
		/// <returns></returns>
		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern bool GetFileSecurity(string lpFileName, SecurityInformation requestedInformation, IntPtr securityDescriptor, int length, ref int lengthNeeded);
        
		// Successful operation constant.
		private const int ERROR_SUCCESS = 0;

		// This function retrieves the DACL from the file's security
		// descriptor.
		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern Boolean GetSecurityDescriptorDacl(
			IntPtr pSecurityDescriptor,
			ref Boolean lpbDaclPresent,
			out IntPtr pDacl,
			ref Boolean lpbDaclDefaulted);
		
		// This function retrieves a SID given a specific account name. The first form
		// is for remote access. The second form is for local access and you set the
		// lpSystemName value to IntPtr.Zero.
		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern Boolean LookupAccountName(String lpSystemName,
			String lpAccountName,
			IntPtr Sid,
			ref int cbSid,
			StringBuilder DomainName,
			ref int cbDomainName,
			ref SID_NAME_USE peUse);

		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern Boolean LookupAccountName(IntPtr NoSystemName,
			String lpAccountName,
			IntPtr Sid,
			ref int cbSid,
			StringBuilder DomainName,
			ref int cbDomainName,
			ref SID_NAME_USE peUse);		

		// This function retrieves the effective rights for a specific trustee
		// contained within an ACL. The returned rights include any group rights
		// that the trustee might have. The first form of this function accepts a
		// TRUSTEE structure containing a string name, while the second form
		// accepts a TRUSTEE structure containing an IntPtr to a structure such as
		// a SID.
		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern int GetEffectiveRightsFromAcl(IntPtr pacl,
			ref TRUSTEE pTrustee,
			ref UInt32 pAccessRights);

		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern int GetEffectiveRightsFromAcl(IntPtr pacl,
			ref TRUSTEE2 pTrustee,
			ref UInt32 pAccessRights);			

		[DllImport("AdvAPI32.DLL", CharSet=CharSet.Auto, SetLastError=true )]
		private static extern int GetAuditedPermissionsFromAcl(
			IntPtr pacl,
			ref TRUSTEE2 pTrustee,
			ref uint pSuccessfulAuditedRights,
			ref uint pFailedAuditedRights);

		#endregion		
	}
}