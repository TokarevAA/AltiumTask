using System;

namespace AltiumSort
{
	internal static class Utils
	{
		internal const string CHUNK_FILE_PREFIX = "chunk";


		internal static string GetGuidString()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}