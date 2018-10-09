using System;

namespace PersonArchive.Logic.Validate
{
	public class Time
	{
		public Time(
			FluffyDate compareDate,
			FluffyDate currentDate
		)
		{
			CompareDate = compareDate;
			CurrentDate = currentDate;
		}

		public FluffyDate CompareDate { get; internal set; }
		public FluffyDate CurrentDate { get; internal set; }

		public bool? CompareDateIsInTheFuture
		{
			get
			{
				if (CompareDate.IsValidDate &&
				    CurrentDate.IsValidDate)
				{
					var compareDate =
						new DateTime(
							CompareDate.Year.Value,
							CompareDate.Month.Value,
							CompareDate.Day.Value,
							0, 0, 0, 0);
					
					var currentDate =
						new DateTime(
							CurrentDate.Year.Value,
							CurrentDate.Month.Value,
							CurrentDate.Day.Value,
							0, 0, 0, 0);

					return compareDate > currentDate;
				}
				else if (
					CompareDate.HasValidYear &&
					CurrentDate.HasValidYear &&
					CompareDate.HasValidMonth &&
					CurrentDate.HasValidMonth
				)
				{
					var compareDate =
						new DateTime(
							CompareDate.Year.Value,
							CompareDate.Month.Value,
							1, 0, 0, 0, 0);

					var currentDate =
						new DateTime(
							CurrentDate.Year.Value,
							CurrentDate.Month.Value,
							1, 0, 0, 0, 0);

					return compareDate > currentDate;
				}
				else if (
					CompareDate.HasValidYear &&
					CurrentDate.HasValidYear
				)
				{
					var compareDate =
						new DateTime(
							CompareDate.Year.Value,
							1, 1, 0, 0, 0, 0);

					var currentDate =
						new DateTime(
							CurrentDate.Year.Value,
							1, 1, 0, 0, 0, 0);

					return compareDate > currentDate;
				}

				return null;
			}
		}
	}
}