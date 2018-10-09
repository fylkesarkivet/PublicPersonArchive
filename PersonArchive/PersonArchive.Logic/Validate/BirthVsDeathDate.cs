using System;

namespace PersonArchive.Logic.Validate
{
	public class BirthVsDeathDate
	{
		public BirthVsDeathDate(
			FluffyDate fluffyDateOfBirth,
			FluffyDate fluffyDateOfDeath
			)
		{
			FluffyDateOfBirth = fluffyDateOfBirth;
			FluffyDateOfDeath = fluffyDateOfDeath;
		}

		public FluffyDate FluffyDateOfBirth { get; internal set; }
		public FluffyDate FluffyDateOfDeath { get; internal set; }

		//
		// Birth vs death
		//

		public bool HasValidYears =>
			FluffyDateOfBirth.HasValidYear &&
			FluffyDateOfDeath.HasValidYear &&
			FluffyDateOfBirth.Year <= FluffyDateOfDeath.Year;

		public bool HasValidMonths
		{
			get
			{
				// Do we have all values to draw a conclusion?
				if (!HasValidYears || 
				    !FluffyDateOfBirth.HasValidMonth || 
				    !FluffyDateOfDeath.HasValidMonth)
					return false;

				// Extra check if birth and death year are equals
				if (FluffyDateOfBirth.Year.Equals(FluffyDateOfDeath.Year))
					return FluffyDateOfBirth.Month <= FluffyDateOfDeath.Month;

				return true;
			}
		}

		public bool HasValidDays
		{
			get
			{
				// Do we have all values to draw a conclusion?
				if (!FluffyDateOfBirth.IsValidDate ||
				    !FluffyDateOfDeath.IsValidDate)
					return false;

				var birth = new DateTime(
					FluffyDateOfBirth.Year.Value,
					FluffyDateOfBirth.Month.Value,
					FluffyDateOfBirth.Day.Value);

				var death = new DateTime(
					FluffyDateOfDeath.Year.Value,
					FluffyDateOfDeath.Month.Value,
					FluffyDateOfDeath.Day.Value);

				return birth <= death;
			}
		}
	}
}