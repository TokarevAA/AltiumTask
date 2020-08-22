using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AltiumTest")]

namespace AltiumSort
{
	internal class LineComparer : IComparer<string>
	{
		private const char _SEPARATOR = '.';


		internal static readonly LineComparer Instance = new LineComparer();


		private LineComparer()
		{
		}


		public unsafe int Compare(string x, string y)
		{
			if (x == null && y == null)
			{
				return 1;
			}

			if (x == null)
			{
				return -1;
			}

			if (y == null)
			{
				return 1;
			}

			fixed (char* xp = x, yp = y)
			{
				int xSeparatorIndex = x.IndexOf(_SEPARATOR, StringComparison.Ordinal);
				int ySeparatorIndex = y.IndexOf(_SEPARATOR, StringComparison.Ordinal);

				int maxLength = Math.Max(x.Length - xSeparatorIndex, y.Length - ySeparatorIndex);

				for (var index = 0; index < maxLength - 1; index++)
				{
					int xIndex = xSeparatorIndex + index + 1;
					int yIndex = ySeparatorIndex + index + 1;

					if (xIndex >= x.Length)
					{
						return -1;
					}

					if (yIndex >= y.Length)
					{
						return 1;
					}

					char xValue = xp[xIndex];
					char yValue = yp[yIndex];

					if (xValue != yValue)
					{
						int result = xValue.CompareTo(yValue);
						return result == 0 ? 1 : result;
					}
				}

				// Ensured content equality. Now number compare

				if (xSeparatorIndex < ySeparatorIndex)
				{
					return -1;
				}

				if (xSeparatorIndex > ySeparatorIndex)
				{
					return 1;
				}

				maxLength = Math.Max(xSeparatorIndex, ySeparatorIndex);

				for (var index = 0; index < maxLength; index++)
				{
					if (index >= xSeparatorIndex)
					{
						return -1;
					}

					if (index >= ySeparatorIndex)
					{
						return 1;
					}

					char xValue = xp[index];
					char yValue = yp[index];

					if (xValue != yValue)
					{
						int result = xValue.CompareTo(yValue);
						return result == 0 ? 1 : result;
					}
				}

				return 1;
			}
		}
	}
}