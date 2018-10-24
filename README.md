# PublicPersonArchive
Person archive source code. Developed in the project "Semantisk teknologi og tenesteutvikling for arkiv".

* Project report: https://www.arkivverket.no/arkivutvikling/utviklingsmidler-for-arkivsektoren/_/attachment/download/b30648b6-f875-403d-88f3-efa9e7b3497e:49a9bd44b49aed8f293b009a4a55d548e743e8f2/Fylkesarkivet%20i%20Sogn%20og%20Fjordane%20-%20Semantrisk%20teknologi%20og%20tjenesteutvikling%20for%20arkiv.pdf
* A demo is available here: https://demo-person-archive.fylkesarkivet.no/

## Technologies

This web solution is built with the integrated development environment (IDE) [Visual Studio Professional 2017](https://visualstudio.microsoft.com/vs/) from Microsoft.

The application uses the cross-platform and open-source framework [Microsoft ASP.NET Core v2](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) and is mainly written with the programming language [C#](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)).

It is designed with the principle of [separation of concerns (SoC)](https://en.wikipedia.org/wiki/Separation_of_concerns), separating the application into distinct sections.

The data section is built with [Entity Framework Core v2](https://docs.microsoft.com/en-us/ef/core/) from Microsoft. It is "a lightweight, extensible, and cross-platform version of the popular Entity Framework data access technology. EF Core can serve as an object-relational mapper (O/RM), enabling .NET developers to work with a database using .NET objects, and eliminating the need for most of the data-access code they usually need to write".

The search section is built with [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) from Microsoft. It is "a search-as-a-service cloud solution that gives developers APIs and tools for adding a rich search experience".

The linked data section is built with [dotNetRDF](https://www.dotnetrdf.org/). It is "an Open Source .NET Library for parsing, managing, querying and writing RDF".

The logic section is covered with unit tests. Those are written using [xUnit.net v2](https://xunit.github.io/), which is "a free, open source, community-focused unit testing tool for the .NET Framework".

The demo application is deployed on [Windows Server 2016](https://en.wikipedia.org/wiki/Windows_Server) with [Internet Information Services (IIS)](https://en.wikipedia.org/wiki/Internet_Information_Services).
