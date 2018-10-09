using System;
using System.Globalization;

namespace PersonArchive.Logic.Validate
{
	public class FluffyDate
	{
		public FluffyDate(int? year, int? month, int? day)
		{
			Year = year;
			Month = month;
			Day = day;
		}

		public int? Year { get; internal set; }
		public int? Month { get; internal set; }
		public int? Day { get; internal set; }

		public bool IsValidDate =>
			DateTime.TryParseExact(
				$"{Year}-{Month}-{Day}",
				"yyyy-M-d",
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out _);

		public bool HasValidDay => 
			Day != null &&
			Day >= 1 &&
			Day <= 31;

		public bool HasValidMonth =>
			Month != null &&
			Month >= 1 &&
			Month <= 12;

		public bool HasValidYear =>
			Year != null &&
			Year >= 0 &&
			Year <= 3000;

		public bool DayCriteriaOk
		{
			get
			{
				if (Day == null)
					return true;

				return HasValidYear && HasValidMonth && HasValidDay;
			}
		}

		public bool MonthCriteriaOk
		{
			get
			{
				if (Month == null)
					return true;

				return HasValidYear && HasValidMonth;
			}
		}
	}
}