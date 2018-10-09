using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonArchive.Entities.PersonDbContext
{
	public class PersonDescription
	{
		//
		// Id
		//

		[Key]
		[Column(Order = 0)]
		public int PersonDescriptionId { get; set; }

		//
		// Data
		//

		[Column(Order = 1)]
		public string Description { get; set; }

		[Column(Order = 2)]
		public PersonDescriptionType Type { get; set; }

		//
		// Meta
		//

		[Column(Order = 3)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeCreated { get; set; } = DateTime.Now;

		[Column(Order = 4)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeLastUpdated { get; set; } = DateTime.Now;

		[Timestamp] [Column(Order = 5)]
		public byte[] DataVersion { get; set; }

		//
		// Foreign key
		//

		[Column(Order = 6)]
		public int PersonId { get; set; }

		public Person Person { get; set; }
	}
}