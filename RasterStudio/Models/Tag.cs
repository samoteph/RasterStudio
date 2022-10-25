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
        public TagRaster(string name, GetTagLineValueHandler getTagLineValueCallback) : base(name, null)
        {
            this.getTagLineValueCallback = getTagLineValueCallback;
        }

        public TagRaster(string name, GetTagRasterLineHandler getTagRasterLineCallback) : base(name, null)
        {
            this.getTagRasterLineCallback = getTagRasterLineCallback;
        }


        private GetTagRasterLineHandler getTagRasterLineCallback;
        private GetTagLineValueHandler getTagLineValueCallback;

        public AtariRaster Raster
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        override public string TagValue
        {
            get
            {
                if (this.getTagRasterLineCallback != null)
                {
                    return this.getTagRasterLineCallback.Invoke(this.Line, this.Raster);
                }
                else if (this.getTagLineValueCallback != null)
                {
                    return this.getTagLineValueCallback.Invoke(this.Line);
                }

                return String.Empty;
            }
        }
    }

    public delegate string GetTagRasterLineHandler(int line, AtariRaster raster);
    public delegate string GetTagLineValueHandler(int line);
    public delegate string GetTagValueHandler();
}
