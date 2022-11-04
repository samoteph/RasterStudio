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

        /// <summary>
        /// TagManager
        /// </summary>

        public TagManager()
        {

        }

        /// <summary>
        /// TagManager
        /// </summary>

        public TagManager(TagManager tagManager)
        {
            // ce qui nous interesse c'est d'avoir un OriginalText different le reste peut rester pareil
            this.tags = tagManager.tags;
        }

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

        public string TextCommand
        {
            get;
            set;
        }

        public string ReplaceText()
        {
            StringBuilder builder = new StringBuilder(this.TextCommand);

            foreach(var tagName in tags.Keys)
            {
                var tag = tags[tagName];
                var tagValue = tag.TagValue;
                builder.Replace(tagName, tagValue);
            }

            return builder.ToString();
        }
    }

    public class TagRasterManager : TagManager
    {
        /// <summary>
        /// TagManager
        /// </summary>

        public TagRasterManager()
        {

        }

        /// <summary>
        /// TagManager
        /// </summary>

        public TagRasterManager(TagRasterManager tagManager) : base(tagManager)
        {
        }

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
                        
                        tagRaster.Parameters.raster = value;
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

                        tagRaster.Parameters.line = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int line;

        public bool IsLastColor
        {
            get
            {
                return this.isLastColor;
            }

            set
            {
                if (isLastColor != value)
                {
                    this.isLastColor = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Parameters.isLastColor = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private bool isLastColor = false;

        public int NextLine
        {
            get
            {
                return this.nextLine;
            }

            set
            {
                if (nextLine != value)
                {
                    this.nextLine = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Parameters.nextLine = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int nextLine = 0;

        public int NextChangingLine
        {
            get
            {
                return this.nextChangingLine;
            }

            set
            {
                if (nextChangingLine != value)
                {
                    this.nextChangingLine = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Parameters.nextChangingLine = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int nextChangingLine = 0;

        public int DiffChangingLine
        {
            get
            {
                return this.diffChangingLine;
            }

            set
            {
                if (diffChangingLine != value)
                {
                    this.diffChangingLine = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Parameters.diffChangingLine = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int diffChangingLine = 0;

        public int LineCounter
        {
            get
            {
                return this.lineCounter;
            }

            set
            {
                if (lineCounter != value)
                {
                    this.lineCounter = value;

                    foreach (var tag in this.GetTags())
                    {
                        var tagRaster = (TagRaster)tag;

                        tagRaster.Parameters.lineCounter = value;
                    }

                    if (this.raster != null)
                    {
                        this.ReplaceText();
                    }
                }
            }
        }

        private int lineCounter = 0;
    }
}
