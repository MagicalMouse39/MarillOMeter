using MarillOMeter.Models;
using MarillOMeter.Pages;
using MarillOMeter.TrackSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Shapes;

namespace MarillOMeter
{
    /// <summary>
    /// Main and unique page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance;

        internal ObservableCollection<Track> Tracks;

        private Random rand;

        public MainPage()
        {
            MainPage.Instance = this;

            this.rand = new Random();
            this.Tracks = new ObservableCollection<Track>();

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

            this.TrackListBtn.Click += (s, e) =>
            {
                this.Splitter.IsPaneOpen = true;
                this.SidePane.Content = new TrackListPage();
            };
        }

        public void CloseSidePane() =>
            this.Splitter.IsPaneOpen = false;

        internal async void RemoveTrackAt(int index)
        {
            var track = this.Tracks[index];
            if (track.IsEdited)
            {
                var dialog = new MessageDialog("This track has been edited, do you want to delete it anyways?", "Confirm track deletion");
                dialog.Commands.Add(new UICommand("Yes"));
                dialog.Commands.Add(new UICommand("No"));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                
                var cmd = await dialog.ShowAsync();

                if (cmd.Label == "No")
                    return;
            }
            this.Map.Layers.Remove(track.ElementsLayer);
            this.Tracks.RemoveAt(index);
        }

        internal async void LoadTrackFile()
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

            byte[] b = new byte[3];
            this.rand.NextBytes(b);
            Color color = Color.FromArgb(255, (byte)(b[0] * 1.5), (byte)(b[1] * 1.5), (byte)(b[2] * 1.5));

            var layer = new MapElementsLayer();

            var polyline = new MapPolyline();
            polyline.StrokeColor = color;
            polyline.StrokeThickness = 3;
            polyline.Path = new Geopath(trackSource.Positions);

            var name = trackSource.TrackName;
            if ((from x in this.Tracks where x.Name == name select x).Count() > 0)
            {
                int i = 2;
                for (; (from x in this.Tracks where x.Name == $"{name} {i}" select x).Count() > 0; i++)
                    ;
                name = $"{name} {i}";
            }

            var track = new Track(name, new SolidColorBrush(color), layer, polyline);

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

            this.Tracks.Add(track);

            this.CenterMapOnTrack(track);
        }

        private async void CenterMapOnTrack(Track track)
        {
            var scene = MapScene.CreateFromBoundingBox(new GeoboundingBox(track.NorthWestCorner, track.SouthEastCorner));

            await Map.TrySetSceneAsync(scene, MapAnimationKind.Bow);
        }
    }
}
