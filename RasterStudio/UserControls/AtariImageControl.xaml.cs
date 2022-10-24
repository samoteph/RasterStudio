using Atari.Images;
using RasterStudio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RasterStudio.UserControls
{
    public sealed partial class AtariImageControl : UserControl
    {
        public AtariImageControl()
        {
            this.InitializeComponent();
        }

        public PaletteControl PaletteControl
        {
            get;
            set;
        }

        public Project Project
        {
            get;
            set;
        }

        public bool DisplayBlankScreen
        {
            get;
            set;
        }

        public bool DisplayAllRasters
        {
            get;
            set;
        }

        public void RestoreColorOnScreen(int colorIndex)
        {
            AtariColor color = this.PaletteControl.Palette[colorIndex];

            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int i = 0; i < (this.Project.Image.Width * this.Project.Image.Height); i++)
                {
                    // le raster sera appliqué ici
                    if (this.Project.Image.GetIndexedColor(i) == colorIndex)
                    {
                        address = i * 4;

                        pixels[address + 0] = (byte)(color.B * 32); // Blue
                        pixels[address + 1] = (byte)(color.G * 32); // Green
                        pixels[address + 2] = (byte)(color.R * 32); // Red
                        pixels[address + 3] = 0xFF; // Aplha
                    }
                }

                this.SlateView.InvalidatePixels();
            }
        }

        public void DrawColorOnScreen(int targetColorIndex, byte r, byte g, byte b)
        {
            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int i = 0; i < (this.Project.Image.Width * this.Project.Image.Height); i++)
                {
                    // le raster sera appliqué ici
                    var indexedColor = this.Project.Image.GetIndexedColor(i);

                    if (indexedColor == targetColorIndex)
                    {
                        address = i * 4;

                        pixels[address + 0] = b; // Blue
                        pixels[address + 1] = g; // Green
                        pixels[address + 2] = r; // Red
                        pixels[address + 3] = 0xFF; // Aplha
                    }
                    else
                    {
                        address = i * 4;

                        var color = this.Project.Image.Palette[indexedColor];

                        pixels[address + 0] = (byte)(color.B * 32); // Blue
                        pixels[address + 1] = (byte)(color.G * 32); // Green
                        pixels[address + 2] = (byte)(color.R * 32); // Red
                        pixels[address + 3] = 0xFF; // Aplha
                    }
                }

                this.SlateView.InvalidatePixels();
            }
        }

        public void DrawBlankScreen()
        {

            if (this.Project.Rasters == null)
            {
                return;
            }

            int targetColorIndex = this.PaletteControl.SelectedColorIndex;

            var raster = this.Project.Rasters[targetColorIndex];

            raster.GenerateRaster();

            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int y = 0; y < this.Project.Image.Height; y++)
                {
                    var rasterColor = raster.Colors[y];

                    var b = (byte)(rasterColor.B * 32);
                    var g = (byte)(rasterColor.G * 32);
                    var r = (byte)(rasterColor.R * 32);

                    for (int x = 0; x < this.Project.Image.Width; x++)
                    {
                        int i = x + (y * this.Project.Image.Width);

                        pixels[address + 0] = b; // Blue
                        pixels[address + 1] = g; // Green
                        pixels[address + 2] = r; // Red
                        pixels[address + 3] = 0xFF; // Aplha
                    }
                }
            }

            this.SlateView.InvalidatePixels();
        }

        public void DrawCursorLineOnScreen(int line)
        {
            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;
                int baseAddress = line * 4 * this.Project.Image.Width;

                // lecture de l'image
                for (int x = 0; x < this.Project.Image.Width; x++)
                {
                    // le raster sera appliqué ici
                    var indexedColor = this.Project.Image.GetIndexedColor(x, line);

                    address = x * 4 + baseAddress;
                    var color = this.PaletteControl.Palette[indexedColor];

                    byte r = (byte)((color.R * 32) ^ 0xFF);
                    byte g = (byte)((color.G * 32) ^ 0xFF);
                    byte b = (byte)((color.B * 32) ^ 0xFF);

                    pixels[address + 0] = b; // Blue
                    pixels[address + 1] = g; // Green
                    pixels[address + 2] = r; // Red
                    pixels[address + 3] = 0xFF; // Aplha
                }

                this.SlateView.InvalidatePixels();
            }
        }

        public void DrawRasterOnScreen()
        {
            if (this.DisplayBlankScreen)
            {
                this.DrawBlankScreen();
            }
            else
            {
                if (this.DisplayAllRasters)
                {
                    this.DrawAllRastersOnScreen();
                }
                else
                {
                    this.DrawSelectedRasterOnScreen();
                }
            }
        }

        public void DrawSelectedRasterOnScreen()
        {

            if (this.Project.Rasters == null)
            {
                return;
            }

            int targetColorIndex = this.PaletteControl.SelectedColorIndex;

            var raster = this.Project.Rasters[targetColorIndex];

            raster.GenerateRaster();

            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int y = 0; y < this.Project.Image.Height; y++)
                {
                    var rasterColor = raster.Colors[y];

                    var b = (byte)(rasterColor.B * 32);
                    var g = (byte)(rasterColor.G * 32);
                    var r = (byte)(rasterColor.R * 32);

                    for (int x = 0; x < this.Project.Image.Width; x++)
                    {
                        int i = x + (y * this.Project.Image.Width);

                        // le raster sera appliqué ici
                        var indexedColor = this.Project.Image.GetIndexedColor(i);

                        if (indexedColor == targetColorIndex)
                        {
                            address = i * 4;

                            pixels[address + 0] = b; // Blue
                            pixels[address + 1] = g; // Green
                            pixels[address + 2] = r; // Red
                            pixels[address + 3] = 0xFF; // Aplha
                        }
                        else
                        {
                            address = i * 4;

                            var color = this.Project.Image.Palette[indexedColor];

                            pixels[address + 0] = (byte)(color.B * 32); // Blue
                            pixels[address + 1] = (byte)(color.G * 32); // Green
                            pixels[address + 2] = (byte)(color.R * 32); // Red
                            pixels[address + 3] = 0xFF; // Aplha
                        }
                    }
                }

                this.SlateView.InvalidatePixels();
            }
        }

        public void DrawAllRastersOnScreen()
        {

            if (this.Project.Rasters == null)
            {
                return;
            }

            for (int colorIndex = 0; colorIndex < this.Project.Rasters.Length; colorIndex++)
            {
                var raster = this.Project.Rasters[colorIndex];

                raster.GenerateRaster();
            }

            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int y = 0; y < this.Project.Image.Height; y++)
                {
                    for (int x = 0; x < this.Project.Image.Width; x++)
                    {
                        int i = x + (y * this.Project.Image.Width);

                        // le raster sera appliqué ici
                        var indexedColor = this.Project.Image.GetIndexedColor(i);

                        var raster = this.Project.Rasters[indexedColor];
                        var color = raster.Colors[y];

                        address = i * 4;

                        pixels[address + 0] = (byte)(color.B * 32); // Blue
                        pixels[address + 1] = (byte)(color.G * 32); // Green
                        pixels[address + 2] = (byte)(color.R * 32); // Red
                        pixels[address + 3] = 0xFF; // Aplha
                    }
                }
            }

            this.SlateView.InvalidatePixels();
        }

        public void DrawAtariImageOnScreen()
        {
            var pixels = this.SlateView.Pixels;

            if (pixels != null && this.Project.Image != null)
            {
                int address = 0;

                // lecture de l'image
                for (int i = 0; i < (this.Project.Image.Width * this.Project.Image.Height); i++)
                {
                    var color = this.Project.Image.Palette[this.Project.Image.GetIndexedColor(i)];

                    address = i * 4;

                    pixels[address + 0] = (byte)(color.B * 32); // Blue
                    pixels[address + 1] = (byte)(color.G * 32); // Green
                    pixels[address + 2] = (byte)(color.R * 32); // Red
                    pixels[address + 3] = 0xFF; // Aplha
                }

                this.SlateView.InvalidatePixels();
            }
        }

        public async Task LoadImageAsync(StorageFile file)
        {
            using (var streamStorage = await file.OpenReadAsync())
            {
                using (var stream = streamStorage.AsStreamForRead())
                {
                    // chargement et creation de la palette automatiquement
                    this.Project.Image = AtariImage.Load(stream);
                }
            }

            // Affichage dans SlateView
            await this.SlateView.LoadImage(file);
        }
    }
}
