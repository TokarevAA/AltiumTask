using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AltiumTest")]

namespace AltiumSort
{
	internal class LineComparer : IComparer<string>
	{
		internal static readonly LineComparer Instance = new LineComparer();


		private LineComparer()
		{
		}


		public unsafe int Compare(string x, string y)
		{
			if (x == y)
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
				int maxLength = Math.Max(x.Length, y.Length);
				int result = CompareText(xp, yp, x.Length, y.Length, maxLength);

				if (result == 0)
				{
					result = CompareDigit(xp, yp, maxLength);
				}

				return result == 0 ? 1 : result;
			}
		}


		private static unsafe int CompareText(char* xp, char* yp, int xLength, int yLength, int maxLength)
		{
			int xIndex = -1;
			int yIndex = -1;

			for (var i = 0; i < maxLength; i++)
			{
				char xValue = xp[i];
				char yValue = yp[i];

				SetLetterIndex(in xValue, in i, ref xIndex);
				SetLetterIndex(in yValue, in i, ref yIndex);

				if (xIndex <= 0 || yIndex <= 0)
				{
					continue;
				}

				char xLetter = xp[xIndex];
				char yLetter = yp[yIndex];

				while (xLetter == yLetter)
				{
					++xIndex;
					++yIndex;

					if (xIndex >= xLength)
					{
						if (yIndex >= yLength)
						{
							break;
						}

						return -1;
					}

					if (yIndex >= yLength)
					{
						return 1;
					}

					xLetter = xp[xIndex];
					yLetter = yp[yIndex];
				}

				if (xLetter != yLetter)
				{
					return xLetter.CompareTo(yLetter);
				}

				return 0;
			}

			return 0;
		}

		private static void SetLetterIndex(in char value, in int index, ref int letterIndex)
		{
			if (letterIndex < 0)
			{
				if (value == ' ')
				{
					letterIndex = index + 1;
				}
			}
		}

		private static unsafe int CompareDigit(char* xp, char* yp, int maxLength)
		{
			for (var i = 0; i < maxLength; i++)
			{
				char xValue = xp[i];
				char yValue = yp[i];

				if (xValue == '.')
				{
					if (yValue == '.')
					{
						return 1;
					}

					return -1;
				}

				if (yValue == '.')
				{
					return 1;
				}

				if (xValue != yValue)
				{
					return xValue.CompareTo(yValue);
				}
			}

			return 0;
		}
	}
}