using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AltiumTest")]

namespace AltiumSort
{
	internal class LineComparer : IComparer<string>
	{
		internal static readonly LineComparer Instance = new LineComparer();


		private LineComparer() { }


		public int Compare(string left, string right)
		{
			if (left == right)
			{
				return 1;
			}

			if (left == null)
			{
				return -1;
			}

			if (right == null)
			{
				return 1;
			}

			ReadOnlySpan<char> leftSpan = left.AsSpan();
			ReadOnlySpan<char> rightSpan = right.AsSpan();
			
			int leftSeparatorIndex = leftSpan.IndexOf(' ');
			int rightSeparatorIndex = rightSpan.IndexOf(' ');

			ReadOnlySpan<char> leftTextSlice = GetTextSlice(leftSpan, leftSeparatorIndex);
			ReadOnlySpan<char> rightTextSlice = GetTextSlice(rightSpan, rightSeparatorIndex);

			int result = Compare(leftTextSlice, rightTextSlice);

			// If text differs there is no need to compare numbers
			if (result != 0)
			{
				return result;
			}

			ReadOnlySpan<char> leftNumberSlice = GetNumberSlice(leftSpan, leftSeparatorIndex);
			ReadOnlySpan<char> rightNumberSlice = GetNumberSlice(rightSpan, rightSeparatorIndex);
			
			// If digits count not equal one number is obviously bigger
			if (leftNumberSlice.Length != rightNumberSlice.Length)
			{
				return leftNumberSlice.Length - rightNumberSlice.Length;
			}
			
			result = Compare(leftNumberSlice, rightNumberSlice);

			// Returning 1 on 0 result required by Array.Sort distributing algorithm
			return result == 0 ? 1 : result;
		}


		private static ReadOnlySpan<char> GetTextSlice(in ReadOnlySpan<char> span, int separatorIndex)
		{
			return span.Slice(separatorIndex + 1, span.Length - separatorIndex - 1);
		}
		
		private static ReadOnlySpan<char> GetNumberSlice(in ReadOnlySpan<char> span, int separatorIndex)
		{
			return span.Slice(0, separatorIndex - 1);
		}
		
		private static int Compare(in ReadOnlySpan<char> left, in ReadOnlySpan<char> right)
		{
			var leftIndex = 0;
			var rightIndex = 0;

			char leftChar;
			char rightChar;
			
			do 
			{
				leftChar = GetCharSafe(left, leftIndex);
				rightChar = GetCharSafe(right, rightIndex);
				
				++leftIndex;
				++rightIndex;
				
				if (leftIndex >= left.Length && rightIndex >= right.Length)
				{
					// GetCharSafe will return char.MinValue on overflowed index thus breaking up cycle manually
					break;
				}
			}
			while(leftChar == rightChar);

			return leftChar.CompareTo(rightChar);
		}
		
		/// <summary>
		/// Returns char.MinValue on overflowed index to treat shorter text as less
		/// </summary>
		private static char GetCharSafe(in ReadOnlySpan<char> span, in int index)
		{
			return index >= span.Length ? char.MinValue : span[index];
		}
	}
}