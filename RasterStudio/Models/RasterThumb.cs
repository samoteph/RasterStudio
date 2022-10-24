using Atari.Images;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace RasterStudio.Models
{
    public class RasterThumb : INotifyPropertyChanged
    {
        public int Line
        {
            get
            {
                return this.line;
            }

            set
            {
                if(this.line != value)
                {
                    this.line = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private int line = -1;

        public bool IsEdge
        {
            get
            {
                return isEdge;
            }

            set
            {
                if(isEdge != value)
                {
                    isEdge = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private bool isEdge = false;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private bool isSelected = false;

        /// <summary>
        /// Couleur Atari sérialisée
        /// </summary>

        public int Color12
        {
            get
            {
                return this.Color.Color;
            }

            set
            {
                if (this.Color.Color != value)
                {
                    this.Color = new AtariColor(value);
                }
            }
        }

        [JsonIgnore]
        public AtariColor Color
        {
            get
            {
                return this.color;
            }

            set
            {
                if(this.color.Color != value.Color)
                {
                    this.color = value;
                    this.Color32 = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, (byte)(value.R * 32), (byte)(value.G * 32),(byte)(value.B * 32)));

                    this.RaisePropertyChange();
                    this.RaisePropertyChange(nameof(Color32));
                }
            }
        }

        private AtariColor color;

        /// <summary>
        /// Color32
        /// </summary>

        [JsonIgnore]
        public SolidColorBrush Color32
        {
            get;
            private set;
        } = new SolidColorBrush(Colors.Black);

        public EasingFunction EasingFunction
        {
            get
            {
                return easingFunction;
            }

            set
            {
                if(this.easingFunction != value)
                {
                    this.easingFunction = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private EasingFunction easingFunction = EasingFunction.Linear;

        public EasingMode EasingMode
        {
            get
            {
                return easingMode;
            }

            set
            {
                if (this.easingMode != value)
                {
                    this.easingMode = value;
                    this.RaisePropertyChange();
                }
            }
        }

        private EasingMode easingMode = EasingMode.Out;

        public RasterThumb()
        {
        }

        public void Fill(int line, AtariColor color, EasingFunction easingFunction = EasingFunction.Linear, bool isEdge = true)
        {
            Line = line;
            Color = color;
            IsEdge = isEdge;
            EasingFunction = easingFunction;
        }

        private void RaisePropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
