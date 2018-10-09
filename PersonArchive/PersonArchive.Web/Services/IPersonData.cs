using System;
using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.Services
{
	public interface IPersonData
	{
		//
		// Person
		//

		//TODO Switch IEnumerable with queryable later. IEnumerable is in memory and will be bad in a big database.
		IEnumerable<Person> GetAll();

		IQueryable<Person> ReadAllPersonsWithAllData();

		IQueryable<Person> ReadLastAddedPersonsWithAllData(int limit);

		Person GetPerson(Guid guid);
		Person ReadPerson(Guid guid);
		Person ReadPersonWithNames(Guid guid);
		Person ReadPersonWithDescriptions(Guid guid);
		Person ReadPersonWithFluffyDates(Guid guid);
		Person ReadAllPersonData(Guid guid);
		Person AddPerson(Person person);
		Person UpdatePerson(Person person);
		void DeletePerson(Person person);

		//
		// Description
		//

		PersonDescription AddPersonDescription(PersonDescription description);
		void DeletePersonDescription(PersonDescription description);
		PersonDescription GetPersonDescriptionWithNavigation(int personDescriptionId);
		PersonDescription ReadPersonDescriptionWithNavigation(int personDescriptionId);
		PersonDescription UpdatePersonDescription(PersonDescription description);

		//
		// Name
		//

		PersonName AddPersonName(PersonName name);
		void DeletePersonName(PersonName name);
		PersonName UpdatePersonName(PersonName name);
		PersonName ReadPersonNameWithNavigation(int personNameId);
		PersonName GetPersonNameWithNavigation(int personNameId);
		void AdjustAllNameWeights(int personId);
		void AdjustAllNameWeightsButOne(int personId, int personNameId);

		//
		// Flyffy date
		//

		PersonFluffyDate AddPersonFluffyDate(PersonFluffyDate fluffyDate);
		void DeletePersonFluffyDate(PersonFluffyDate fluffyDate);
		PersonFluffyDate UpdatePersonFluffyDate(PersonFluffyDate fluffyDate);
		PersonFluffyDate ReadPersonFluffyDateWithNavigation(int personNameId);
		IQueryable<PersonFluffyDate> ReadPersonFluffyDatesByPersonId(int personId);
		PersonFluffyDate GetPersonFluffyDateWithNavigation(int personNameId);
	}
}