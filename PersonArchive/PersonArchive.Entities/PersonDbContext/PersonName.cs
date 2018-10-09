using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonArchive.Entities.PersonDbContext
{
	public class PersonName
	{
		//
		// Id
		//

		[Key]
		[Column(Order = 0)]
		public int PersonNameId { get; set; }

		[Column(Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid PersonNameGuid { get; set; } = Guid.NewGuid();

		//
		// Data
		//

		[Column(Order = 2)]
		public string Prefix { get; set; }

		[Column(Order = 3)]
		public string First { get; set; }

		[Column(Order = 4)]
		public string Middle { get; set; }

		[Column(Order = 5)]
		public string Last { get; set; }

		[Column(Order = 6)]
		public string Suffix { get; set; }

		[Column(Order = 7)]
		// Order names by this name weight.
		// Heaviest name is the last one in use.
		public int NameWeight { get; set; }

		//
		// Meta
		//

		[Column(Order = 8)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeCreated { get; set; } = DateTime.Now;

		[Column(Order = 9)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime TimeLastUpdated { get; set; } = DateTime.Now;

		[Timestamp]
		[Column(Order = 10)]
		public byte[] DataVersion { get; set; }

		//
		// Foreign key
		//

		[Column(Order = 11)]
		public int PersonId { get; set; }
		public Person Person { get; set; }
	}
}