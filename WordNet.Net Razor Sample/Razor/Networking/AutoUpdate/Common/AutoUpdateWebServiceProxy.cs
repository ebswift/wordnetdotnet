using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateWebService.
	/// </summary>
	//	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name="AutoUpdateServiceSoap", Namespace="http://tempuri.org/")]
	public class AutoUpdateWebServiceProxy : SoapHttpClientProtocol
	{		
		public static string DefaultWebServiceUrl = @"http://mrbelles.brinkster.net/webservices/autoupdate/autoupdatewebservice.asmx";

		/// <summary>
		/// Initializes a new instance of the AutoUpdateWebService class
		/// </summary>
		public AutoUpdateWebServiceProxy() 
		{
			this.Url = DefaultWebServiceUrl;
		}
        
		/// <summary>
		/// Initializes a new instance of the AutoUpdateWebService class
		/// </summary>
		/// <param name="url">The url of the service description file</param>
		public AutoUpdateWebServiceProxy(string url) 
		{
			this.Url = url;	
			this.PreAuthenticate = true;
			this.Credentials = System.Net.CredentialCache.DefaultCredentials;
		}

		#region Version 2.0.0

		/// <remarks/>
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryLatestVersionEx", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Xml.XmlNode QueryLatestVersionEx(string productName, string currentVersion, string productId) 
		{
			object[] results = this.Invoke("QueryLatestVersionEx", new object[] {
																					productName,
																					currentVersion,
																					productId});
			return ((System.Xml.XmlNode)(results[0]));
		}
        
		/// <remarks/>
		public System.IAsyncResult BeginQueryLatestVersionEx(string productName, string currentVersion, string productId, System.AsyncCallback callback, object asyncState) 
		{
			return this.BeginInvoke("QueryLatestVersionEx", new object[] {
																			 productName,
																			 currentVersion,
																			 productId}, callback, asyncState);
		}
        
		/// <remarks/>
		public System.Xml.XmlNode EndQueryLatestVersionEx(System.IAsyncResult asyncResult) 
		{
			object[] results = this.EndInvoke(asyncResult);
			return ((System.Xml.XmlNode)(results[0]));
		}
        
		#endregion

//		#region Version 1.0.0
//		
//		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryLatestVersion", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
//		public string QueryLatestVersion(string appName) 
//		{			
//			object[] results = this.Invoke("QueryLatestVersion", new object[] {
//																				  appName});
//			return ((string)(results[0]));
//		}
//        		
//		public System.IAsyncResult BeginQueryLatestVersion(string appName, System.AsyncCallback callback, object asyncState) 
//		{
//			return this.BeginInvoke("QueryLatestVersion", new object[] {
//																		   appName}, callback, asyncState);
//		}
//        		
//		public string EndQueryLatestVersion(System.IAsyncResult asyncResult) 
//		{
//			object[] results = this.EndInvoke(asyncResult);
//			return ((string)(results[0]));
//		}
//
//		#endregion		
	}
}
