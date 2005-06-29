using System;
using Razor.Networking.AutoUpdate.Common.Xml;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateManifest.
	/// </summary>
	public class AutoUpdateManifest
	{
		protected string _id;
		protected AutoUpdateProductDescriptor _product;
		protected AutoUpdateHref _moreInfo;
		protected AutoUpdateChangeSummaryList _changeSummaries;		
		protected string _urlOfUpdate;	
		protected long _sizeOfUpdate;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateManifest class
		/// </summary>
		public AutoUpdateManifest()
		{
			_id = Guid.NewGuid().ToString();
			_product = new AutoUpdateProductDescriptor();
			_moreInfo = new AutoUpdateHref();
			_changeSummaries = new AutoUpdateChangeSummaryList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="product"></param>
		/// <param name="moreInfo"></param>
		/// <param name="changeSummaries"></param>
		/// <param name="urlOfUpdate"></param>
		/// <param name="sizeOfUpdate"></param>
		public AutoUpdateManifest(string id, AutoUpdateProductDescriptor product, AutoUpdateHref moreInfo, AutoUpdateChangeSummaryList changeSummaries, string urlOfUpdate, long sizeOfUpdate)
		{
			_id = id;
			_product = product;
			_moreInfo = moreInfo;
			_changeSummaries = changeSummaries;
			_urlOfUpdate = urlOfUpdate;
			_sizeOfUpdate = sizeOfUpdate;
		}

		#region My Public Properties

		/// <summary>
		/// Gets or sets the manifest id (This will be the update identifier used for registration key hashing)
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
		/// Gets or sets the product descriptor 
		/// </summary>
		public AutoUpdateProductDescriptor Product
		{
			get
			{
				return _product;
			}
			set
			{
				_product = value;
			}
		}

		/// <summary>
		/// Gets or sets the href for more information
		/// </summary>
		public AutoUpdateHref MoreInfo
		{
			get
			{
				return _moreInfo;
			}
			set
			{
				_moreInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the change summary list
		/// </summary>
		public AutoUpdateChangeSummaryList ChangeSummaries
		{
			get
			{
				return _changeSummaries;
			}
			set
			{
				_changeSummaries = value;
			}
		}

		/// <summary>
		/// Gets or sets the where the update can be downloaded from (ex: http://www.depcoinc.com/autoupdate/updates/assistant/1.0.0.0.update) (this Url can be a web link or a unc path)
		/// </summary>
		public string UrlOfUpdate
		{
			get
			{
				return _urlOfUpdate;
			}
			set
			{
				_urlOfUpdate = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of the update as it will be when downloaded
		/// </summary>
		public long SizeOfUpdate
		{
			get
			{
				return _sizeOfUpdate;
			}
			set
			{
				_sizeOfUpdate = value;
			}
		}

		#endregion

		#region My Virtual Methods

		/// <summary>
		/// Returns the Xml representing this AutoUpdateManifest
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return XmlAutoUpdateManifestWriter.ToXml(this, System.Text.Encoding.UTF8);
		}

		#endregion
	}
}
