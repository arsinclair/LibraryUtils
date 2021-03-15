using System.Collections.Generic;

namespace WASM.LibraryUtils
{
    public class Tag
    {
        public TagType Type { get; set; }
        public string RawValue { get; set; }

        public string PrimaryValue { get; set; }
        public List<string> SecondaryValues { get; set; } = new List<string>();

        public Tag(string rawValue, TagType type)
        {
            this.RawValue = rawValue;
            this.Type = type;
        }

        public void Convert()
        {
            PrimaryValue = TagHelpers.NormalizeTag(this.RawValue, NormalizationMethod.Spaced);
            string converted = TagHelpers.NormalizeTag(this.RawValue, NormalizationMethod.NonSpaced);
            if (converted != PrimaryValue && SecondaryValues.Contains(converted) == false)
            {
                SecondaryValues.Add(converted);
            }
        }
    }

    public enum TagType
    {
        Style,
        Artist,
        Title,
        Country,
        ReleaseType,
        Label,
        Other
    }
}