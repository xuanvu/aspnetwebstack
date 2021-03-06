﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace System.Net.Http
{
    /// <summary>
    /// Provides various internal utility functions
    /// </summary>
    internal static class FormattingUtilities
    {
        /// <summary>
        /// The default max depth for our formatter is 256
        /// </summary>
        public const int DefaultMaxDepth = 256;

        /// <summary>
        /// The default min depth for our formatter is 1
        /// </summary>
        public const int DefaultMinDepth = 1;

        /// <summary>
        /// HTTP X-Requested-With header field name
        /// </summary>
        public const string HttpRequestedWithHeader = @"x-requested-with";

        /// <summary>
        /// HTTP X-Requested-With header field value
        /// </summary>
        public const string HttpRequestedWithHeaderValue = @"xmlhttprequest";

        /// <summary>
        /// HTTP Host header field name
        /// </summary>
        public const string HttpHostHeader = "Host";

        /// <summary>
        /// HTTP Version token
        /// </summary>
        public const string HttpVersionToken = "HTTP";

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="UTF8Encoding"/>.
        /// </summary>
        public static readonly Type Utf8EncodingType = typeof(UTF8Encoding);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="UnicodeEncoding"/>.
        /// </summary>
        public static readonly Type Utf16EncodingType = typeof(UnicodeEncoding);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="HttpRequestMessage"/>.
        /// </summary>
        public static readonly Type HttpRequestMessageType = typeof(HttpRequestMessage);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="HttpResponseMessage"/>.
        /// </summary>
        public static readonly Type HttpResponseMessageType = typeof(HttpResponseMessage);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="HttpContent"/>.
        /// </summary>
        public static readonly Type HttpContentType = typeof(HttpContent);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="DelegatingEnumerable{T}"/>.
        /// </summary>
        public static readonly Type DelegatingEnumerableGenericType = typeof(DelegatingEnumerable<>);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static readonly Type EnumerableInterfaceGenericType = typeof(IEnumerable<>);

        /// <summary>
        /// A <see cref="Type"/> representing <see cref="IQueryable{T}"/>.
        /// </summary>
        public static readonly Type QueryableInterfaceGenericType = typeof(IQueryable<>);

        /// <summary>
        /// Determines whether <paramref name="type"/> is a <see cref="JToken"/> type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="type"/> is a <see cref="JToken"/> type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJTokenType(Type type)
        {
            return typeof(JToken).IsAssignableFrom(type);
        }

        /// <summary>
        /// Creates an empty <see cref="HttpContentHeaders"/> instance. The only way is to get it from a dummy 
        /// <see cref="HttpContent"/> instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        public static HttpContentHeaders CreateEmptyContentHeaders()
        {
            HttpContent tempContent = null;
            HttpContentHeaders contentHeaders = null;
            try
            {
                tempContent = new StringContent(String.Empty);
                contentHeaders = tempContent.Headers;
                contentHeaders.Clear();
            }
            finally
            {
                // We can dispose the content without touching the headers
                if (tempContent != null)
                {
                    tempContent.Dispose();
                }
            }

            return contentHeaders;
        }

        /// <summary>
        /// Ensure the actual collection is identical to the expected one
        /// </summary>
        /// <param name="actual">The actual collection of the instance</param>
        /// <param name="expected">The expected collection of the instance</param>
        /// <returns>Returns true if they are identical</returns>
        public static bool ValidateCollection(Collection<MediaTypeHeaderValue> actual, MediaTypeHeaderValue[] expected)
        {
            if (actual.Count != expected.Length)
            {
                return false;
            }

            foreach (MediaTypeHeaderValue value in expected)
            {
                if (!actual.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create a default reader quotas with a default depth quota of 1K
        /// </summary>
        /// <returns></returns>
        public static XmlDictionaryReaderQuotas CreateDefaultReaderQuotas()
        {
            return new XmlDictionaryReaderQuotas()
            {
                MaxArrayLength = Int32.MaxValue,
                MaxBytesPerRead = Int32.MaxValue,
                MaxDepth = DefaultMaxDepth,
                MaxNameTableCharCount = Int32.MaxValue,
                MaxStringContentLength = Int32.MaxValue
            };
        }

        /// <summary>
        /// Remove bounding quotes on a token if present
        /// </summary>
        /// <param name="token">Token to unquote.</param>
        /// <returns>Unquoted token.</returns>
        public static string UnquoteToken(string token)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}
