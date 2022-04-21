using MarillOMeter.Models;
using MarillOMeter.TrackSources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MarillOMeter
{
    /// <summary>
    /// Main and unique page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Track> tracks;

        public MainPage()
        {
            this.tracks = new List<Track>();

            this.InitializeComponent();

            this.FileExitBtn.Click += (s, e) =>
                Application.Current.Exit();

            foreach (var style in Enum.GetValues(typeof(MapStyle)) as MapStyle[])
            {
                if (style == MapStyle.None || style == MapStyle.Custom)
                    continue;

                MenuFlyoutItem styleBtn = new MenuFlyoutItem() { Text = style.ToString() };
                styleBtn.Click += (s, e) =>
                    this.Map.Style = style;
                this.MapStyleBtn.Items.Add(styleBtn);
            }

            this.FileOpenBtn.Click += (s, e) =>
                this.LoadTrackFile();
        }

        private async void LoadTrackFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".gpx");

            StorageFile file = await picker.PickSingleFileAsync();

            TrackSourceBase trackSource = default;

            switch (file.FileType)
            {
                case ".gpx":
                    trackSource = new GpxTrackSource(file);
                    break;
            }

            var layer = new MapElementsLayer();

            var polyline = new MapPolyline();
            polyline.StrokeColor = Colors.Red;
            polyline.StrokeThickness = 3;
            polyline.Path = new Geopath(trackSource.Positions);

            var track = new Track(trackSource.TrackName, layer, polyline);

            layer.MapElements.Add(polyline);

            var startPointIcon = new MapIcon
            {
                Location = new Geopoint(track.StartPoint),
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = $"Start: {track.Name}"
            };

            var endPointIcon = new MapIcon
            {
                Location = new Geopoint(track.EndPoint),
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = $"End: {track.Name}"
            };


            layer.MapElements.Add(startPointIcon);
            layer.MapElements.Add(endPointIcon);

            Map.Layers.Add(layer);

            this.tracks.Add(track);

            this.CenterMapOnTrack(track);
        }

        private async void CenterMapOnTrack(Track track)
        {
            var scene = MapScene.CreateFromBoundingBox(new GeoboundingBox(track.NorthWestCorner, track.SouthEastCorner));

            await Map.TrySetSceneAsync(scene, MapAnimationKind.Bow);
        }
    }
}
