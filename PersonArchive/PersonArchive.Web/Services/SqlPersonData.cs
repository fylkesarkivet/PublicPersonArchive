using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Web.DataContexts;

namespace PersonArchive.Web.Services
{
	public class SqlPersonData : IPersonData
	{
		private readonly PersonDbContext _context;

		public SqlPersonData(PersonDbContext context)
		{
			_context = context;
		}

		//
		// Person
		//

		public IEnumerable<Person> GetAll()
		{
			return _context
				.Persons
				.OrderBy(p => p.PersonId);
		}

		public IQueryable<Person> ReadAllPersonsWithAllData()
		{
			return _context
				.Persons
				.OrderBy(p => p.PersonId)
				.Include(i => i.FluffyDates)
				.Include(i => i.Names)
				.Include(i => i.Descriptions)
				.AsNoTracking(); // read-only scenario
		}

		public IQueryable<Person> ReadLastAddedPersonsWithAllData(int limit)
		{
			return _context
				.Persons
				.OrderByDescending(x => x.TimeCreated)
				.Take(limit)
				.Include(i => i.FluffyDates)
				.Include(i => i.Names)
				.Include(i => i.Descriptions)
				.AsNoTracking(); // read-only scenario
		}

		public Person GetPerson(Guid guid)
		{
			return _context
				.Persons
				.FirstOrDefault(p => p.PersonGuid == guid);
		}

		public Person ReadPerson(Guid guid)
		{
			return _context
				.Persons
				.Where(p => p.PersonGuid == guid)
				.AsNoTracking() // read-only scenario
				.SingleOrDefault();
		}

		public Person ReadPersonWithDescriptions(Guid guid)
		{
			return _context
				.Persons
				.Where(p => p.PersonGuid == guid)
				.Include(i => i.Descriptions)
				.AsNoTracking() // read-only scenario
				.SingleOrDefault();
		}

		public Person ReadPersonWithFluffyDates(Guid guid)
		{
			return _context
				.Persons
				.Where(p => p.PersonGuid == guid)
				.Include(i => i.FluffyDates)
				.AsNoTracking() // read-only scenario
				.SingleOrDefault();
		}

		public Person ReadPersonWithNames(Guid guid)
		{
			return _context
				.Persons
				.Where(p => p.PersonGuid == guid)
				.Include(i => i.Names)
				.AsNoTracking() // read-only scenario
				.SingleOrDefault();
		}

		public Person ReadAllPersonData(Guid guid)
		{
			return _context
				.Persons
				.Where(p => p.PersonGuid == guid)
				.Include(i => i.FluffyDates)
				.Include(i => i.Names)
				.Include(i => i.Descriptions)
				.AsNoTracking() // read-only scenario
				.SingleOrDefault();
		}

		public Person AddPerson(Person person)
		{
			_context
				.Persons
				.Add(person);

			_context.SaveChanges();

			return person;
		}

		public Person UpdatePerson(Person person)
		{
			person.TimeLastUpdated =
				DateTime.Now;

			_context
				.Attach(person)
				.State = EntityState.Modified;

			_context.SaveChanges();

			return person;
		}

		public void DeletePerson(Person person)
		{
			_context.Persons.Remove(person);
			_context.SaveChanges();
		}

		//
		// Description
		//

		public PersonDescription AddPersonDescription(
			PersonDescription description)
		{
			_context.PersonDescriptions.Add(description);
			_context.SaveChanges();
			return description;
		}

		public void DeletePersonDescription(
			PersonDescription description)
		{
			_context.PersonDescriptions.Remove(description);
			_context.SaveChanges();
		}

