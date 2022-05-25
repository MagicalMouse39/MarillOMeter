using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;

namespace MarillOMeter.Models
{
    internal class Track
    {
        private bool visible;

        private Queue<MapIcon> movablePins;

        public string Name { get; set; }
        
        public Brush Color { get; set; }

        public bool IsVisible
        {
            get =>
                this.visible;
            set
            {
                this.ElementsLayer.Visible = value;
                this.visible = value;
            }
        }

        public bool IsEdited { get; set; } = false;

        private MapPolyline polyline;

        public MapPolyline Polyline
        {
            get =>
                this.polyline;
            set
            {
                this.polyline = value;
                this.GenerateBoundaries();
            }
        }

        public MapElementsLayer ElementsLayer { get; set; }

        public BasicGeoposition NorthWestCorner { get; set; }

        public BasicGeoposition SouthEastCorner { get; set; }

        public BasicGeoposition StartPoint { get; set; }

        public BasicGeoposition EndPoint { get; set; }

        public Track(string name, Brush color)
        {
            this.visible = true;

            this.Name = name;
            this.Color = color;
        }

        public Track(string name, Brush color, MapPolyline polyline, MapElementsLayer layer) : this(name, color, polyline, layer, true)
        {
            
        }

        public Track(string name, Brush color, MapPolyline polyline, MapElementsLayer layer, bool addStartEndPins)
        {
            this.visible = true;
            this.movablePins = new Queue<MapIcon>();

            this.Name = name;
            this.Color = color;
            this.Polyline = polyline;
            this.ElementsLayer = layer;

            this.GenerateBoundaries();

            this.ElementsLayer.MapElements.Add(polyline);

            if (addStartEndPins)
                this.AddStartEndPins();
        }

        /// <summary>
        /// Delete and dispose all the movable pins
        /// </summary>
        public void ResetMovablePins()
        {
            while (this.movablePins.Count > 0)
            {
                var pin = this.movablePins.Dequeue();
                this.ElementsLayer.MapElements.Remove(pin);
                pin = null;
            }
            GC.Collect();
        }


        /// <summary>
        /// Add Start and End <see cref="MapIcon"/> to the track
        /// </summary>
        public void AddStartEndPins()
        {
            var startPointIcon = new MapIcon
            {
                Location = new Geopoint(this.StartPoint),
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = $"Start: {this.Name}"
            };

            var endPointIcon = new MapIcon
            {
                Location = new Geopoint(this.EndPoint),
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = $"End: {this.Name}"
            };

            this.ElementsLayer.MapElements.Add(startPointIcon);
            this.ElementsLayer.MapElements.Add(endPointIcon);
        }

        public void SummonIconsNearby(MapControl map, Point point, double radius)
        {
            this.ResetMovablePins();
            
            Geopoint pos;

            if (!map.TryGetLocationFromOffset(point, out pos))
                return;

            new MessageDialog($"Lat: {pos.Position.Latitude}; Long: {pos.Position.Longitude}").ShowAsync();

            foreach (var leg in this.polyline.Path.Positions)
            {
                if (Math.Abs(pos.Position.Latitude - leg.Latitude) <= radius &&
                    Math.Abs(pos.Position.Longitude - leg.Longitude) <= radius)
                {
                    var icon = new MapIcon()
                    {
                        Location = new Geopoint(leg) 
                    };
                    this.ElementsLayer.MapElements.Add(icon);
                    this.movablePins.Enqueue(icon);
                }
            }
        }

        private void GenerateBoundaries()
        {
            double latMax = 0, latMin = this.polyline.Path.Positions[0].Latitude, lonMax = 0, lonMin = this.polyline.Path.Positions[0].Longitude;

            foreach (var leg in this.polyline.Path.Positions)
            {
                if (leg.Latitude > latMax)
                    latMax = leg.Latitude;

                if (leg.Latitude < latMin)
                    latMin = leg.Latitude;

                if (leg.Longitude > lonMax)
                    lonMax = leg.Longitude;

                if (leg.Longitude < lonMin)
                    lonMin = leg.Longitude;
            }

            this.SouthEastCorner = new BasicGeoposition() { Latitude = latMin, Longitude = lonMin };
            this.NorthWestCorner = new BasicGeoposition() { Latitude = latMax, Longitude = lonMax };

            this.StartPoint = this.polyline.Path.Positions.First();
            this.EndPoint = this.polyline.Path.Positions.Last();
        }
    }
}
