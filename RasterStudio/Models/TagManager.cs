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
        List<Tag> listTags = null;

        public List<Tag> GetTags()
        {
            if (listTags == null)
            {
                listTags = tags.Values.ToList();
            }

            return listTags;
        }

        public void AddTag(Tag tag)
        {
            this.tags.Add(tag.TextCommand, tag);
        }

        public string OriginalText
        {
            get;
            set;
        }

        public string ReplaceText()
        {
            StringBuilder builder = new StringBuilder(this.OriginalText);

            foreach(var tagName in tags.Keys)
            {
                builder.Replace(tagName, tags[tagName].TagValue);
            }

            return builder.ToString();
        }
    }

    public class TagRasterManager : TagManager
    {
        public AtariRaster Raster
        {
            get
            {
                return this.raster;
            }

            set
            {
                if(raster != value)
                {
                    this.raster = value;

                    foreach(var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;
                        
                        tagRaster.Raster = value;
                    }

                    this.ReplaceText();
                }
            }
        }

        private AtariRaster raster;

        public int Line
        {
            get
            {
                return this.line;
            }

            set
            {
                if (line != value)
                {
                    this.line = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Line = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int line;
    }
}
