namespace PersonArchive.Logic.Validate
{
	public class PersonName
	{
		public PersonName(
			string prefix,
			string first,
			string middle,
			string last,
			string suffix,
			int? nameWeight)
		{
			Prefix = prefix;
			First = first;
			Middle = middle;
			Last = last;
			Suffix = suffix;
			NameWeight = nameWeight;
		}

		public string Prefix { get; set; }
		public string First { get; set; }
		public string Middle { get; set; }
		public string Last { get; set; }
		public string Suffix { get; set; }
		public int? NameWeight { get; set; }
	}
}