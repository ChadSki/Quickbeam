using Blamite.Injection;

namespace Quickbeam.Models.Cache
{
	public class ExtractingTagEntry
	{
		public ExtractingTagEntry(bool extract, ExtractedTag extractedTag)
		{
			Extract = extract;
			ExtractedTag = extractedTag;
		}

		public bool Extract { get; set; }

		public ExtractedTag ExtractedTag { get; set; }
	}
}
