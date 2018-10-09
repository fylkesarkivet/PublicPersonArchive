using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.Models.ViewModels
{
	public class DisplayPersonDescription
	{
		public DisplayPersonDescription(PersonDescription personDescription)
		{
			PersonDescription = personDescription;
		}

		public PersonDescription PersonDescription { get; }

		public int Id =>
			PersonDescription.PersonDescriptionId;

		public string Text =>
			PersonDescription.Description;

		public string Language
		{
			get
			{
				switch (PersonDescription.Type)
				{
					case PersonDescriptionType.Norwegian:
						return "Norsk";
					case PersonDescriptionType.English:
						return "Engelsk";
					default:
						return "?";
				}
			}
		}

		public bool LastItemInList { get; set; }
	}
}