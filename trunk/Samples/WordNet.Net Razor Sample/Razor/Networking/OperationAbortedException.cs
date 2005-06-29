using System;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for OperationAbortedException.
	/// </summary>
	public class OperationAbortedException : Exception
	{
		public OperationAbortedException() : base(@"The operation was aborted by a manual reset event.")
		{
			
		}
	}
}
