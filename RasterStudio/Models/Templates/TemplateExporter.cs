using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class TemplateExporter
    {
        public TemplateExporter()
        {
        }

        /// <summary>
        /// Template name
        /// </summary>

        public string Name
        {
            get;
            set;
        }

        public bool IsEditable
        {
            get;
            set;
        }

        public TemplateExporter(TextExporter exporter)
        {
            this.ColorSelector = exporter.ColorSelector;
            this.LineSelector = exporter.LineSelector;
            this.OrientationSelector = exporter.OrientationSelector;
            this.Separator = exporter.Separator;

            this.PaletteHeader.TextCommand = exporter.PaletteHeaderTagManager.TextCommand;
            this.PaletteFooter.TextCommand = exporter.PaletteFooterTagManager.TextCommand;

            this.PaletteRaster.HeaderTextCommand = exporter.RasterLineHeaderTagManager.TextCommand;
            this.PaletteRaster.FooterTextCommand = exporter.RasterLineFooterTagManager.TextCommand;
            this.PaletteRaster.ColorTextCommand = exporter.RasterColorTagManager.TextCommand;
        }

        public void CopyTo(Project project)
        {
            var exporter = project.Exporter;

            exporter.ColorSelector = this.ColorSelector;
            exporter.LineSelector = this.LineSelector;
            exporter.OrientationSelector = this.OrientationSelector;
            exporter.Separator = this.Separator;

            exporter.PaletteHeaderTagManager.TextCommand = this.PaletteHeader.TextCommand;
            exporter.PaletteFooterTagManager.TextCommand = this.PaletteFooter.TextCommand;

            exporter.RasterLineHeaderTagManager.TextCommand = this.PaletteRaster.HeaderTextCommand;
            exporter.RasterLineFooterTagManager.TextCommand = this.PaletteRaster.FooterTextCommand;
            exporter.RasterColorTagManager.TextCommand = this.PaletteRaster.ColorTextCommand;
        }

        public TemplateTagManager PaletteHeader
        {
            get;
            set;
        } = new TemplateTagManager();

        public TemplateTagManager PaletteFooter
        {
            get;
            set;
        } = new TemplateTagManager();

        public TemplateTagRasterManager PaletteRaster
        {
            get;
            set;
        } = new TemplateTagRasterManager();

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
    }
}
