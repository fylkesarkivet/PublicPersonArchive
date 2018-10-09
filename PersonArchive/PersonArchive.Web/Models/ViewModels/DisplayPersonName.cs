using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.Models.ViewModels
{
	public class DisplayPersonName
	{
		public DisplayPersonName(PersonName personName)
		{
			PersonName = personName;
		}

		public PersonName PersonName { get; }

		public int Id => PersonName.PersonNameId;
		public int NameWeight => PersonName.NameWeight;

		public string FullName
		{
			get
			{
				var fullName = 
					string.Join(" ", new[]
					{
						PersonName.Prefix,
						PersonName.First,
						PersonName.Middle,
						PersonName.Last,
						PersonName.Suffix
					}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

				return fullName;
			}
		}

		public List<string> NameSplitUpInList
		{
			get
			{
				var list = new List<string>();

				if (!string.IsNullOrWhiteSpace(PersonName.Prefix))
					list.Add($"Prefix: {PersonName.Prefix}");

				if (!string.IsNullOrWhiteSpace(PersonName.First))
					list.Add($"Fornamn: {PersonName.First}");

				if (!string.IsNullOrWhiteSpace(PersonName.Middle))
					list.Add($"Mellomnamn: {PersonName.Middle}");

				if (!string.IsNullOrWhiteSpace(PersonName.Last))
					list.Add($"Etternamn: {PersonName.Last}");

				if (!string.IsNullOrWhiteSpace(PersonName.Suffix))
					list.Add($"Suffix: {PersonName.Suffix}");

				if (list.Count == 1)
				{
					// Only return split type if there is only one of them.

					if (!string.IsNullOrWhiteSpace(PersonName.Prefix))
						return new List<string> { "Prefix" };

					if (!string.IsNullOrWhiteSpace(PersonName.First))
						return new List<string> { "Fornamn" };

					if (!string.IsNullOrWhiteSpace(PersonName.Middle))
						return new List<string> { "Mellomnamn" };

					if (!string.IsNullOrWhiteSpace(PersonName.Last))
						return new List<string> { "Etternamn" };

					if (!string.IsNullOrWhiteSpace(PersonName.Suffix))
						return new List<string> { "Suffix" };
				}

				return list;
			}
		}

		public bool LastItemInList { get; set; }
	}
}