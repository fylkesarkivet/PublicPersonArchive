using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonArchive.Entities.PersonDbContext
{
	public class Person
	{
		//
		// Id
		//

		[Key, Column(Order = 0)]
		public int PersonId { get; set; }

		[Column(Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid PersonGuid { get; set; } = Guid.NewGuid();

		//
		// Data
		//

		[Column(Order = 2)]
		public PersonGender Gender { get; set; }

		//
		// Meta
		//

		[Column(Order = 3)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeCreated { get; set; } = DateTime.Now;

		[Column(Order = 4)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeLastUpdated { get; set; } = DateTime.Now;

		[Timestamp]
		[Column(Order = 5)]
		public byte[] DataVersion { get; set; }

		//
		// Navigation properties
		//

		public ICollection<PersonFluffyDate> FluffyDates { get; set; }
		public ICollection<PersonName> Names { get; set; }
		public ICollection<PersonDescription> Descriptions { get; set; }
	}
}