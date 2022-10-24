using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class TagManager
    {
        Dictionary<string, Tag> tags = new Dictionary<string, Tag>();

        public List<Tag> GetTags()
        {
            return tags.Values.ToList();
        }

        public void AddTag(Tag tag)
        {
            this.tags.Add(tag.TextCommand, tag);
        }

        public string ReplaceText(string text)
        {
            StringBuilder builder = new StringBuilder(text);

            foreach(var tagName in tags.Keys)
            {
                builder.Replace(tagName, tags[tagName].TagValue);
            }

            return builder.ToString();
        }
    }
}
