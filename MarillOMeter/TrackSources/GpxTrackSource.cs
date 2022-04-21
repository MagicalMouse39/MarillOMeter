using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace MarillOMeter.TrackSources
{
    internal class GpxTrackSource : TrackSourceBase
    {
        public GpxTrackSource(StorageFile file) : base(file)
        {
            
        }

        protected override void LoadPoints()
        {
            var doc = new XmlDocument();

            doc.LoadXml(FileIO.ReadTextAsync(this.File).AsTask().Result);
            
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("x", "http://www.topografix.com/GPX/1/1");

            var nodes = doc.SelectNodes("//x:trkpt", nsmgr);

            foreach (XmlNode node in nodes)
                this.Positions.Add(new BasicGeoposition() { Latitude = double.Parse(node.Attributes["lat"].Value), Longitude = double.Parse(node.Attributes["lon"].Value) });
        }
    }
}
