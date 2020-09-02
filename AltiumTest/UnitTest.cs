using AltiumSort;
using Xunit;

namespace AltiumTest
{
	public class UnitTest
	{
		[Theory]
		[InlineData("412. Test", "412. Test")]
		[InlineData(null, null)]
		public void CompareStrings_SameString_One(string a, string b)
		{
			Assert.Equal(1, LineComparer.Instance.Compare(a, b));
		}

		[Theory]
		[InlineData("41. Test", "410. Test")]
		[InlineData("412. Ab", "410. Bb")]
		[InlineData("412. A", "410. Bb")]
		[InlineData("4444. t", "4. test")]
		[InlineData(null, "4. test")]
		public void CompareStrings_DifferentString_LessThanZero(string a, string b)
		{
			Assert.True(LineComparer.Instance.Compare(a, b) < 0);
		}

		[Theory]
		[InlineData("4. test", "4444. t")]
		[InlineData("4. tets", "4. test")]
		[InlineData("412. Test", "410. Test")]
		[InlineData("33. Bb", "410. A")]
		[InlineData("10029. anyway someone totally not", "1003. anyway someone totally not")]
		[InlineData("33. Bb", null)]
		public void CompareStrings_DifferentString_GreaterThanZero(string a, string b)
		{
			Assert.True(LineComparer.Instance.Compare(a, b) > 0);
		}
	}
}