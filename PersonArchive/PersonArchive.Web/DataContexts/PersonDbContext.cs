using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.DataContexts
{
	public class PersonDbContext : DbContext
	{
		public PersonDbContext(DbContextOptions options)
			: base(options)
		{
		}

		public DbSet<Person> Persons { get; set; }
		public DbSet<PersonName> PersonNames { get; set; }
		public DbSet<PersonDescription> PersonDescriptions { get; set; }
		public DbSet<PersonFluffyDate> PersonFluffyDates { get; set; }

		/// <summary>
		/// This method uses the Fluent API.
		/// </summary>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//
			// Class names
			//

			modelBuilder.Entity<Person>().ToTable("Person");
			modelBuilder.Entity<PersonName>().ToTable("PersonName");
			modelBuilder.Entity<PersonDescription>().ToTable("PersonDescription");
			modelBuilder.Entity<PersonFluffyDate>().ToTable("PersonFluffyDate");

			//
			// Indexes
			//

			modelBuilder
				.Entity<Person>()
				.HasIndex(x => x.PersonGuid)
				.HasName("IX_Person_PersonGuid")
				.IsUnique();

			modelBuilder
				.Entity<PersonName>()
				.HasIndex(x => x.PersonNameGuid)
				.HasName("IX_PersonName_PersonNameGuid")
				.IsUnique();

			//
			// Person
			//

			modelBuilder
				.Entity<Person>()
				.Property(x => x.Gender)
				.HasDefaultValue(PersonGender.Undefined);
		}
	}
}