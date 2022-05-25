using MarillOMeter.Controls;
using MarillOMeter.Extensions;
using MarillOMeter.Models;
using MarillOMeter.Pages;
using MarillOMeter.TrackSources;
using MarillOMeter.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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

        private Track editingTrack;

        private MapIcon selectedElement;

        internal ObservableCollection<Track> Tracks;

        public bool IsEditing { get; set; } = false;

        public MainPage()
        {
            MainPage.Instance = this;

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

            this.Map.MapTapped += (s, e) =>
            {
                /*
                foreach (var el in from el in this.Map.MapElements where el is MapIcon select el as MapIcon)
                    el.Image = null;
                */
                this.selectedElement = null;
            };

            this.Map.MapElementClick += (s, e) =>
            {
                if (this.editingTrack == null)
                    return;

                if (this.selectedElement != null)
                    this.selectedElement.Image = null;

                this.selectedElement = (from x in e.MapElements where x is MapIcon select x as MapIcon).ToArray()[0];

                this.selectedElement.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/StoreLogo.png"));
            };

            /*
            this.Map.MapRightTapped += (s, e) =>
            {
                if (this.editingTrack == null)
                    return;

                var pos = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                var mousePos = new Point(pos.X - Window.Current.Bounds.X, pos.Y - Window.Current.Bounds.Y);

                this.editingTrack.SummonIconsNearby(this.Map, mousePos, .1);
            };
            */
            /*
            this.Map.MapHolding += (s, e) =>
            {
                new MessageDialog("Holding tight").ShowAsync();
            };
            */
        }

        internal void EditTrack(Track track)
        {
            this.IsEditing = true;
            this.editingTrack = track;

            if (this.editingTrack.Polyline != null)
                this.editingTrack.Polyline.StrokeDashed = true;

            // TODO: Add mouse box "move the pointer to view the pins"
        }

        internal void EditSelection()
        {
            this.editingTrack.IsEdited = true;

            var points = this.MapSelection.SelectionPoints;

            Geopoint topleft, bottomright;

            if (!this.Map.TryGetLocationFromOffset(points.X, out topleft))
                return;
            if (!this.Map.TryGetLocationFromOffset(points.Y, out bottomright))
                return;

            // new MessageDialog($"{topleft.Position.Latitude}-{topleft.Position.Longitude} # {bottomright.Position.Latitude}-{bottomright.Position.Longitude}").ShowAsync();

            /*
            foreach (var leg in from leg in this.editingTrack.Polyline.Path.Positions where leg.IsInArea(topleft.Position, bottomright.Position) select leg)
            {
                var pin = new Button();
                this.Map.Children.Add(pin);
                MapControl.SetLocation(pin, new Geopoint(leg));
            }
            */



            if (this.editingTrack.Polyline != null)
                this.editingTrack.Polyline.StrokeDashed = true;
        }

        internal void FinishEditTrack()
        {
            this.IsEditing = false;

            if (this.editingTrack.Polyline != null)
                this.editingTrack.Polyline.StrokeDashed = false;

            this.editingTrack.ResetMovablePins();

            this.editingTrack = null;
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

            if (track == this.editingTrack)
            {
                this.FinishEditTrack();
                
                var trackListPage = this.SidePane.Content as TrackListPage;
                if (trackListPage == null)
                    return;
                trackListPage.FinishEditTrack();
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

            if (file == null)
                return;

            TrackSourceBase trackSource = default;

            switch (file.FileType)
            {
                case ".gpx":
                    trackSource = new GpxTrackSource(file);
                    break;
            }

            var layer = new MapElementsLayer();

            var color = ColorUtils.RandomBrightColor();

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

            var track = new Track(name, new SolidColorBrush(color), polyline, layer);

            Map.Layers.Add(layer);

            this.Tracks.Add(track);

            this.CenterMapOnTrack(track);
        }

        internal async void CenterMapOnTrack(Track track)
        {
            var scene = MapScene.CreateFromBoundingBox(new GeoboundingBox(track.NorthWestCorner, track.SouthEastCorner));

            await Map.TrySetSceneAsync(scene, MapAnimationKind.Bow);
        }
    }
}
