using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Controllo utente è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234236

namespace MarillOMeter.Controls
{
    public sealed partial class MovableMapPin : MapElement
    {
        private MapControl map;
        private MapLayer mapLayer;
        private Geopoint mapCenter;

        private bool isDragging;

        public bool IsDraggable { get; set; }

        public MovableMapPin(MapControl map)
        {
            this.isDragging = false;

            this.map = map;
            
            this.InitializeComponent();
        }

            /*
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (!this.IsDraggable || this.map == null)
                return;

            this.mapCenter = this.map.Center;

            this.map.PointerMoved += Map_PointerMoved;
        }

        private void Map_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.isDragging)
            {
                //If so, move the Pushpin to where the pointer is.
                var pointerPosition = e.GetCurrentPoint(this.map);

                Geopoint location = null;

                //Convert the point pixel to a Location coordinate
                if (this.map.TryGetLocationFromOffset(pointerPosition.Position, out location))
                {
                    this.(this, location);
                }
            }
        }

        private void Map_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Pushpin released, remove dragging events
            if (this.map != null)
            {
                this.map.CenterChanged -= Map_CenterChanged;
                this.map.PointerReleasedOverride -= Map_PointerReleased;
                this.map.PointerMovedOverride -= Map_PointerMoved;
            }

            var pointerPosition = e.GetCurrentPoint(_map);

            Location location = null;

            //Convert the point pixel to a Location coordinate
            if (_map.TryPixelToLocation(pointerPosition.Position, out location))
            {
                MapLayer.SetPosition(this, location);
            }

            if (DragEnd != null)
            {
                DragEnd(location);
            }

            this.isDragging = false;
        }

        private void Map_CenterChanged(object sender, EventArgs e)
        {
            if (this.isDragging)
                this.map.Center = this.mapCenter;
        }
        */
    }
}