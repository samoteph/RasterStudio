using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            this.PaletteHeader.TextCommand = templateExporter.PaletteHeader.TextCommand;
            this.PaletteFooter.TextCommand = templateExporter.PaletteFooter.TextCommand;
            this.Extension = templateExporter.Extension;

            foreach(var rasterExporter in templateExporter.TemplateRasterExporters)
            {
                this.TemplateRasterExporters.Add(new TemplateRasterExporter(rasterExporter));
            }
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
            foreach (var rasterExporter in exporter.TextRasterExporters)
            {
                this.TemplateRasterExporters.Add(new TemplateRasterExporter());
            }

            this.CopyFrom(exporter);
        }

        public void CopyFrom(TextExporter exporter)
        {
            this.PaletteHeader.TextCommand = exporter.PaletteHeaderTagManager.TextCommand;
            this.PaletteFooter.TextCommand = exporter.PaletteFooterTagManager.TextCommand;

            this.TemplateRasterExporters.Clear();

            for (int i=0;i<exporter.TextRasterExporters.Count;i++)
            {
                var rasterExporter = exporter.TextRasterExporters[i];
                var templateRasterExporter = new TemplateRasterExporter();
                
                templateRasterExporter.CopyFrom(rasterExporter);
                
                this.TemplateRasterExporters.Add(templateRasterExporter);
            }
        }

        public void RaisePropertyChange([CallerMemberName] string propertyName=null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CopyTo(Project project)
        {
            var exporter = project.Exporter;

            exporter.PaletteHeaderTagManager.TextCommand = this.PaletteHeader.TextCommand;
            exporter.PaletteFooterTagManager.TextCommand = this.PaletteFooter.TextCommand;

            exporter.TextRasterExporters.Clear();

            // normalement pas possible mais pour gérer les templates pre existant sans collection de Te
            if (this.TemplateRasterExporters.Count == 0)
            {
                exporter.TextRasterExporters.Add(new TextRasterExporter(exporter));
            }
            else
            {
                foreach (var rasterExporter in this.TemplateRasterExporters)
                {
                    var newRasterExporter = new TextRasterExporter(exporter);
                    rasterExporter.CopyTo(newRasterExporter);
                    exporter.TextRasterExporters.Add(newRasterExporter);
                }
            }
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

        public List<TemplateRasterExporter> TemplateRasterExporters
        {
            get;
            private set;
        } = new List<TemplateRasterExporter>();
    }

    public class TemplateRasterExporter
    {
        public TemplateRasterExporter()
        {
        }

        public TemplateRasterExporter(TemplateRasterExporter rasterExporter)
        {
            this.Separator = rasterExporter.Separator;

            this.ColorSelector = rasterExporter.ColorSelector;
            this.LineSelector = rasterExporter.LineSelector;
            this.OrientationSelector = rasterExporter.OrientationSelector;
            this.Separator = rasterExporter.Separator;

            this.PaletteRaster.HeaderTextCommand = rasterExporter.PaletteRaster.HeaderTextCommand;
            this.PaletteRaster.FooterTextCommand = rasterExporter.PaletteRaster.FooterTextCommand;
            this.PaletteRaster.ColorTextCommand = rasterExporter.PaletteRaster.ColorTextCommand;
        }

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

        public void CopyFrom(TextRasterExporter exporter)
        {
            this.ColorSelector = exporter.ColorSelector;
            this.LineSelector = exporter.LineSelector;
            this.OrientationSelector = exporter.OrientationSelector;
            this.Separator = exporter.Separator;

            this.PaletteRaster.HeaderTextCommand = exporter.RasterLineHeaderTagManager.TextCommand;
            this.PaletteRaster.FooterTextCommand = exporter.RasterLineFooterTagManager.TextCommand;
            this.PaletteRaster.ColorTextCommand = exporter.RasterColorTagManager.TextCommand;
        }

        internal void CopyTo(TextRasterExporter exporter)
        {
            exporter.ColorSelector = this.ColorSelector;
            exporter.LineSelector = this.LineSelector;
            exporter.OrientationSelector = this.OrientationSelector;
            exporter.Separator = this.Separator;

            exporter.RasterLineHeaderTagManager.TextCommand = this.PaletteRaster.HeaderTextCommand;
            exporter.RasterLineFooterTagManager.TextCommand = this.PaletteRaster.FooterTextCommand;
            exporter.RasterColorTagManager.TextCommand = this.PaletteRaster.ColorTextCommand;
        }
    }

}
