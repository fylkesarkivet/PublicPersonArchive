using Xunit;

namespace PersonArchive.Logic.UnitTests.Validate
{
	public class BirthVsDeathDate
	{
		[Theory]
		// True
		[InlineData(1, 1, true)]
		[InlineData(1990, 1990, true)]
		[InlineData(1990, 2000, true)]
		// False
		[InlineData(null, null, false)]
		[InlineData(-1, -1, false)]
		[InlineData(1990, 1989, false)]
		public void HasValidYears(
			int? yearOfBirth,
			int? yearOfDeath,
			bool expectedResult)
		{
			var fluffyDateOfBirth =
				new Logic.Validate.FluffyDate(yearOfBirth, null, null);

			var fluffyDateOfDeath =
				new Logic.Validate.FluffyDate(yearOfDeath, null, null);

			var birthVsDeathDate =
				new Logic
					.Validate
					.BirthVsDeathDate(
						fluffyDateOfBirth,
						fluffyDateOfDeath);

			Assert.Equal(
				expectedResult,
				birthVsDeathDate.HasValidYears);
		}

		[Theory]
		// True
		[InlineData(1990, 1, 1990, 1, true)]
		[InlineData(1990, 1, 1990, 2, true)]
		// False
		[InlineData(null, null, null, null, false)]
		[InlineData(1990, 2, 1990, 1, false)]
		[InlineData(1991, 1, 1990, 1, false)]
		[InlineData(1991, 1, 1990, 2, false)]
		public void HasValidMonths(
			int? yearOfBirth,
			int? monthOfBirth,
			int? yearOfDeath,
			int? monthOfDeath,
			bool expectedResult)
		{
			var fluffyDateOfBirth =
				new Logic
					.Validate
					.FluffyDate(yearOfBirth, monthOfBirth, null);

			var fluffyDateOfDeath =
				new Logic
					.Validate
					.FluffyDate(yearOfDeath, monthOfDeath, null);

			var birthVsDeathDate =
				new Logic
					.Validate
					.BirthVsDeathDate(
						fluffyDateOfBirth,
						fluffyDateOfDeath);

			Assert.Equal(
				expectedResult,
				birthVsDeathDate.HasValidMonths);
		}

		[Theory]
		// True
		[InlineData(1990, 1, 1, 1990, 1, 1, true)]
		[InlineData(1990, 1, 1, 1990, 1, 2, true)]
		// False
		[InlineData(null, null, null, null, null, null, false)]
		[InlineData(1991, 1, 1, 1990, 1, 1, false)]
		[InlineData(1990, 2, 1, 1990, 1, 1, false)]
		[InlineData(1990, 1, 2, 1990, 1, 1, false)]
		public void HasValidDays(
			int? yearOfBirth,
			int? monthOfBirth,
			int? dayOfBirth,
			int? yearOfDeath,
			int? monthOfDeath,
			int? dayOfDeath,
			bool expectedResult)
		{
			var fluffyDateOfBirth =
				new Logic
					.Validate
					.FluffyDate(yearOfBirth, monthOfBirth, dayOfBirth);

			var fluffyDateOfDeath =
				new Logic
					.Validate
					.FluffyDate(yearOfDeath, monthOfDeath, dayOfDeath);

			var birthVsDeathDate =
				new Logic
					.Validate
					.BirthVsDeathDate(
						fluffyDateOfBirth,
						fluffyDateOfDeath);

			Assert.Equal(
				expectedResult,
				birthVsDeathDate.HasValidDays);
		}
	}
}