		public PersonDescription GetPersonDescriptionWithNavigation(
			int personDescriptionId)
		{
			return _context
				.PersonDescriptions
				.Where(p => p.PersonDescriptionId == personDescriptionId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public PersonDescription ReadPersonDescriptionWithNavigation(
			int personDescriptionId)
		{
			return _context
				.PersonDescriptions
				.AsNoTracking() // read-only scenario
				.Where(p => p.PersonDescriptionId == personDescriptionId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public PersonDescription UpdatePersonDescription(
			PersonDescription description)
		{
			description.TimeLastUpdated =
				DateTime.Now;

			_context
				.Attach(description)
				.State = EntityState.Modified;

			_context.SaveChanges();

			return description;
		}

		//
		// Name
		//

		public PersonName AddPersonName(
			PersonName name)
		{
			_context.PersonNames.Add(name);
			_context.SaveChanges();
			return name;
		}

		public PersonName ReadPersonNameWithNavigation(
			int personNameId)
		{
			return _context
				.PersonNames
				.AsNoTracking() // read-only scenario
				.Where(p => p.PersonNameId == personNameId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public PersonName GetPersonNameWithNavigation(int personNameId)
		{
			return _context
				.PersonNames
				.Where(p => p.PersonNameId == personNameId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public void AdjustAllNameWeights(int personId)
		{
			var names =
				_context
					.PersonNames
					.Where(p => p.PersonId == personId)
					.OrderBy(x => x.NameWeight);

			if (!names.Any()) return;

			var nameWeightCounter = 1;

			foreach (var name in names)
			{
				name.NameWeight = nameWeightCounter;
				// Dont't update TimeLastUpdated because
				// it is the system that updates the name weight.
				_context.Attach(name).State = EntityState.Modified;
				nameWeightCounter++;
			}

			_context.SaveChanges();
		}

		public void AdjustAllNameWeightsButOne(
			int personId, 
			int personNameId)
		{
			var names =
				_context
					.PersonNames
					.Where(p => p.PersonId == personId)
					.OrderBy(x => x.NameWeight);

			// Don't do anything if there is less than two names
			if (!names.Any() || names.Count() < 2) return;
			
			var allButThisOne = 
				names.First(x => x.PersonNameId == personNameId);

			var nameWeightCounter = 1;

			// All names
			foreach (var name in names)
			{
				// All the other names
				if (name.PersonNameId != allButThisOne.PersonNameId)
				{
					// Update counter if the weight number is used
					if (nameWeightCounter == allButThisOne.NameWeight)
						nameWeightCounter++;

					// Update name weight
					name.NameWeight = nameWeightCounter;

					// Dont't update TimeLastUpdated because
					// it is the system that updates the name weight.

					_context
						.Attach(name)
						.State = EntityState.Modified;

					// Update nameWeightCounter and move on
					nameWeightCounter++;
				}
			}

			_context.SaveChanges();
		}

		public PersonName UpdatePersonName(PersonName name)
		{
			name.TimeLastUpdated = DateTime.Now;

			_context
				.Attach(name)
				.State = EntityState.Modified;

			_context.SaveChanges();

			return name;
		}

		public void DeletePersonName(
			PersonName name)
		{
			_context.PersonNames.Remove(name);
			_context.SaveChanges();
		}

		//
		// Flyffy date
		//

		public PersonFluffyDate AddPersonFluffyDate(
			PersonFluffyDate fluffyDate)
		{
			_context.PersonFluffyDates.Add(fluffyDate);
			_context.SaveChanges();
			return fluffyDate;
		}

		public PersonFluffyDate ReadPersonFluffyDateWithNavigation(
			int personFluffyDateId)
		{
			return _context
				.PersonFluffyDates
				.AsNoTracking() // read-only scenario
				.Where(p => p.PersonFluffyDateId == personFluffyDateId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public IQueryable<PersonFluffyDate> ReadPersonFluffyDatesByPersonId(
			int personId)
		{
			return _context
				.PersonFluffyDates
				.AsNoTracking() // read-only scenario
				.Where(x => x.PersonId == personId);
		}

		public PersonFluffyDate GetPersonFluffyDateWithNavigation(int personFluffyDateId)
		{
			return _context
				.PersonFluffyDates
				.Where(p => p.PersonFluffyDateId == personFluffyDateId)
				.Include(i => i.Person) // Navigation
				.SingleOrDefault();
		}

		public PersonFluffyDate UpdatePersonFluffyDate(PersonFluffyDate fluffyDate)
		{
			fluffyDate.TimeLastUpdated =
				DateTime.Now;

			_context
				.Attach(fluffyDate)
				.State = EntityState.Modified;

			_context.SaveChanges();

			return fluffyDate;
		}

		public void DeletePersonFluffyDate(
			PersonFluffyDate fluffyDate)
		{
			_context.PersonFluffyDates.Remove(fluffyDate);
			_context.SaveChanges();
		}
	}
}