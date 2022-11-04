using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class Tag
    {
        public Tag(string name, GetTagValueHandler getTagValueCallback)
        {
            string textCommand = name.ToLower().Replace(" ","");
            this.Initialize(name, $"@{textCommand}@", getTagValueCallback);
        }

        public Tag(string name, string textCommand, GetTagValueHandler getTagValueCallback)
        {
            this.Initialize(name, textCommand, getTagValueCallback);
        }

        private void Initialize(string name, string textCommand, GetTagValueHandler getTagValueCallback)
        {
            this.Name = name;
            this.TextCommand = textCommand;
            this.getTagValueCallback = getTagValueCallback;
        }

        public string Name
        {
            get;
            private set;
        }

        public string TextCommand
        {
            get;
            private set;
        }

        private GetTagValueHandler getTagValueCallback;

        virtual public string TagValue
        {
            get
            {
                return getTagValueCallback?.Invoke();
            }
        }
    }

    /// <summary>
    /// TagRaster
    /// </summary>
    /// <param name="raster"></param>
    /// <returns></returns>

    public class TagRaster : Tag
    {
        public TagRaster(string name, GetTagRasterHandler getTagRasterCallback) : base(name, null)
        {
            this.getTagRasterCallback = getTagRasterCallback;
        }

        private GetTagRasterHandler getTagRasterCallback;

        public TagRasterParameters Parameters
        {
            get;
            set;
        } = new TagRasterParameters();

        override public string TagValue
        {
            get
            {
                if (this.getTagRasterCallback != null)
                {
                    return this.getTagRasterCallback.Invoke(this.Parameters);
                }

                return String.Empty;
            }
        }
    }

    public class TagRasterParameters
    {
        public AtariRaster raster;
        public int line;
        public bool isLastColor;
        public int nextLine;
        public int nextChangingLine;
        public int diffChangingLine;
        public int lineCounter;
    }

    public delegate string GetTagRasterHandler(TagRasterParameters parameters);
    public delegate string GetTagValueHandler();
}
