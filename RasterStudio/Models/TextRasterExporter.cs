using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class TextRasterExporter
    {
        private Project project;
        private TextExporter exporter;

        public TextRasterExporter(TextExporter exporter)
        {
            this.project = exporter.Project;
            this.exporter = exporter;

            this.RasterLineHeaderTagManager = new TagRasterManager();
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("#Line", (parameter) => parameter.line.ToString()));
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("#Next Line", (parameter) => parameter.nextLine.ToString()));
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("#Next Changing Line", (parameter) => parameter.nextChangingLine.ToString()));
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("#Diff Changing Line", (parameter) => parameter.diffChangingLine.ToString()));
            this.RasterLineHeaderTagManager.AddTag(new TagRaster("Counter", (parameter) => parameter.lineCounter.ToString()));

            this.RasterLineFooterTagManager = new TagRasterManager(this.RasterLineHeaderTagManager);

            this.RasterColorTagManager = new TagRasterManager();
            this.RasterColorTagManager.AddTag(new TagRaster("Color Address", (parameter) => parameter.raster.ColorAddress));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Index", (parameter) => parameter.raster.ColorIndex.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("Color Hexa Value", (parameter) => parameter.raster.Colors[parameter.line].Color.ToString("x4")));
            this.RasterColorTagManager.AddTag(new TagRaster("#Line", (parameter) => parameter.line.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("#Next Line", (parameter) => parameter.nextLine.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("#Next Changing Line", (parameter) => parameter.nextChangingLine.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("#Diff Changing Line", (parameter) => parameter.diffChangingLine.ToString()));
            this.RasterColorTagManager.AddTag(new TagRaster("Separator", (parameter) => parameter.isLastColor ? String.Empty : this.Separator));
            this.RasterColorTagManager.AddTag(new TagRaster("Counter", (parameter) => parameter.lineCounter.ToString()));
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

        public int LineCouter
        {
            get;
            private set;
        }

        public void GetExportText(StringBuilder builder)
        {
            var rasters = this.project.Rasters;

            List<AtariRaster> usedRasters = null;
            List<int> changingLines = new List<int>(200);

            // LineSelector.All
            int lineCount = rasters[0].Colors.Length;

            // la ligne change si une couleur est differente de la precedente
            // On change LineCount si lineSelector.Changing + on remplie lineChanges

            usedRasters = this.exporter.GetUsedRasters();

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

            switch (this.LineSelector)
            {
                case LineSelector.Changing:
                    lineCount = changingLines.Count;
                    break;

                case LineSelector.ChangingWithoutLast:
                    // on retire la dernière ligne mais on conserve quand même dans la collection de changingLine pour que NextLine fonctionne
                    lineCount = changingLines.Count - 1;

                    break;
                case LineSelector.LineZero:
                    lineCount = 1;
                    break;
                case LineSelector.ChangingLastLine:

                    // on ne met que la dernière ligne
                    if (changingLines.Count >= 1)
                    {
                        // on stocke la dernière ligne uniquement
                        changingLines = new List<int>() { changingLines[changingLines.Count - 1] };
                        lineCount = 1;
                    }
                    else
                    {
                        lineCount = 0;
                    }


                    lineCount = 1;
                    break;
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
                        selectedRasters = this.exporter.GetUsedRasters();
                    }
                    else
                    {
                        selectedRasters = usedRasters;
                    }
                    break;
            }

            int oldLine = 0;
            int line = 0;

            this.LineCouter = lineCount;

            for (int i = 0; i < lineCount; i++)
            {

                this.RasterLineHeaderTagManager.LineCounter = i;
                this.RasterLineFooterTagManager.LineCounter = i;
                this.RasterColorTagManager.LineCounter = i;

                oldLine = line;

                switch(this.LineSelector)
                {
                    case LineSelector.Changing:
                    case LineSelector.ChangingWithoutLast:
                    case LineSelector.ChangingLastLine:
                        line = changingLines[i];
                        break;
                    default:
                        line = i;
                        break;
                }

                this.RasterLineHeaderTagManager.Line = line;
                this.RasterLineFooterTagManager.Line = line;
                this.RasterColorTagManager.Line = line;

                this.RasterLineHeaderTagManager.NextLine = line + 1;
                this.RasterLineFooterTagManager.NextLine = line + 1;
                this.RasterColorTagManager.NextLine = line + 1;

                int nextChangingLine = line;
                int diffChangingLine;

                if (changingLines.Count > i + 1)
                {
                    if (LineSelector == LineSelector.LineZero)
                    {
                        nextChangingLine = changingLines[i];
                    }
                    else
                    {
                        nextChangingLine = changingLines[i + 1];
                    }
                }

                diffChangingLine = nextChangingLine - line;

                this.RasterLineHeaderTagManager.NextChangingLine = nextChangingLine;
                this.RasterLineFooterTagManager.NextChangingLine = nextChangingLine;
                this.RasterColorTagManager.NextChangingLine = nextChangingLine;

                this.RasterLineHeaderTagManager.DiffChangingLine = diffChangingLine;
                this.RasterLineFooterTagManager.DiffChangingLine = diffChangingLine;
                this.RasterColorTagManager.DiffChangingLine = diffChangingLine;

                BuilderAppendWithOrientation(builder, this.RasterLineHeaderTagManager);

                int lastColorIndex = 0;

                // Determiner la dernière ligne (relou de faire tout ca en avance mais pas le choix car on ne peut pas le faire pendant l'affichage (effectué en dessous)
                if (ColorSelector == ColorSelector.Changing)
                {
                    for (int r = 0; r < selectedRasters.Count; r++)
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

                    this.RasterColorTagManager.Raster = raster;

                    BuilderAppendWithOrientation(builder, this.RasterColorTagManager);

                }

                BuilderAppendWithOrientation(builder, this.RasterLineFooterTagManager);

                if (this.OrientationSelector == OrientationSelector.Horizontal && builder.Length > 0)
                {
                    builder.AppendLine();
                }
            }
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
    }
}
