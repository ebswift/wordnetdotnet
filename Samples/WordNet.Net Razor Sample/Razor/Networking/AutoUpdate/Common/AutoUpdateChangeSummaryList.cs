using System;
using System.Collections;
using System.Diagnostics;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateChangeSummaryList.
	/// </summary>
	public class AutoUpdateChangeSummaryList : CollectionBase		
	{
		public AutoUpdateChangeSummaryList()
		{
			
		}

		public void Add(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			if (this.Contains(summary))
				throw new AutoUpdateChangeSummaryAlreadyExistsException(summary);

			base.InnerList.Add(summary);
		}

		public void AddRange(AutoUpdateChangeSummary[] summaries)
		{
			if (summaries == null)
				throw new ArgumentNullException("summaries");

			foreach(AutoUpdateChangeSummary summary in summaries)
				this.Add(summary);
		}	

		public void Remove(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			if (this.Contains(summary))
				base.InnerList.Remove(summary);
		}

		public bool Contains(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			foreach(AutoUpdateChangeSummary existingSummary in base.InnerList)
				if (string.Compare(existingSummary.Id, summary.Id, true) == 0)
					return true;

			return false;
		}

		public AutoUpdateChangeSummary this[int index]
		{
			get
			{
				return base.InnerList[index] as AutoUpdateChangeSummary;
			}
		}

		public AutoUpdateChangeSummary this[string id]
		{
			get
			{
				foreach(AutoUpdateChangeSummary existingSummary in base.InnerList)
					if (string.Compare(existingSummary.Id, id, true) == 0)
						return existingSummary;

				return null;
			}
		}
	}
}
