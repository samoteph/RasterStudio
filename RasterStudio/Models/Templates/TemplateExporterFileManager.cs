using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static System.Net.WebRequestMethods;

namespace RasterStudio.Models.Templates
{
    public class TemplateExporterFileManager
    {
        public ObservableCollection<TemplateExporter> CompleteTemplateExporters
        {
            get;
            private set;
        } = new ObservableCollection<TemplateExporter>();

        public List<TemplateExporter> WritableTemplateExporters
        {
            get;
            private set;
        } = new List<TemplateExporter>();


        public async Task LoadCompleteTemplateExportersAsync()
        {
            string json = null;

            // ReadOnly template
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Templates/DefaultTemplate.json"));

            using (var inputStream = await file.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                json = streamReader.ReadToEnd();
            }

            var readOnlyTemplateExporters = TemplateExporterSerializer.DeserializeCollection(json);

            // Writable template
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            file = null;

            try
            {
                file = await storageFolder.GetFileAsync("editableTemplateExporters.json");
            }
            catch
            {
                file = null;
            }

            if (file != null)
            {
                using (var inputStream = await file.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    json = await streamReader.ReadToEndAsync();
                }

                this.WritableTemplateExporters = TemplateExporterSerializer.DeserializeCollection(json);
            }

            if(this.WritableTemplateExporters == null)
            {
                this.WritableTemplateExporters = new List<TemplateExporter>();
            }

            this.CompleteTemplateExporters = new ObservableCollection<TemplateExporter>(readOnlyTemplateExporters);

            foreach (var templateExporter in this.WritableTemplateExporters)
            {
                this.CompleteTemplateExporters.Add(templateExporter);
            }
        }

        public async Task SaveWriteableTemplateExporterAsync(TemplateExporter exporter)
        {
            if(exporter.IsEditable == false)
            {
                throw new Exception("This template is not editable so you can not save it!");
            }

            if(this.WritableTemplateExporters.Contains(exporter) == false)
            {
                this.WritableTemplateExporters.Add(exporter);
                this.CompleteTemplateExporters.Add(exporter);
            }

            string json = TemplateExporterSerializer.SerializeCollection(this.WritableTemplateExporters);

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            try
            {
                file = await storageFolder.CreateFileAsync("editableTemplateExporters.json", CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                file = null;
            }

            if (file != null)
            {
                using (var inputStream = await file.OpenStreamForWriteAsync())
                using (var streamWriter = new StreamWriter(inputStream))
                {
                    await streamWriter.WriteAsync(json);
                }
            }
        }

        public async Task DeleteWriteableTemplateExporterAsync(TemplateExporter exporter)
        {
            if (exporter.IsEditable == false)
            {
                throw new Exception("This template is not editable so you can not delete it!");
            }

            this.WritableTemplateExporters.Remove(exporter);
            this.CompleteTemplateExporters.Remove(exporter);

            string json = TemplateExporterSerializer.SerializeCollection(this.WritableTemplateExporters);

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            try
            {
                file = await storageFolder.CreateFileAsync("editableTemplateExporters.json", CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                file = null;
            }

            if (file != null)
            {
                using (var inputStream = await file.OpenStreamForWriteAsync())
                using (var streamWriter = new StreamWriter(inputStream))
                {
                    await streamWriter.WriteAsync(json);
                }
            }
        }
    }
}
