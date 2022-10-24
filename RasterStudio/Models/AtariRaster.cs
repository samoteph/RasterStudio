using Atari.Images;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace RasterStudio.Models
{
    public class AtariRaster
    {
        // Contient les couleurs du rasters
        [JsonIgnore]
        public AtariColor[] Colors
        {
            get;
            private set;
        }

        [JsonIgnore]
        ObservableCollection<RasterThumb> thumbs = new ObservableCollection<RasterThumb>();

        public ObservableCollection<RasterThumb> Thumbs
        {
            get
            {
                return this.thumbs;
            }

            // Pour la serialisation

            set
            {
                this.thumbs = value;
            }
        }

        [JsonIgnore]
        public RasterThumb SelectedRasterThumb
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool HaveRasterThumbDefined 
        { 
            get
            {
                if(this.thumbs.Count > 2)
                {
                    return true;
                }
                else if(this.thumbs.Count == 2)
                {
                    bool haveOnlyEdge = this.thumbs[0].Line == 0 && this.thumbs[1].Line == 199;
                    return haveOnlyEdge == false;
                }

                return false;
            }
        }

        public AtariRaster()
        {
        }

        /// <summary>
        /// Adresse de la couleur dans la achine (Atari,Amiga)
        /// </summary>

        public string ColorAddress
        {
            get;
            private set;
        }

        public int ColorIndex
        {
            get
            {
                return colorIndex;
            }

            private set
            {
                this.colorIndex = value;
                this.ColorAddress = GetColorAddress(value);
            }
        }

        private int colorIndex;

        public static string GetColorAddress(int colorIndex)
        {
            // Atari
            uint address = 0xffff8240 + (uint)(colorIndex * 2);
            return address.ToString("X");
        }

        public void Initialize(AtariImage image, int colorIndex)
        {
            this.ColorIndex = colorIndex;
            this.Colors = new AtariColor[image.Height];

            var color = image.Palette[colorIndex];

            // Head peut être queue et vice versa

            if (this.thumbs.Count == 0)
            {
                var thumbHead = new RasterThumb();
                thumbHead.Fill( 0, color, EasingFunction.Linear, true);
                var thumbQueue = new RasterThumb();
                thumbQueue.Fill( image.Height - 1, color, EasingFunction.Linear, true);

                this.Thumbs.Add(thumbHead);
                this.Thumbs.Add(thumbQueue);
            }
        }

        public RasterThumb GetThumbEdgeHead()
        {
            RasterThumb head = null;

            foreach(var thumb in Thumbs)
            {
                if(thumb.IsEdge)
                {
                    if(head == null)
                    {
                        head = thumb;
                    }
                    else if(head.Line > thumb.Line)
                    {
                        head = thumb;
                    }
                }
            }

            return head;
        }

        public RasterThumb GetThumbEdgeQueue()
        {
            RasterThumb queue = null;

            foreach (var thumb in Thumbs)
            {
                if (thumb.IsEdge)
                {
                    if (queue == null)
                    {
                        queue = thumb;
                    }
                    else if (queue.Line < thumb.Line)
                    {
                        queue = thumb;
                    }
                }
            }

            return queue;
        }

        public void GenerateRaster()
        {
            // trie de thumbs
            var sortedThumbs = thumbs.ToList();

            sortedThumbs.Sort((a, b) => a.Line > b.Line ? 1 : -1);

            var head = this.GetThumbEdgeHead();
            // RemoveBefore Head
            while (sortedThumbs[0] != head)
            {
                sortedThumbs.RemoveAt(0);
            }

            var queue = this.GetThumbEdgeQueue();
            // RemoveAfter Queue
            while (sortedThumbs[sortedThumbs.Count-1] != queue)
            {
                sortedThumbs.RemoveAt(sortedThumbs.Count - 1);
            }

            if(head.Line > 0)
            {
                var raster = new RasterThumb();
                raster.Fill(0, head.Color, EasingFunction.None, false);

                sortedThumbs.Insert(0, raster);
            }

            if (queue.Line < 199)
            {
                var raster = new RasterThumb();
                raster.Fill(199, queue.Color, EasingFunction.None, false);
                sortedThumbs.Add(raster);
            }


            RasterThumb lastThumb = null;


            foreach(var thumb in sortedThumbs)
            {
                // par deux toujours ils vont, le thumb et son last
                if(lastThumb != null)
                {
                    double distancePixel = (thumb.Line - lastThumb.Line) + 1;

                    double distanceR = (thumb.Color.R - lastThumb.Color.R) + 1;
                    double distanceG = (thumb.Color.G - lastThumb.Color.G) + 1;
                    double distanceB = (thumb.Color.B - lastThumb.Color.B) + 1;

                    double startR = lastThumb.Color.R;
                    double startG = lastThumb.Color.G;
                    double startB = lastThumb.Color.B;

                    for (int i= 0; i < distancePixel; i++)
                    {
                        double normalizedDistance = i / (distancePixel - 1);

                        normalizedDistance = EasingTool.Interpolate(normalizedDistance, lastThumb.EasingFunction, lastThumb.EasingMode);

                        normalizedDistance= Math.Clamp(normalizedDistance, 0,1);

                        double r = startR + (distanceR * normalizedDistance);
                        double g = startG + (distanceG * normalizedDistance);
                        double b = startB + (distanceB * normalizedDistance);

                        Colors[lastThumb.Line + i] = new AtariColor((byte)r, (byte)g, (byte)b);
                    }
                }

                lastThumb = thumb;
            }
        }
    }
}
