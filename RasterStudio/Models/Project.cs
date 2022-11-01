using Atari.Images;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.IO.Compression;
using System.ComponentModel;
//using System.Text.Json.Serialization;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Security.Principal;
using Windows.UI.Xaml.Input;

namespace RasterStudio.Models
{
    public class Project
    {
        public Project()
        {
           this.Exporter = new TextExporter(this);
        }

        [JsonIgnore]
        public TextExporter Exporter
        {
            get;
        }

        [JsonIgnore]
        public string Filename
        {
            get
            {
                if(this.filename == null)
                {
                    this.filename = this.title + ".hbl";
                }
                
                return this.filename;
            }

            set
            {
                if(this.filename != value)
                {
                    this.filename = value;
                    this.FilenameWithoutExtension = Path.GetFileNameWithoutExtension(value)?.ToUpper();
                }
            }
        }

        private string filename = null;

        [JsonIgnore]
        public string FilenameWithoutExtension
        {
            get;
            private set;
        } = "New project";

        [JsonIgnore]
        public AtariImage Image
        {
            get;
            set;
        }

        public AtariRaster[] Rasters
        {
            get;
            set;
        }

        public int SelectedRasterIndex
        {
            get;
            set;
        }

        [JsonIgnore]
        public AtariRaster SelectedRaster
        {
            get
            {
                return this.Rasters[this.SelectedRasterIndex];
            }

            set
            {
                for(int i = 0; i < this.Rasters.Length; i++)
                {
                    if (this.Rasters[i] == value)
                    {
                        this.SelectedRasterIndex = i;
                        break;
                    }
                }
            }
        }

        public void SwapRasterWithSelected(AtariRaster raster)
        {
            int indexOf = raster.ColorIndex;

            if(indexOf != -1 && indexOf != this.SelectedRasterIndex)
            {
                var selectedIndexOf = this.SelectedRasterIndex;
                var selectedRaster = this.SelectedRaster;

                this.Rasters[indexOf].SwapColorIndex(this.SelectedRaster);
                //SelectedRasterIndex = indexOf;

                this.Rasters[selectedIndexOf] = this.Rasters[indexOf];
                this.Rasters[indexOf] = selectedRaster;
            }
        }

        public string Title 
        {
            get
            {
                if(string.IsNullOrWhiteSpace(this.title))
                {
                    this.title = this.FilenameWithoutExtension;

                    if (string.IsNullOrWhiteSpace(this.title))
                    {
                        this.title = "New Project";
                    }
                }

                return this.title;
            }

            set
            {
                this.title = value;
            }
        }

        private string title;

        private async Task<byte[]> EncodeBGRA8ToPngAsync(byte[] pixelsBGRA32, int width, int height)
        {
            using (InMemoryRandomAccessStream pngStream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, pngStream);
                    
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)width,
                    (uint)height,
                    dpiX: 96,
                    dpiY: 96,
                    pixels: pixelsBGRA32);
                        
                await encoder.FlushAsync();

                byte[] result = new byte[pngStream.Size];

                DataReader reader = new DataReader(pngStream);
                await reader.LoadAsync((uint)result.Length);
                reader.ReadBytes(result);

                return result;
            }
        }

        public async Task SaveProjectAsync()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Raster Studio Project file", new List<string>() { ".hbl" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = this.Filename;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            
            if (file != null)
            {
                await this.SaveProjectAsync(file);
            }
        }

        public async Task SaveProjectAsync(StorageFile file)
        {
            string jsonProject = JsonConvert.SerializeObject(this);

            var jsonArray = Encoding.ASCII.GetBytes(jsonProject);

            // Image to Png
            var bgra8Pixels = this.Image.GetPixelsBGRA8();
            var pngArray = await this.EncodeBGRA8ToPngAsync(bgra8Pixels, this.Image.Width, this.Image.Height);

            // Ecriture du fichier .ras
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                await stream.WriteAsync(BitConverter.GetBytes(jsonArray.Length), 0, 4);
                await stream.WriteAsync(jsonArray, 0, jsonArray.Length);
                await stream.WriteAsync(BitConverter.GetBytes(pngArray.Length), 0, 4);
                await stream.WriteAsync(pngArray, 0, pngArray.Length);
            }
        }

        public async Task ExportProjectAsync(string extension)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Raster Studio Export file", new List<string>() { extension });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = this.FilenameWithoutExtension + extension;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                await this.ExportProjectAsync(file);
            }
        }

        public async Task ExportProjectAsync(StorageFile file)
        {
            string export = this.Exporter.GetExportText();

            var exportArray = Encoding.ASCII.GetBytes(export);

            // Ecriture du fichier d'exportation
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                await stream.WriteAsync(exportArray, 0, exportArray.Length);
            }
        }

        public async Task LoadProjectAsync()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".hbl");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                await LoadProjectAsync(file);

                this.Filename = file.Name;
            }
        }

        public async Task LoadProjectAsync(StorageFile file)
        {
            byte[] content = null;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                content = new byte[stream.Length];
                await stream.ReadAsync(content, 0, content.Length);
            }

            int index = 0;

            int jsonLength = BitConverter.ToInt32(content, index);

            index += 4;

            var jsonProject = Encoding.ASCII.GetString(content, index, jsonLength);

            index += jsonLength;

            int pngLength = BitConverter.ToInt32(content, index);

            index += 4;

            MemoryStream streamPng = new MemoryStream(content, index, pngLength);

            AtariImage image = AtariImage.Load(streamPng);

            var project = JsonConvert.DeserializeObject <Project>(jsonProject);
        
            for(int i = 0; i<this.Rasters.Length; i++)
            {
                var raster = project.Rasters[i];
                raster.Initialize(image, i);
            }

            this.Image = image;
            this.Rasters = project.Rasters;
            this.SelectedRasterIndex = project.SelectedRasterIndex;
        }
    }
}
