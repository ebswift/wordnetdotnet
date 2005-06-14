using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpMessageTypes.
	/// </summary>
	[Serializable()]
	public enum HttpMessageTypes
	{
		HttpRequest,
		HttpResponse,
		Unknown
	}
}
