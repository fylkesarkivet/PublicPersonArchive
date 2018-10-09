using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using PersonArchive.Entities;
using PersonArchive.Entities.SearchIndex;
using PersonArchive.SearchStore.Person;

namespace PersonArchive.SearchStore.Services
{
	public class PersonIndex : IPersonIndex
	{
		private readonly IOptions<SearchStoreServiceSettingsModel> _serviceSettings;

		public PersonIndex(
			IOptions<SearchStoreServiceSettingsModel> serviceSettings)
		{
			_serviceSettings = serviceSettings;
		}

		public void DeleteIndexIfExists()
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				if (serviceClient.Indexes.Exists(_serviceSettings.Value.SearchIndexClientName))
					serviceClient.Indexes.Delete(_serviceSettings.Value.SearchIndexClientName);
			}
		}

		public void CreateIndex()
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var definition = new Index()
				{
					Name = _serviceSettings.Value.SearchIndexClientName,
					Fields = FieldBuilder.BuildForType<PersonIndexModel>()
				};

				serviceClient.Indexes.Create(definition);
			}
		}

		public void UploadDocuments(List<PersonDocumentCreateModel> createModels)
		{
			// Abort if list is empty
			if (!createModels.Any())
				return;

			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(_serviceSettings.Value.SearchIndexClientName);

				var persons = new List<PersonIndexModel>();

				foreach (var createModel in createModels)
				{
					persons.Add(new PersonIndexModel
					{
						PersonGuid = createModel.PersonGuid,
						Gender = createModel.Gender,
						Names = createModel.Names.ToArray(),
						YearOfBirth = createModel.YearOfBirth,
						YearOfDeath = createModel.YearOfDeath,
						Age = createModel.Age
					});
				}

				var batch = IndexBatch.Upload(persons);

				try
				{
					indexClient.Documents.Index(batch);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ", 
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public PersonSearchResult SearchPersonNames(
			string searchText,
			string filterText)
		{
			if (string.IsNullOrWhiteSpace(searchText))
				return new PersonSearchResult();

			using (var indexClient =
				new SearchIndexClient(
					_serviceSettings.Value.SearchServiceName,
					_serviceSettings.Value.SearchIndexClientName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				//
				// Search query
				//

				SearchParameters parameters;
				DocumentSearchResult<PersonIndexModel> search;

				parameters =
					new SearchParameters
					{
						Facets = new[] { "gender" },
						//Select = new[] { "names" },
						//HighlightFields = new[] { "names" },
						//Filter = "gender eq 'Male'",
						IncludeTotalResultCount = true
					};

				// Filter
				if (!string.IsNullOrWhiteSpace(filterText))
					parameters.Filter = filterText;

				search = indexClient.Documents.Search<PersonIndexModel>(searchText, parameters);

				//
				// Map and return results
				//

				var mappedSearchResult = new PersonSearchResult();

				if (search.Count != null)
					mappedSearchResult.NumberOfItemsInSearchResult = (int) search.Count;

				mappedSearchResult.NumberOfItemsOnThisPage = search.Results.Count;

				// Get persons
				foreach (var result in search.Results)
				{
					var personDocument = new PersonDocument
					{
						PersonGuid = result.Document.PersonGuid,
						Gender = result.Document.Gender,
						YearOfBirth = result.Document.YearOfBirth,
						YearOfDeath = result.Document.YearOfDeath,
						Age = result.Document.Age
					};

					// Get names and create highlights if possible
					try
					{
						foreach (var name in result.Document.Names)
						{
							var nameWithHighlight =
								Regex.Replace(
									name,
									searchText,
									match => "<em>" + match.Value + "</em>",
									RegexOptions.Compiled | RegexOptions.IgnoreCase);

							personDocument.Names.Add(nameWithHighlight);
						}
					}
					catch (ArgumentException)
					{
						// If user input contains * then Regex fails.
						// Then display names without highlights.
						personDocument.Names = result.Document.Names.ToList();
					}

					mappedSearchResult.PersonDocuments.Add(personDocument);
				}

				// Get facets
				if (search.Facets != null &&
				    search.Facets.ContainsKey("gender"))
				{
					foreach (var facet in search.Facets["gender"])
					{
						var newFacet = new SearchFacetItem
						{
							Name = facet.Value.ToString()
						};

						if (facet.Count != null)
						{
							newFacet.Count = (int)facet.Count;
						}

						mappedSearchResult
							.SearchFacetItems
							.Add(newFacet);
					}
				}

				return mappedSearchResult;
			}
		}

		public void MergeGender(
			string personGuid,
			string gender)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid,
					["gender"] = gender
				};

				var documents = IndexBatch.Merge(new[] { document } );

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public void MergeNames(
			string personGuid,
			string[] names)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid,
					["names"] = names
				};

				var documents = IndexBatch.Merge(new[] { document });

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public void MergeYearOfBirth(
			string personGuid,
			int? yearOfBirth)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid,
					["yearOfBirth"] = yearOfBirth
				};

				var documents = IndexBatch.Merge(new[] { document });

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public void MergeYearOfDeath(
			string personGuid,
			int? yearOfDeath)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid,
					["yearOfDeath"] = yearOfDeath
				};

				var documents = IndexBatch.Merge(new[] { document });

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public void MergeAge(
			string personGuid,
			int? age)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid,
					["age"] = age
				};

				var documents = IndexBatch.Merge(new[] { document });

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}

		public void DeletePerson(Guid personGuid)
		{
			using (var serviceClient =
				new SearchServiceClient(
					_serviceSettings.Value.SearchServiceName,
					new SearchCredentials(
						_serviceSettings.Value.PrimaryAdminKeyInAzurePortal)))
			{
				var indexClient =
					serviceClient.Indexes.GetClient(
						_serviceSettings.Value.SearchIndexClientName);

				var document = new Document
				{
					["personGuid"] = personGuid.ToString()
				};

				var documents = IndexBatch.Delete(new[] { document });

				try
				{
					indexClient.Documents.Index(documents);
				}
				catch (IndexBatchException e)
				{
					//TODO code something for if this fail
					// Sometimes when your Search service is under load, indexing will fail for some of the documents in
					// the batch. Depending on your application, you can take compensating actions like delaying and
					// retrying. For this simple demo, we just log the failed document keys and continue.
					Console.WriteLine(
						"Failed to index some of the documents: {0}",
						string.Join(
							", ",
							e.IndexingResults
								.Where(r => !r.Succeeded)
								.Select(r => r.Key)));
				}
			}
		}
	}
}