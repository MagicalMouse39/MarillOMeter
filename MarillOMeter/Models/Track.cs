using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace MarillOMeter.Models
{
    internal class Track
    {
        public string Name { get; set; }
        
        public MapPolyline Polyline { get; set; }

        public MapElementsLayer ElementsLayer { get; set; }

        public BasicGeoposition NorthWestCorner { get; set; }

        public BasicGeoposition SouthEastCorner { get; set; }

        public BasicGeoposition StartPoint { get; set; }

        public BasicGeoposition EndPoint { get; set; }

        public Track(string name, MapElementsLayer layer, MapPolyline polyline)
        {
            this.Name = name;
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
