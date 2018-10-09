using System;
using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.DataTripleStore.Services
{
	public interface IPersonData
	{
		void AddOrUpdatePerson(Person person);
		void DeletePerson(Guid personGuid);
	}
}