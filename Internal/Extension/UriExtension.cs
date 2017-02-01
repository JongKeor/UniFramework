using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;

namespace UniFramework.Extension
{

	public static class UriExtension
	{
		sealed class HttpQSCollection : NameValueCollection
		{
			public override string ToString ()
			{
				int count = Count;
				if (count == 0)
					return "";
				StringBuilder sb = new StringBuilder ();
				string[] keys = AllKeys;
				for (int i = 0; i < count; i++) {
					sb.AppendFormat ("{0}={1}&", keys [i], this [keys [i]]);
				}
				if (sb.Length > 0)
					sb.Length--;
				return sb.ToString ();
			}
		}

		public static Uri ExtendQuery (this Uri uri, IDictionary<string, string> values)
		{
			var uriBuilder = new UriBuilder (uri);
			var query = ParseQueryString (uriBuilder.Query);

			foreach (var kv in values) {
				query [kv.Key] = kv.Value;
			}

			uriBuilder.Query = query.ToString ();

			return uriBuilder.Uri;
		}

		public static NameValueCollection ParseQueryString (string s)
		{
			NameValueCollection nvc = new HttpQSCollection ();
			if (s.Contains ("?")) {
				s = s.Substring (s.IndexOf ('?') + 1);
			}
			foreach (string vp in Regex.Split(s, "&")) {
				if (string.IsNullOrEmpty (vp))
					continue;
				string[] singlePair = Regex.Split (vp, "=");

				if (singlePair.Length == 2) {
					nvc.Add (singlePair [0], singlePair [1]);
				} else {
					nvc.Add (singlePair [0], string.Empty);
				}
			}
			return nvc;

		}

	}
}


