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

        public string TagValue
        {
            get
            {
                return getTagValueCallback?.Invoke();
            }
        }
    }

    public delegate string GetTagValueHandler();
}
