using System;

namespace Razor.Networking.Http
{
	/*
	  "100"  ; Section 10.1.1: Continue
    | "101"  ; Section 10.1.2: Switching Protocols
    | "200"  ; Section 10.2.1: OK
    | "201"  ; Section 10.2.2: Created
    | "202"  ; Section 10.2.3: Accepted
    | "203"  ; Section 10.2.4: Non-Authoritative Information
    | "204"  ; Section 10.2.5: No Content
    | "205"  ; Section 10.2.6: Reset Content
    | "206"  ; Section 10.2.7: Partial Content
	| "300"  ; Section 10.3.1: Multiple Choices
	| "301"  ; Section 10.3.2: Moved Permanently
	| "302"  ; Section 10.3.3: Found
	| "303"  ; Section 10.3.4: See Other
	| "304"  ; Section 10.3.5: Not Modified
	| "305"  ; Section 10.3.6: Use Proxy
	| "307"  ; Section 10.3.8: Temporary Redirect
	| "400"  ; Section 10.4.1: Bad Request
	| "401"  ; Section 10.4.2: Unauthorized
	| "402"  ; Section 10.4.3: Payment Required
	| "403"  ; Section 10.4.4: Forbidden
	| "404"  ; Section 10.4.5: Not Found
	| "405"  ; Section 10.4.6: Method Not Allowed
	| "406"  ; Section 10.4.7: Not Acceptable
	| "407"  ; Section 10.4.8: Proxy Authentication Required
	| "408"  ; Section 10.4.9: Request Time-out
	| "409"  ; Section 10.4.10: Conflict
	| "410"  ; Section 10.4.11: Gone
	| "411"  ; Section 10.4.12: Length Required
	| "412"  ; Section 10.4.13: Precondition Failed
	| "413"  ; Section 10.4.14: Request Entity Too Large
	| "414"  ; Section 10.4.15: Request-URI Too Large
	| "415"  ; Section 10.4.16: Unsupported Media Type
	| "416"  ; Section 10.4.17: Requested range not satisfiable
	| "417"  ; Section 10.4.18: Expectation Failed
	| "500"  ; Section 10.5.1: Internal Server Error
	| "501"  ; Section 10.5.2: Not Implemented
	| "502"  ; Section 10.5.3: Bad Gateway
	| "503"  ; Section 10.5.4: Service Unavailable
	| "504"  ; Section 10.5.5: Gateway Time-out
	| "505"  ; Section 10.5.6: HTTP Version not supported

	*/

	[Serializable()]
	public class ContinueStatus : HttpStatus
	{
		public ContinueStatus() : base(100, "Continue")
		{
		}
	}

	[Serializable()]
	public class SwitchingProtocolsStatus : HttpStatus
	{
		public SwitchingProtocolsStatus() : base(101, @"Switching Protocols")
		{
		}
	}

	[Serializable()]
	public class OkStatus : HttpStatus
	{
		public OkStatus() : base(200, @"OK")
		{
		}
	}

	[Serializable()]
	public class CreatedStatus : HttpStatus
	{
		public CreatedStatus() : base(201, @"Created")
		{
		}
	}

	[Serializable()]
	public class AcceptedStatus : HttpStatus
	{
		public AcceptedStatus() : base(202, @"Accepted")
		{
		}
	}

	[Serializable()]
	public class NonAuthoritativeInformationStatus : HttpStatus
	{
		public NonAuthoritativeInformationStatus() : base(203, @"Non-Authoritative Information")
		{
		}
	}

	[Serializable()]
	public class NoContentStatus : HttpStatus
	{
		public NoContentStatus() : base(204, @"No Content")
		{
		}
	}

	[Serializable()]
	public class ResetContentStatus : HttpStatus
	{
		public ResetContentStatus() : base(205, @"Reset Content")
		{
		}
	}

	[Serializable()]
	public class PartialContentStatus : HttpStatus
	{
		public PartialContentStatus() : base(206, @"Partial Content")
		{			
		}
	}

	[Serializable()]
	public class MultipleChoicesStatus : HttpStatus
	{
		public MultipleChoicesStatus() : base(300, @"Multiple Choices")
		{
		}
	}

	[Serializable()]
	public class MovedPermanentlyStatus : HttpStatus
	{
		public MovedPermanentlyStatus() : base(301, @"Moved Permanently")
		{
		}
	}

	[Serializable()]
	public class FoundStatus : HttpStatus
	{
		public FoundStatus() : base(302, @"Found")
		{
		}
	}

	[Serializable()]
	public class SeeOtherStatus : HttpStatus
	{
		public SeeOtherStatus() : base(303, @"See Other")
		{
		}
	}

	[Serializable()]
	public class NotModifiedStatus : HttpStatus
	{
		public NotModifiedStatus() : base(304, @"Not Modified")
		{
		}
	}

