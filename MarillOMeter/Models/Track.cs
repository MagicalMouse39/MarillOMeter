using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace MarillOMeter.Models
{
    internal class Track
    {
        public string Name { get; set; }
        
        public MapPolyline Polyline { get; set; }

        public Track(string name, MapPolyline polyline)
        {
            this.Name = name;
            this.Polyline = polyline;
        }
    }
}
