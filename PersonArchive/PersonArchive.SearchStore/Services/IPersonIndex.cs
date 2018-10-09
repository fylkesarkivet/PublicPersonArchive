using System;
using System.Collections.Generic;
using PersonArchive.Entities.SearchIndex;

namespace PersonArchive.SearchStore.Services
{
	public interface IPersonIndex
	{
		void DeleteIndexIfExists();

		void CreateIndex();

		void UploadDocuments(
			List<PersonDocumentCreateModel> createModels);

		PersonSearchResult SearchPersonNames(
			string searchText,
			string searchFilter);

		void MergeGender(
			string personGuid,
			string gender);

		void MergeNames(
			string personGuid,
			string[] names);

		void MergeYearOfBirth(
			string personGuid,
			int? yearOfBirth);

		void MergeYearOfDeath(
			string personGuid,
			int? yearOfDeath);

		void MergeAge(
			string personGuid,
			int? age);

		void DeletePerson(Guid personGuid);
	}
}