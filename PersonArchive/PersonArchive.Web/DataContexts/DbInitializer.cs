//using System;
//using System.Linq;
//using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.DataContexts
{
	public static class DbInitializer
	{
		public static void Seed(PersonDbContext context)
		{
			/*
			if (context.Persons.Any())
				return; // Db has been seeded

			var persons = new[]
			{
				new Person
				{
					Gender = PersonGender.Male,
					Names = new[]
					{
						new PersonName
						{
							First = "Lars",
							NameWeight = 1,
							TimeCreated = DateTime.Now,
							TimeLastUpdated = DateTime.Now
						}
					},
					Descriptions = new[]
					{
						new PersonDescription
						{
							Description = "Lars kjem frå Sogndal.",
							Type = PersonDescriptionType.Norwegian,
							TimeCreated = DateTime.Now,
							TimeLastUpdated = DateTime.Now
						}
					},
					FluffyDates = new[]
					{
						new PersonFluffyDate
						{
							Year = 1985,
							Type = PersonFluffyDateType.Birth,
							TimeCreated = DateTime.Now,
							TimeLastUpdated = DateTime.Now
						}
					},
					TimeCreated = DateTime.Now,
					TimeLastUpdated = DateTime.Now
				},
				new Person
				{
					Gender = PersonGender.Female,
					TimeCreated = DateTime.Now,
					TimeLastUpdated = DateTime.Now
				},
				new Person
				{
					Gender = PersonGender.Undefined,
					TimeCreated = DateTime.Now,
					TimeLastUpdated = DateTime.Now
				}
			};

			foreach (var person in persons)
				context.Persons.Add(person);

			context.SaveChanges();
			*/
		}
	}
}