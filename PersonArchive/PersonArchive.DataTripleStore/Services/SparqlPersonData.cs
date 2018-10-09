using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using PersonArchive.Entities;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;
using VDS.RDF;
using VDS.RDF.Writing;

namespace PersonArchive.DataTripleStore.Services
{
	public class SparqlPersonData : IPersonData
	{
		private readonly IOptions<DataTripleStoreServiceSettingsModel> _serviceSettings;

		public SparqlPersonData(
			IOptions<DataTripleStoreServiceSettingsModel> serviceSettings)
		{
			_serviceSettings = serviceSettings;
		}

		public void AddOrUpdatePerson(Person person)
		{
			if (person.PersonGuid == Guid.Empty)
				return;

			IGraph g = new Graph();

			// Namespaces
			g.BaseUri = 
				new Uri($"https://resource.fylkesarkivet.no/person/{person.PersonGuid}#");

			// Default:
			// @prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
			// @prefix rdfs: < http://www.w3.org/2000/01/rdf-schema#>.
			// @prefix xsd: < http://www.w3.org/2001/XMLSchema#>.

			g.NamespaceMap.AddNamespace("foaf", UriFactory.Create("http://xmlns.com/foaf/0.1/"));
			// http://dbpedia.org/ontology/Person
			g.NamespaceMap.AddNamespace("dbo", UriFactory.Create("http://dbpedia.org/ontology/"));
			g.NamespaceMap.AddNamespace("dbp", UriFactory.Create("http://dbpedia.org/property/"));
			g.NamespaceMap.AddNamespace("xsd", UriFactory.Create("http://www.w3.org/2001/XMLSchema#"));
			g.NamespaceMap.AddNamespace("fark", UriFactory.Create("https://ontology.fylkesarkivet.no/"));

			// Agent
			g.Assert(new Triple(
				g.CreateUriNode(),
				g.CreateUriNode("rdf:type"),
				g.CreateUriNode("foaf:Agent")));

			// Person
			g.Assert(new Triple(
				g.CreateUriNode(),
				g.CreateUriNode("rdf:type"),
				g.CreateUriNode("foaf:Person")));

			// Gender
			switch (person.Gender)
			{
				case PersonGender.Male:
					g.Assert(new Triple(
						g.CreateUriNode(),
						g.CreateUriNode("foaf:Gender"),
						g.CreateLiteralNode("male")));
					break;
				case PersonGender.Female:
					g.Assert(new Triple(
						g.CreateUriNode(),
						g.CreateUriNode("foaf:Gender"),
						g.CreateLiteralNode("female")));
					break;
				case PersonGender.GenderVariant:
					g.Assert(new Triple(
						g.CreateUriNode(),
						g.CreateUriNode("foaf:Gender"),
						g.CreateLiteralNode("gender variant")));
					break;
			}

			// Name
			if (person.Names != null && person.Names.Any())
			{
				foreach (var name in person.Names)
				{
					var triples = new List<Triple>();

					var blankNode = g.CreateBlankNode();

					triples.Add(
						new Triple(
							g.CreateUriNode(),
							g.CreateUriNode("fark:Name"),
							blankNode));

					// foaf:name

					var fullName =
						string.Join(" ", new[]
						{
							name.Prefix,
							name.First,
							name.Middle,
							name.Last,
							name.Suffix
						}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

					if (!string.IsNullOrWhiteSpace(fullName))
						triples.Add(
							new Triple(
								blankNode,
								g.CreateUriNode("foaf:name"),
								g.CreateLiteralNode(fullName)));

					// foaf:firstName
					var firstName =
						string.Join(" ", new[]
						{
							name.Prefix,
							name.First
						}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
					if (!string.IsNullOrWhiteSpace(firstName))
						triples.Add(
							new Triple(
								blankNode,
								g.CreateUriNode("foaf:firstName"),
								g.CreateLiteralNode(firstName)));

					// foaf:middleName
					if (!string.IsNullOrWhiteSpace(name.Middle))
						triples.Add(
							new Triple(
								blankNode,
								g.CreateUriNode("foaf:middleName"),
								g.CreateLiteralNode(name.Middle)));

					// foaf:lastName
					var lastName =
						string.Join(" ", new[]
						{
							name.Last,
							name.Suffix
						}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
					if (!string.IsNullOrWhiteSpace(lastName))
						triples.Add(
							new Triple(
								blankNode,
								g.CreateUriNode("foaf:lastName"),
								g.CreateLiteralNode(lastName)));

					// fark:nameWeight

					if (!string.IsNullOrWhiteSpace(name.NameWeight.ToString()))
						triples.Add(
							new Triple(
								blankNode,
								g.CreateUriNode("fark:nameWeight"),
								g.CreateLiteralNode(
									name.NameWeight.ToString(),
									g.CreateUriNode("xsd:integer").Uri)));

					// Add triples to graph
					if (triples.Count > 1)
						g.Assert(triples);
				}
			}

			// Description
			if (person.Descriptions != null && person.Descriptions.Any())
			{
				foreach (var description in person.Descriptions)
				{
					var lang = "en";

					if (description.Type == PersonDescriptionType.Norwegian)
						lang = "no";

					g.Assert(new Triple(
						g.CreateUriNode(),
						g.CreateUriNode("dbo:abstract"),
						g.CreateLiteralNode(description.Description, lang)));
				}
			}

			// Date
			if (person.FluffyDates != null && person.FluffyDates.Any())
			{
				foreach (var fluffyDate in person.FluffyDates)
				{
					if (fluffyDate.Type == PersonFluffyDateType.Birth &&
						fluffyDate.Type == PersonFluffyDateType.Death)
						continue;

					var date = 
						new FluffyDate(
							fluffyDate.Year, 
							fluffyDate.Month, 
							fluffyDate.Day);

					var monthWithZero = string.Format("{0:00}", date.Month);
					var dayWithZero = string.Format("{0:00}", date.Day);

					var dateType = "dbo:birthDate";
					var yearType = "dbo:birthYear";

					if (fluffyDate.Type == PersonFluffyDateType.Death)
					{
						dateType = "dbo:deathDate";
						yearType = "dbo:deathYear";

						// FluffyDate can be blank if person is dead
						g.Assert(new Triple(
							g.CreateUriNode(),
							g.CreateUriNode("rdf:type"),
							g.CreateUriNode("fark:Dead")));
					}

					if (date.IsValidDate)
						g.Assert(new Triple(
							g.CreateUriNode(),
							g.CreateUriNode(dateType),
							g.CreateLiteralNode(
								$"{date.Year}-{monthWithZero}-{dayWithZero}", 
								g.CreateUriNode("xsd:date").Uri)));

					if (date.HasValidYear)
						g.Assert(new Triple(
							g.CreateUriNode(),
							g.CreateUriNode(yearType),
							g.CreateLiteralNode(
								$"{date.Year}",
								g.CreateUriNode("xsd:gYear").Uri)));
				}
			}

			// Timestamp
			g.Assert(new Triple(
				g.CreateUriNode(),
				g.CreateUriNode("dbp:timestamp"),
				g.CreateLiteralNode(
					person.TimeLastUpdated.ToString("yyyyMMddHHmmss"),
					g.CreateUriNode("xsd:double").Uri)));

			var writer = new CompressingTurtleWriter();
			writer.Save(g, $"{_serviceSettings.Value.RdfTurtleFilesForPersonPath}/{person.PersonGuid}.ttl");
		}

		public void DeletePerson(Guid personGuid)
		{
			File.Delete($"{_serviceSettings.Value.RdfTurtleFilesForPersonPath}/{personGuid}.ttl");
		}
	}
}