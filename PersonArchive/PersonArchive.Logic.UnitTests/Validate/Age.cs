using Xunit;

namespace PersonArchive.Logic.UnitTests.Validate
{
	public class Age
	{
		[Theory]
		// True
		[InlineData(1985, 7, 28, 1985, 7, 28, 0)]
		[InlineData(1985, 7, 28, 1986, 7, 28, 1)]
		[InlineData(1985, 7, 28, 2018, 2, 13, 32)]
		[InlineData(1985, 7, null, 2018, 2, null, 32)]
		[InlineData(1985, null, null, 2018, null, null, 32)]
		[InlineData(1985, null, null, 1985, null, null, 0)]
		[InlineData(2000, 2, 1, 2000, 2, null, 0)]
		// False
		[InlineData(null, null, null, null, null, null, null)]
		[InlineData(1985, 7, 28, 1984, 7, 28, null)]
		[InlineData(1985, 7, 28, null, null, null, null)]
		public void InYears(
			int? startYear,
			int? startMonth,
			int? startDay,
			int? endYear,
			int? endMonth,
			int? endDay,
			int? expectedResult)
		{
			var fluffyDateOfBirth =
				new Logic.Validate.FluffyDate(
					startYear, startMonth, startDay);

			var fluffyDateOfDeath =
				new Logic.Validate.FluffyDate(
					endYear, endMonth, endDay);

			var age =
				new Logic
					.Validate
					.Age(
						fluffyDateOfBirth,
						fluffyDateOfDeath);

			Assert.Equal(
				expectedResult,
				age.InYears);
		}
	}
}