using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;

namespace WASM.LibraryUtils
{
    public class ReleaseHelper
    {
        public const string TextblockSeparator = "————————————————";
        public const string TagSeparator = " | ";
        public List<Tag> Tags;
        private List<string> UsedTags;
        private Release _release;
        private MusicBrainzClient client;

        public ReleaseHelper(Release release, MusicBrainzClient musicBrainzClient)
        {
            Tags = new List<Tag>();
            UsedTags = new List<string>();
            _release = release;
            client = musicBrainzClient;
        }

        public List<string> GetStyles()
        {
            IEnumerable<Tag> remoteTags = _release.Tags.Select(x => new Tag(x.Name, TagType.Style));
            Tags.AddRange(remoteTags);
            return Tags.Where(x => x.Type == TagType.Style).Select(x => x.RawValue).Distinct().ToList();
        }

        public string GetYear()
        {
            // TODO: There's a chance that this Year is different from the First Publish Date (in case of reissues). If it is different, we should use the original year as the primary year, and the re-issue year should be in the brackets, e.g. 2008 (1969)
            if (!string.IsNullOrEmpty(_release.Date) && _release.Date.Length >= 4)
            {
                return _release.Date.Substring(0, 4);
            }
            return string.Empty;
        }

        public async Task<string> GetArtistCountry()
        {
            List<string> countries = new List<string>();

            foreach (var artistCredit in _release.Credits)
            {
                var artist = await client.Artists.GetAsync(artistCredit.Artist.Id);
                if (artist.Area != null && countries.Contains(artist.Area.Name.Trim()) == false)
                {
                    // TODO: Possibility of getting a city instead of a country here. It is a BUGG!
                    countries.Add(artist.Area.Name.Trim());
                    Tags.Add(new Tag(artist.Area.Name.Trim(), TagType.Country));
                }
            }
            return string.Join(", ", countries);
        }

        public string GetCatalogueNumber()
        {
            List<string> catalogNumbers = _release.Labels.Where(x => x.Label.Name != "[no label]" && x.Label != null && x.CatalogNumber != null).Select(x => x.CatalogNumber.Trim()).ToList();

            if (catalogNumbers != null && catalogNumbers.Count > 0)
            {
                return string.Join(", ", catalogNumbers.Distinct());
            }
            return string.Empty;
        }

        public string GetLabel()
        {
            List<string> labels = _release.Labels.Where(x => x.Label.Name != "[no label]").Select(x => x.Label.Name.Trim()).ToList();
            if (labels != null && labels.Count > 0)
            {
                Tags.AddRange(labels.Select(x => new Tag(x, TagType.Label)));
                return string.Join(", ", labels);
            }
            return string.Empty;
        }

        public string GetTitle()
        {
            string title = _release.Title.Trim();
            Tags.Add(new Tag(title, TagType.Title));
            return title;
        }

        public string GetArtist()
        {
            if (_release.Credits != null && _release.Credits.Any())
            {
                IEnumerable<Tag> artistTags = _release.Credits.Select(x => new Tag(x.Name.Trim(), TagType.Artist));
                Tags.AddRange(artistTags);
                return string.Join("", _release.Credits.Select(x => x.Name + x.JoinPhrase));
            }
            return string.Empty;
        }

        public string GenerateAlbumHeadline(string artist, string title)
        {
            return $"{artist} — {title}";
        }

        public string GenerateAlbumBottomline(string artistCountry, string year)
        {
            return string.Join(", ", new[] { artistCountry, year });
        }

        public string GenerateHeaderStyleTags()
        {
            var output = new List<string>();

            foreach (var tag in Tags)
            {
                if (tag.Type == TagType.Style)
                {
                    tag.Convert();
                    UsedTags.Add(tag.PrimaryValue);
                    output.Add(tag.PrimaryValue);
                }
            }

            return string.Join(TagSeparator, output);
        }

        public string GenerateReleaseType()
        {
            List<string> output = new List<string>();
            if (!string.IsNullOrEmpty(_release.ReleaseGroup.PrimaryType))
            {
                output.Add(_release.ReleaseGroup.PrimaryType);
            }
            if (_release.ReleaseGroup.SecondaryTypes.Any())
            {
                output.AddRange(_release.ReleaseGroup.SecondaryTypes);
            }
            output = output.Distinct().ToList();
            foreach (var outputItem in output)
            {
                if (Tags.Any(x => x.Type == TagType.ReleaseType && x.RawValue == outputItem))
                {
                    continue;
                }
                Tags.Add(new Tag(outputItem.Trim(), TagType.ReleaseType));
            }
            Tags.AddRange(output.Select(x => new Tag(x, TagType.ReleaseType)));
            return string.Join(", ", output);
        }

        public string CalculateDuration(Release release)
        {
            int sum = 0;

            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks)
                {
                    if (track.Length == null)
                    {
                        return "-1";
                    }
                    else
                    {
                        sum += (int)track.Length;
                    }
                }
            }
            return TimeSpan.FromMilliseconds(sum).ToString(@"hh\:mm\:ss");
        }

        public string GenerateFooterStyleTags()
        {
            var output = new List<string>();

            foreach (var tag in Tags)
            {
                if (tag.Type == TagType.Style)
                {
                    foreach (var tagToAdd in tag.SecondaryValues)
                    {
                        if (UsedTags.Contains(tagToAdd) == false)
                        {
                            output.Add(tagToAdd);
                        }
                    }
                }
            }

            return string.Join(" ", output);
        }

        public string GenerateExtraTags()
        {
            List<string> output = new List<string>();

            foreach (var tag in Tags)
            {
                if (tag.Type != TagType.Style)
                {
                    tag.Convert();
                    output.Add(tag.PrimaryValue);
                    output.AddRange(tag.SecondaryValues);
                }
            }
            return string.Join(" ", output.Distinct());
        }

        public void UpdateStyleTags(List<string> updatedTags)
        {
            foreach (var newTag in updatedTags)
            {
                if (Tags.Any(x => x.Type == TagType.Style && x.RawValue == newTag) == false)
                {
                    Tags.Add(new Tag(newTag, TagType.Style));
                }
            }
            List<Tag> toBeDeleted = new List<Tag>();
            foreach (var tag in Tags)
            {
                if (tag.Type == TagType.Style && updatedTags.Contains(tag.RawValue) == false)
                {
                    toBeDeleted.Add(tag);
                }
            }
            if (toBeDeleted.Any())
            {
                foreach (var tbdItem in toBeDeleted)
                {
                    Tags.Remove(tbdItem);
                }
            }
        }
    }
}