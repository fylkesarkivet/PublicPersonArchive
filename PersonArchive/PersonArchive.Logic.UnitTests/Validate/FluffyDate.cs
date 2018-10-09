using Xunit;
using Xunit.Sdk;

namespace PersonArchive.Logic.UnitTests.Validate
{
	public class FluffyDate
	{
		[Theory]
		[InlineData(null, false)]
		[InlineData(0, false)]
		[InlineData(1, true)]
		[InlineData(31, true)]
		[InlineData(32, false)]
		public void HasValidDay(
			int? day, 
			bool expectedResult)
		{
			var date = 
				new Logic
					.Validate
					.FluffyDate(null, null, day);

			Assert.Equal(
				expectedResult,
				date.HasValidDay);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData(0, false)]
		[InlineData(1, true)]
		[InlineData(12, true)]
		[InlineData(13, false)]
		public void HasValidMonth(
			int? month,
			bool expectedResult)
		{
			var date =
				new Logic
					.Validate
					.FluffyDate(null, month, null);

			Assert.Equal(
				expectedResult,
				date.HasValidMonth);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData(-1, false)]
		[InlineData(0, true)]
		[InlineData(1, true)]
		[InlineData(3000, true)]
		[InlineData(3001, false)]
		public void HasValidYear(
			int? year,
			bool expectedResult)
		{
			var date =
				new Logic
					.Validate
					.FluffyDate(year, null, null);

			Assert.Equal(
				expectedResult,
				date.HasValidYear);
		}

		[Theory]
		[InlineData(null, null, null, false)]
		[InlineData(1985, 7, 28, true)]
		[InlineData(2018, 2, 28, true)]
		[InlineData(2018, 2, 29, false)]
		public void IsValidDate(
			int? year,
			int? month,
			int? day,
			bool expectedResult)
		{
			var date =
				new Logic
					.Validate
					.FluffyDate(year, month, day);

			Assert.Equal(
				expectedResult,
				date.IsValidDate);
		}

		[Theory]
		// True
		[InlineData(null, null, null, true)]
		[InlineData(2018, 2, 14, true)]

		// False
		[InlineData(null, null, 0, false)]
		[InlineData(null, null, 1, false)]
		[InlineData(null, null, 31, false)]
		[InlineData(null, null, 32, false)]

		[InlineData(null, 1, 0, false)]
		[InlineData(null, 1, 1, false)]
		[InlineData(null, 1, 31, false)]
		[InlineData(null, 1, 32, false)]

		[InlineData(2018, null, 0, false)]
		[InlineData(2018, null, 1, false)]
		[InlineData(2018, null, 31, false)]
		[InlineData(2018, null, 32, false)]
		public void DayCriteriaOk(
			int? year,
			int? month,
			int? day,
			bool expectedResult)
		{
			var date =
				new Logic
					.Validate
					.FluffyDate(year, month, day);

			Assert.Equal(
				expectedResult,
				date.DayCriteriaOk);
		}

		[Theory]
		// True
		[InlineData(null, null, null, true)]
		[InlineData(2018, 2, 14, true)]
		// False
		[InlineData(null, 0, null, false)]
		[InlineData(null, 1, null, false)]
		[InlineData(null, 1, 31, false)]
		[InlineData(null, 1, 32, false)]
		public void MonthCriteriaOk(
			int? year,
			int? month,
			int? day,
			bool expectedResult)
		{
			var date =
				new Logic
					.Validate
					.FluffyDate(year, month, day);

			Assert.Equal(
				expectedResult,
				date.MonthCriteriaOk);
		}
	}
}