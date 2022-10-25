using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class TextExporter
    {
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

        public TagRasterManager RasterLineHeaderTagManager
        {
            get;
            private set;
        }

        public TagRasterManager RasterLineFooterTagManager
        {
            get;
            private set;
        }

        public TagRasterManager RasterColorTagManager
        {
            get;
            private set;
        }

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
            this.PaletteHeaderTagManager.AddTag(new Tag("Year", () => DateTime.Now.Year.ToString()));
            this.PaletteHeaderTagManager.AddTag(new Tag("Month", () => DateTime.Now.Month.ToString()));
            this.PaletteHeaderTagManager.AddTag(new Tag("Day", () => DateTime.Now.Day.ToString()));

            this.PaletteFooterTagManager = new TagManager(this.PaletteHeaderTagManager);

            this.RasterLineHeaderTagManager = new TagRasterManager();
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("Raster Line", (line) => line.ToString()));

            this.RasterLineFooterTagManager = new TagRasterManager(this.RasterLineHeaderTagManager);

            this.RasterColorTagManager = new TagRasterManager();
            this.RasterColorTagManager.AddTag(new TagRaster("Color Address", (line,raster) => raster.ColorAddress));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Index", (line, raster) => raster.ColorIndex.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Hexa Value", (line, raster) => raster.Colors[line].Color.ToString("x4")));
            this.RasterColorTagManager.AddTag(new TagRaster("Raster Line", (line, raster) => line.ToString()));

        }

        public string GetExportText()
        {
            StringBuilder builder = new StringBuilder();

            string text = this.PaletteHeaderTagManager.ReplaceText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }

            var rasters = this.Project.Rasters;

            int lineCount = rasters[0].Colors.Length;

            List<AtariRaster> usedRasters = new List<AtariRaster>(16);

            foreach (var raster in rasters)
            {
                if( MainPage.Instance.IsRasterColorModified(raster.ColorIndex) == true)
                {
                    usedRasters.Add(raster);
                }
            }
            
            for (int line = 0; line < lineCount; line++)
            {
                this.RasterLineHeaderTagManager.Line = line;
                this.RasterLineFooterTagManager.Line = line;
                this.RasterColorTagManager.Line = line;

                text = this.RasterLineHeaderTagManager.ReplaceText();

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    builder.AppendLine(text);
                }

                foreach (var raster in usedRasters)
                {
                    this.RasterColorTagManager.Raster = raster;

                    text = this.RasterColorTagManager.ReplaceText();

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        builder.AppendLine(text);
                    }

                }

                text = this.RasterLineFooterTagManager.ReplaceText();

                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    builder.AppendLine(text);
                }
            }

            text = this.PaletteFooterTagManager.ReplaceText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }

            return builder.ToString();
        }
    }
}
