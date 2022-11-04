using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace RasterStudio.Models
{
    public class TextExporter
    {
        private int lineCounter;

        public TagManager PaletteHeaderTagManager
        {
            get;
            private set;
        }

        public TagManager PaletteFooterTagManager
        {
            get;
            private set;
        }

        public ObservableCollection<TextRasterExporter> TextRasterExporters
        {
            get;
            private set;
        } = new ObservableCollection<TextRasterExporter>();

        public Project Project
        {
            get;
            private set;
        }

        public TextExporter(Project project)
        {
            this.Project = project;

            this.PaletteHeaderTagManager = new TagManager();
            this.PaletteHeaderTagManager.AddTag(new Tag("Filename", () => MainPage.Instance.Project.Filename));
            this.PaletteHeaderTagManager.AddTag(new Tag("Project", () => MainPage.Instance.Project.Title));
            this.PaletteHeaderTagManager.AddTag(new Tag("Label", () => MainPage.Instance.Project.Title.Replace(" ","_")));
            this.PaletteHeaderTagManager.AddTag(new Tag("Year", () => DateTime.Now.Year.ToString()));
            this.PaletteHeaderTagManager.AddTag(new Tag("Month", () => DateTime.Now.Month.ToString()));
            this.PaletteHeaderTagManager.AddTag(new Tag("Day", () => DateTime.Now.Day.ToString()));
            this.PaletteHeaderTagManager.AddTag(new Tag("Counter", () => lineCounter.ToString()));

            this.PaletteFooterTagManager = new TagManager(this.PaletteHeaderTagManager);

            // par defaut il y a un element
            this.TextRasterExporters.Add(new TextRasterExporter(this));
        }

        public string GetExportText()
        {
            var rasters = this.Project.Rasters;

            if(rasters == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            StringBuilder builderRaster = new StringBuilder();

            int maxCount = 0;

            // on calcule d'abord les TestRasterExportes pour obtenir lineCouter dans les header/footer
            foreach (var rasterExporter in this.TextRasterExporters)
            {
                rasterExporter.GetExportText(builderRaster);
                maxCount = Math.Max(rasterExporter.LineCouter, maxCount);
            }

            this.lineCounter = maxCount;

            // Affchage du header
            BuilderAppendLine(builder, this.PaletteHeaderTagManager);

            builder.Append(builderRaster);

            BuilderAppendLine(builder, this.PaletteFooterTagManager);

            return builder.ToString();
        }

        private void BuilderAppendLine(StringBuilder builder, TagManager tagRasterManager)
        {
            string text = tagRasterManager.ReplaceText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }
        }

        internal List<AtariRaster> GetUsedRasters()
        {
            List<AtariRaster> selectedRasters = new List<AtariRaster>();

            foreach (var raster in this.Project.Rasters)
            {
                if (MainPage.Instance.IsRasterColorModified(raster.ColorIndex) == true)
                {
                    selectedRasters.Add(raster);
                }
            }

            return selectedRasters;
        }
    }

    public enum LineSelector
    {
        All,
        Changing,
        LineZero,
        ChangingWithoutLast,
        ChangingLastLine,
    }

    public enum ColorSelector
    {
        All,
        Used,
        Changing
    }

    public enum OrientationSelector
    {
        Vertical,
        Horizontal,
    }
}