	[Serializable()]
	public class UseProxyStatus : HttpStatus
	{
		public UseProxyStatus() : base(305, @"Use Proxy")
		{
		}
	}

	[Serializable()]
	public class TemporaryRedirectStatus : HttpStatus
	{
		public TemporaryRedirectStatus() : base(307, @"Temporary Redirect")
		{
		}
	}

	[Serializable()]
	public class BadRequestStatus : HttpStatus
	{
		public BadRequestStatus() : base(400, @"Bad Request")
		{
		}
	}

	[Serializable()]
	public class UnauthorizedStatus : HttpStatus
	{
		public UnauthorizedStatus() : base(401, @"Unauthorized")
		{
		}
	}

	[Serializable()]
	public class PaymentRequiredStatus : HttpStatus
	{
		public PaymentRequiredStatus() : base(402, @"Payment Required")
		{
		}
	}

	[Serializable()]
	public class ForbiddenStatus : HttpStatus
	{
		public ForbiddenStatus() : base(403, @"Forbidden")
		{
		}
	}

	[Serializable()]
	public class NotFoundStatus : HttpStatus
	{
		public NotFoundStatus() : base(404, @"Not Found")
		{
		}
	}

	[Serializable()]
	public class MethodNotAllowedStatus : HttpStatus
	{
		public MethodNotAllowedStatus() : base(405, @"Method Not Allowed")
		{
		}
	}

	[Serializable()]
	public class NotAcceptableStatus : HttpStatus
	{
		public NotAcceptableStatus() : base(406, @"Not Acceptable")
		{
		}
	}

	[Serializable()]
	public class ProxyAuthenticationRequiredStatus : HttpStatus
	{
		public ProxyAuthenticationRequiredStatus() : base(407, @"Proxy Authentication Required")
		{
		}
	}

	[Serializable()]
	public class RequestTimeoutStatus : HttpStatus
	{
		public RequestTimeoutStatus() : base(408, @"Request Time-out")
		{
		}
	}

	[Serializable()]
	public class ConflictStatus : HttpStatus
	{
		public ConflictStatus() : base(409, @"Conflict")
		{
		}
	}

	[Serializable()]
	public class GoneStatus : HttpStatus
	{
		public GoneStatus() : base(410, @"Gone")
		{
		}
	}

	[Serializable()]
	public class LengthRequiredStatus : HttpStatus
	{
		public LengthRequiredStatus() : base(411, @"Length Required")
		{
		}
	}

	[Serializable()]
	public class PreconditionFailedStatus : HttpStatus
	{
		public PreconditionFailedStatus() : base(412, @"Precondition Failed")
		{
		}
	}

	[Serializable()]
	public class RequestEntityTooLargeStatus : HttpStatus
	{
		public RequestEntityTooLargeStatus() : base(413, @"Request-Entity Too Large")
		{
		}
	}

	[Serializable()]
	public class RequestUriTooLargeStatus : HttpStatus
	{
		public RequestUriTooLargeStatus() : base(414, @"Request-Uri Too Large")
		{
		}
	}

	[Serializable()]
	public class UnsupportedMediaTypeStatus : HttpStatus
	{
		public UnsupportedMediaTypeStatus() : base(415, @"Unsupported Media Type")
		{
		}
	}

	[Serializable()]
	public class RequestedRangeNotSatisfiableStatus : HttpStatus
	{
		public RequestedRangeNotSatisfiableStatus() : base(416, @"Requested range not satisfiable")
		{
		}
	}

	[Serializable()]
	public class ExpectationFailedStatus : HttpStatus
	{
		public ExpectationFailedStatus() : base(417, @"Expectation Failed")
		{
		}
	}

	[Serializable()]
	public class InternalServerErrorStatus : HttpStatus
	{
		public InternalServerErrorStatus() : base(500, @"Internal Server Error")
		{
		}
	}

	[Serializable()]
	public class NotImplementedStatus : HttpStatus
	{
		public NotImplementedStatus() : base(501, @"Not Implemented")
		{
		}
	}

	[Serializable()]
	public class BadGatewayStatus : HttpStatus
	{
		public BadGatewayStatus() : base(502, @"Bad Gateway")
		{
		}
	}

	[Serializable()]
	public class ServiceUnavailableStatus : HttpStatus
	{
		public ServiceUnavailableStatus() : base(503, @"Service Unavailable")
		{
		}
	}

	[Serializable()]
	public class GatewayTimeoutStatus : HttpStatus
	{
		public GatewayTimeoutStatus() : base(504, @"Gateway Time-out")
		{
		}
	}

	[Serializable()]
	public class HttpVersionNotSupportedStatus : HttpStatus
	{
		public HttpVersionNotSupportedStatus() : base(505, @"HTTP Version not supported")
		{
		}
	}

}
