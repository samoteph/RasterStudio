using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static System.Net.Mime.MediaTypeNames;

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

        public LineSelector LineSelector
        {
            get; set;
        } = LineSelector.All;

        public ColorSelector ColorSelector
        {
            get; set;
        } = ColorSelector.All;

        public OrientationSelector OrientationSelector
        {
            get;
            set;
        } = OrientationSelector.Vertical;

        public string Separator
        {
            get;
            set;
        } = ",";

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
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("Raster Line", (parameter) => parameter.line.ToString()));

            this.RasterLineFooterTagManager = new TagRasterManager(this.RasterLineHeaderTagManager);

            this.RasterColorTagManager = new TagRasterManager();
            this.RasterColorTagManager.AddTag(new TagRaster("Color Address", (parameter) => parameter.raster.ColorAddress));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Index", (parameter) => parameter.raster.ColorIndex.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Hexa Value", (parameter) => parameter.raster.Colors[parameter.line].Color.ToString("x4")));
            this.RasterColorTagManager.AddTag(new TagRaster("Raster Line", (parameter) => parameter.line.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("Separator", (parameter) => parameter.isLastColor ? String.Empty : this.Separator));
        }

        public string GetExportText()
        {
            var rasters = this.Project.Rasters;

            if(rasters == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();

            // Affchage du header

            BuilderAppendLine(builder, this.PaletteHeaderTagManager);
            
            List<AtariRaster> usedRasters = null;
            List<int> changingLines = new List<int>(200);
            
            // LineSelector.All
            int lineCount = rasters[0].Colors.Length;

            // la ligne change si une couleur est differente de la precedente
            // On change LineCount si lineSelector.Changing + on remplie lineChanges
            if (this.LineSelector == LineSelector.Changing)
            {
                usedRasters = this.GetUsedRasters();

                for (int l = 0; l < lineCount; l++)
                {
                    if (l > 0)
                    {
                        for (int c = 0; c < usedRasters.Count; c++)
                        {
                            var usedRaster = usedRasters[c];
                            var oldColor = usedRaster.Colors[l - 1].Color;
                            var color = usedRaster.Colors[l].Color;

                            if (color != oldColor)
                            {
                                changingLines.Add(l);
                                break;
                            }
                        }
                    }
                }

                if (LineSelector == LineSelector.Changing)
                {
                    lineCount = changingLines.Count;
                }
            }

            List<AtariRaster> selectedRasters = new List<AtariRaster>(16);

            switch (this.ColorSelector)
            {
                // Toutes les couleurs
                case ColorSelector.All:
                    selectedRasters = rasters.ToList();
                    break;
                // Seulement les couleurs utilisant un raster à l'écran
                case ColorSelector.Changing:
                case ColorSelector.Used:

                    if (usedRasters == null)
                    {
                        selectedRasters = this.GetUsedRasters();
                    }
                    else
                    {
                        selectedRasters = usedRasters;
                    }
                    break;
            }

            int oldLine = 0;
            int line = 0;
            
            for (int i = 0; i < lineCount; i++)
            {
                oldLine = line;
                line = i;

                if(changingLines.Count > 0)
                {
                    line = changingLines[i];
                }

                this.RasterLineHeaderTagManager.Line = line;
                this.RasterLineFooterTagManager.Line = line;
                this.RasterColorTagManager.Line = line;

                BuilderAppendWithOrientation(builder, this.RasterLineHeaderTagManager);

                int lastColorIndex = 0;

                // Determiner la dernière ligne (relou de faire tout ca en avance mais pas le choix car on ne peut pas le faire pendant l'affichage (effectué en dessous)
                if (ColorSelector == ColorSelector.Changing)
                {
                    for ( int r = 0; r < selectedRasters.Count; r++)
                    {
                        if (ColorSelector == ColorSelector.Changing)
                        {
                            if (selectedRasters.Count > 1)
                            {
                                var oldColor = selectedRasters[r].Colors[oldLine].Color;
                                var newColor = selectedRasters[r].Colors[line].Color;

                                if (oldColor == newColor)
                                {
                                    continue;
                                }
                            }
                        }

                        lastColorIndex = r;
                    }
                }
                else
                {
                    lastColorIndex = selectedRasters.Count - 1;
                }

                // Affichage des rasters
                for (int r = 0; r < selectedRasters.Count; r++)
                {
                    bool isLastColor = r == lastColorIndex;

                    this.RasterLineHeaderTagManager.IsLastColor = isLastColor;
                    this.RasterLineFooterTagManager.IsLastColor = isLastColor;
                    this.RasterColorTagManager.IsLastColor = isLastColor;

                    var raster = selectedRasters[r];

                    if(ColorSelector == ColorSelector.Changing)
                    {
                        if(selectedRasters.Count > 1)
                        {
                            var oldColor = selectedRasters[r].Colors[oldLine].Color;
                            var newColor = selectedRasters[r].Colors[line].Color;
                        
                            if(oldColor == newColor)
                            {
                                continue;
                            }
                        }
                    }

                    this.RasterColorTagManager.Raster = raster;

                    BuilderAppendWithOrientation(builder, this.RasterColorTagManager);

                }

                BuilderAppendWithOrientation(builder, this.RasterLineFooterTagManager);
            
                if(this.OrientationSelector == OrientationSelector.Horizontal)
                {
                    builder.AppendLine();
                }
            }

            BuilderAppendLine(builder, this.PaletteFooterTagManager);

            return builder.ToString();
        }

        private void BuilderAppendWithOrientation(StringBuilder builder, TagRasterManager tagRasterManager)
        {
            string text = tagRasterManager.ReplaceText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                if (this.OrientationSelector == OrientationSelector.Horizontal)
                {
                    builder.Append(text);
                }
                else
                {
                    builder.AppendLine(text);
                }
            }
        }

        private void BuilderAppendLine(StringBuilder builder, TagManager tagRasterManager)
        {
            string text = tagRasterManager.ReplaceText();

            if (string.IsNullOrWhiteSpace(text) == false)
            {
                builder.AppendLine(text);
            }
        }

        private List<AtariRaster> GetUsedRasters()
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
        Changing
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
