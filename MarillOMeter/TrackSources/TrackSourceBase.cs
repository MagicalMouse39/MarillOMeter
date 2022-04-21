using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace MarillOMeter.TrackSources
{
    internal abstract class TrackSourceBase
    {
        public StorageFile File { get; protected set; }

        public string TrackName { get; protected set; }

        public List<BasicGeoposition> Positions { get; private set; }

        public TrackSourceBase(StorageFile file)
        {
            this.File = file;
            this.Positions = new List<BasicGeoposition>();
            this.LoadPoints();
        }

        protected abstract void LoadPoints();
    }
}
