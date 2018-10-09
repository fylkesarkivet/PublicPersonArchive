using Xunit;

namespace PersonArchive.Logic.UnitTests.Validate
{
	public class Time
	{
		[Theory]

		// True
		[InlineData(1985, 7, 29, 1985, 7, 28, true)]
		[InlineData(1985, 8, null, 1985, 7, null, true)]
		[InlineData(1986, null, null, 1985, null, null, true)]

		// False
		[InlineData(1985, 7, 28, 1985, 7, 28, false)]
		[InlineData(1985, 7, null, 1985, 7, null, false)]
		[InlineData(1985, null, null, 1985, null, null, false)]

		[InlineData(1985, 7, 27, 1985, 7, 28, false)]
		[InlineData(1985, 6, null, 1985, 7, null, false)]
		[InlineData(1984, null, null, 1985, null, null, false)]

		// Null
		[InlineData(null, null, null, null, null, null, null)]

		public void CompareDateIsInTheFuture(
			int? compareYear,
			int? compareMonth,
			int? compareDay,
			int? currentYear,
			int? currentMonth,
			int? currentDay,
			bool? expectedResult
		)
		{
			var compareDate =
				new Logic.Validate.FluffyDate(
					compareYear, compareMonth, compareDay);

			var currentDate =
				new Logic.Validate.FluffyDate(
					currentYear, currentMonth, currentDay);

			var time =
				new Logic.Validate.Time(
					compareDate,
					currentDate);

			Assert.Equal(
				expectedResult,
				time.CompareDateIsInTheFuture);
		}
	}
}