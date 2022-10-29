using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public static class TemplateExporterSerializer
    {
        public static string Serialize(TemplateExporter template)
        {
            return JsonConvert.SerializeObject(template);
        }

        public static string SerializeCollection(List<TemplateExporter> template)
        {
            return JsonConvert.SerializeObject(template);
        }

        public static TemplateExporter Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<TemplateExporter>(json);
        }

        public static List<TemplateExporter> DeserializeCollection(string json)
        {
            return JsonConvert.DeserializeObject<List<TemplateExporter>>(json);
        }

    }
}
