using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonArchive.Entities.PersonDbContext
{
	public class PersonFluffyDate
	{
		//
		// Id
		//

		[Key]
		[Column(Order = 0)]
		public int PersonFluffyDateId { get; set; }

		//
		// Data
		//

		[Column(Order = 1)]
		public int? Year { get; set; }

		[Column(Order = 2)]
		public int? Month { get; set; }

		[Column(Order = 3)]
		public int? Day { get; set; }

		[Column(Order = 4)]
		public PersonFluffyDateType Type { get; set; }

		//
		// Meta
		//

		[Column(Order = 5)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeCreated { get; set; } = DateTime.Now;

		[Column(Order = 6)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeLastUpdated { get; set; } = DateTime.Now;

		[Timestamp]
		[Column(Order = 7)]
		public byte[] DataVersion { get; set; }

		//
		// Foreign key
		//

		[Column(Order = 8)]
		public int PersonId { get; set; }
		public Person Person { get; set; }
	}
}