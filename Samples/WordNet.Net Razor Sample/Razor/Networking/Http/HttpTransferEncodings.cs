using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpTransferEncodings.
	/// </summary>
	public class HttpTransferEncodings
	{
		/// <summary>
		/// The chunked encoding modifies the body of a message in order to transfer it as a series of chunks, each with its own size indicator, followed by an OPTIONAL trailer containing entity-header fields. This allows dynamically produced content to be transferred along with the information necessary for the recipient to verify that it has received the full message.																																																							 
		/// </summary>
		public const string Chunked = @"chunked";

		/// <summary>
		/// The default (identity) encoding; the use of no transformation whatsoever. This content-coding is used only in the Accept-Encoding header, and SHOULD NOT be used in the Content-Encoding header.
		/// </summary>
		public const string Identity = @"identity";

		/// <summary>
		/// An encoding format produced by the file compression program “gzip” (GNU zip) as described in RFC 1952 [25]. This format is a Lempel-Ziv coding (LZ77) with a 32 bit CRC.
		/// </summary>
		public const string GZip = @"gzip";

		/// <summary>
		/// The encoding format produced by the common UNIX file compression program “compress”. This format is an adaptive Lempel-Ziv-Welch coding (LZW). Use of program names for the identification of encoding formats is not desirable and is discouraged for future encodings. Their use here is representative of historical practice, not good design. For compatibility with previous implementations of HTTP, applications SHOULD consider “x-gzip” and “x-compress” to be equivalent to “gzip” and “compress” respectively.
		/// </summary>
		public const string Compress = @"compress";

		/// <summary>
		/// The “zlib” format defined in RFC 1950 [31] in combination with the “deflate” compression mechanism described in RFC 1951 [29].
		/// </summary>
		public const string Deflate = @"deflate";
	}
}
