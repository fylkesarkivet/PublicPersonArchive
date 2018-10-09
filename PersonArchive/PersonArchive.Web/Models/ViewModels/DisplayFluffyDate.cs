using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;

namespace PersonArchive.Web.Models.ViewModels
{
	public class DisplayFluffyDate
	{
		public int PersonFluffyDateId { get; internal set; }
		public int? Year { get; internal set; }
		public int? Month { get; internal set; }
		public int? Day { get; internal set; }
		private PersonFluffyDateType PersonFluffyDateType { get; }

		public DisplayFluffyDate(
			int personFluffyDateId,
			int? year,
			int? month,
			int? day,
			PersonFluffyDateType type)
		{
			PersonFluffyDateId = personFluffyDateId;
			Year = year;
			Month = month;
			Day = day;
			PersonFluffyDateType = type;
		}

		public string Date
		{
			get
			{
				var fluffyDate = new FluffyDate(Year, Month, Day);

				var cultureInfo = new CultureInfo("nn-NO");
				Thread.CurrentThread.CurrentCulture = cultureInfo;

				var dateIsValidDate =
					DateTime.TryParseExact(
						$"{Year}-{Month}-{Day}",
						"yyyy-M-d",
						cultureInfo,
						DateTimeStyles.None,
						out var fullDateParsed);

				if (dateIsValidDate)
				{
					var dateFormatedAsText =
						string.Format(
							"{0:dddd, d. MMMM yyyy}",
							fullDateParsed);

					var dateFormatedAsTextWithFirstUpperCase =
						dateFormatedAsText.First().ToString().ToUpper() +
						dateFormatedAsText.Substring(1);

					return dateFormatedAsTextWithFirstUpperCase;
				}

				if (fluffyDate.HasValidYear &&
					fluffyDate.HasValidMonth)
				{
					DateTime.TryParseExact(
						$"{Year}-{Month}-1",
						"yyyy-M-d",
						cultureInfo,
						DateTimeStyles.None,
						out var parsedDate);

					var dateFormatedAsText =
						string.Format(
							"{0:MMMM yyyy}",
							parsedDate);

					var dateFormatedAsTextWithFirstUpperCase =
						dateFormatedAsText.First().ToString().ToUpper() +
						dateFormatedAsText.Substring(1);

					return dateFormatedAsTextWithFirstUpperCase;
				}

				return fluffyDate.HasValidYear ?
					$"År {Year}" :
					"Ukjent årstal"; // Only empty fluffy date if person is dead
			}
		}

		public string Type
		{
			get
			{
				switch (PersonFluffyDateType)
				{
					case PersonFluffyDateType.Birth:
						return "Fødd";
					case PersonFluffyDateType.Death:
						return "Død";
					default:
						return "?";
				}
			}
		}

		public bool LastItemInList { get; set; }
	}
}