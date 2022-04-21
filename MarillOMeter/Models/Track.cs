using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;

namespace MarillOMeter.Models
{
    internal class Track
    {
        private bool visible;

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

        public MapPolyline Polyline { get; set; }

        public MapElementsLayer ElementsLayer { get; set; }

        public BasicGeoposition NorthWestCorner { get; set; }

        public BasicGeoposition SouthEastCorner { get; set; }

        public BasicGeoposition StartPoint { get; set; }

        public BasicGeoposition EndPoint { get; set; }

        public Track(string name, Brush color, MapElementsLayer layer, MapPolyline polyline)
        {
            this.visible = true;

            this.Name = name;
            this.Color = color;
            this.ElementsLayer = layer;
            this.Polyline = polyline;

            double latMax = 0, latMin = polyline.Path.Positions[0].Latitude, lonMax = 0, lonMin = polyline.Path.Positions[0].Longitude;

            foreach (var leg in polyline.Path.Positions)
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

            this.StartPoint = polyline.Path.Positions.First();
            this.EndPoint = polyline.Path.Positions.Last();
        }
    }
}
