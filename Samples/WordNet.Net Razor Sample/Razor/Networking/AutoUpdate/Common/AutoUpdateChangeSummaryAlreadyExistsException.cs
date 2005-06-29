using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateChangeSummaryAlreadyExistsException.
	/// </summary>
	public class AutoUpdateChangeSummaryAlreadyExistsException : Exception
	{
		protected AutoUpdateChangeSummary _summary;

		public AutoUpdateChangeSummaryAlreadyExistsException(AutoUpdateChangeSummary summary) : base("A summary with the same Id already exists.")
		{
			_summary = summary;
		}

		public AutoUpdateChangeSummary Summary
		{
			get
			{
				return _summary;
			}
		}
	}
}
