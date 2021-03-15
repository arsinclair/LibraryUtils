using System;

namespace WASM.LibraryUtils
{
    public static class TagHelpers
    {
        public static string NormalizeTag(string tag, NormalizationMethod method)
        {
            string output = string.Empty;
            if (!string.IsNullOrEmpty(tag))
            {
                output = tag.Trim().TrimStart('#').Trim();
                switch (method)
                {
                    case NormalizationMethod.NonSpaced: output = NormalizeNonSpaced(output); break;
                    case NormalizationMethod.Spaced: output = NormalizeSpaced(output); break;
                    default: throw new NotImplementedException();
                }
            }
            return "#" + output;
        }


        private static string NormalizeSpaced(string tag)
        {
            return NormalizeBase(tag)
                .Replace(" ", "_")
                .Replace("-", "_");
        }

        private static string NormalizeNonSpaced(string tag)
        {
            return NormalizeBase(tag)
                .Replace(" ", "")
                .Replace("-", "");
        }

        private static string NormalizeBase(string tag)
        {
            return tag
                .Replace(".", "")
                .Replace("'", "")
                .Replace("`", "")
                .Replace(",", "")
                .Replace("&", "And");
        }
    }

    public enum NormalizationMethod
    {
        NonSpaced,
        Spaced
    }
}