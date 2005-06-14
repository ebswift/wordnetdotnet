using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateChangeTypes
	/// </summary>
	public enum AutoUpdateChangeTypes
	{
		/*
		 * Bug fixes and corrections
		 * */
		Correction,

		/*
		 * new libraries, classes, and features
		 * */
		Addition,

		/*
		 * reviews of features needed or existing
		 * */
		Review,

		/*
		 * rewrites of existing code
		 * */
		Rewrite
	}

	/// <summary>
	/// Summary description for AutoUpdateChangeSummary.
	/// </summary>
	public class AutoUpdateChangeSummary
	{
		protected string _id;
		protected string _title;
		protected string _preview;
		protected string _postedBy;
		protected DateTime _datePosted;
		protected AutoUpdateChangeTypes _type;		
		protected static int MaxPreviewLength = 150;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateChangeSummary class
		/// </summary>
		public AutoUpdateChangeSummary()
		{
			_id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateChangeSummary class
		/// </summary>
		/// <param name="title">The title of the change</param>
		/// <param name="preview">A preview of the change</param>
		/// <param name="postedBy">Who posted the change</param>
		/// <param name="datePosted">The date it was posted</param>
		/// <param name="type">The type of change</param>
		public AutoUpdateChangeSummary(string id, string title, string preview, string postedBy, DateTime datePosted, AutoUpdateChangeTypes type)
		{
			_id = id;
			_title = title;
			this.Preview = preview;
			_postedBy = postedBy;
			_datePosted = datePosted;
			_type = type;
		}
		
		/// <summary>
		/// Gets or sets the id for this change
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets or sets the title of the change
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a preview of the description, usually about 150 words or less...
		/// </summary>
		public string Preview
		{
			get
			{
				return _preview;
			}
			set
			{
				if (value != null)
				{
					if (value.Length > MaxPreviewLength)
					{
						value = value.Substring(0, MaxPreviewLength);
						value += @"...";
					}
				}

				_preview = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the person that posted the change
		/// </summary>
		public string PostedBy
		{
			get
			{
				return _postedBy;
			}
			set
			{
				_postedBy = value;
			}
		}

		/// <summary>
		/// Gets or sets the date the change was posted
		/// </summary>
		public DateTime DatePosted
		{
			get
			{
				return _datePosted;
			}
			set
			{
				_datePosted = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of change made
		/// </summary>
		public AutoUpdateChangeTypes Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
	}
}
