using System;

namespace PersonArchive.Logic.Validate
{
	public class Age
	{
		public Age(
			FluffyDate fluffyDateStart,
			FluffyDate fluffyDateEnd
		)
		{
			FluffyDateStart = fluffyDateStart;
			FluffyDateEnd = fluffyDateEnd;
		}

		public FluffyDate FluffyDateStart { get; internal set; }
		public FluffyDate FluffyDateEnd { get; internal set; }

		public int? InYears
		{
			get
			{
				int years = -1;

				DateTime dt1 = new DateTime();
				DateTime dt2 = new DateTime();

				if (FluffyDateStart.IsValidDate &&
				    FluffyDateEnd.IsValidDate)
				{
					dt1 =
						new DateTime(
							FluffyDateStart.Year.Value,
							FluffyDateStart.Month.Value,
							FluffyDateStart.Day.Value,
							0, 0, 0, 0);

					dt2 =
						new DateTime(
							FluffyDateEnd.Year.Value,
							FluffyDateEnd.Month.Value,
							FluffyDateEnd.Day.Value,
							0, 0, 0, 0);

					// If the day of a month are the same each year
					if (FluffyDateStart.Year <= FluffyDateEnd.Year &&
						FluffyDateStart.Month == FluffyDateEnd.Month &&
						FluffyDateStart.Day == FluffyDateEnd.Day)
						years++; // You are a year older on your birthday
				}
				else if (
					FluffyDateStart.HasValidYear &&
					FluffyDateEnd.HasValidYear &&
					FluffyDateStart.HasValidMonth &&
					FluffyDateEnd.HasValidMonth
				)
				{
					dt1 =
						new DateTime(
							FluffyDateStart.Year.Value,
							FluffyDateStart.Month.Value,
							1, 0, 0, 0, 0);

					dt2 =
						new DateTime(
							FluffyDateEnd.Year.Value,
							FluffyDateEnd.Month.Value,
							1, 0, 0, 0, 0);

					if (dt1 == dt2)
						return 0;
				}
				else if (
					FluffyDateStart.HasValidYear &&
					FluffyDateEnd.HasValidYear
				)
				{
					dt1 =
						new DateTime(
							FluffyDateStart.Year.Value,
							1, 1, 0, 0, 0, 0);

					dt2 =
						new DateTime(
							FluffyDateEnd.Year.Value,
							1, 1, 0, 0, 0, 0);

					if (dt1 == dt2)
						return 0;
				}

				DateTime tmp = dt1;

				while (tmp < dt2)
				{
					years++;
					tmp = tmp.AddYears(1);
				}

				if (years < 0)
					return null;

				return years;
			}
		}
	}
}