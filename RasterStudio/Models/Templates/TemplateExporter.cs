using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class TemplateExporter : INotifyPropertyChanged
    {
        public TemplateExporter()
        {
        }

        public TemplateExporter(TemplateExporter templateExporter)
        {
            this.Name = templateExporter.Name;
            this.Separator = templateExporter.Separator;
            this.Extension = templateExporter.Extension;

            this.ColorSelector = templateExporter.ColorSelector;
            this.LineSelector = templateExporter.LineSelector;
            this.OrientationSelector = templateExporter.OrientationSelector;
            this.Separator = templateExporter.Separator;

            this.PaletteHeader.TextCommand = templateExporter.PaletteHeader.TextCommand;
            this.PaletteFooter.TextCommand = templateExporter.PaletteFooter.TextCommand;

            this.PaletteRaster.HeaderTextCommand = templateExporter.PaletteRaster.HeaderTextCommand;
            this.PaletteRaster.FooterTextCommand = templateExporter.PaletteRaster.FooterTextCommand;
            this.PaletteRaster.ColorTextCommand = templateExporter.PaletteRaster.ColorTextCommand;
        }

        /// <summary>
        /// Template name
        /// </summary>

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private string name;

        public bool IsEditable
        {
            get;
            set;
        }

        public string Extension
        {
            get;
            set;
        }

        public TemplateExporter(TextExporter exporter)
        {
            this.CopyFrom(exporter);
        }

        public void CopyFrom(TextExporter exporter)
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

        public void RaisePropertyChange([CallerMemberName] string propertyName=null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
