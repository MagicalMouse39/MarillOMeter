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
        public MainPage()
        {
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

            this.TestMapRoute();
        }

        private async void TestMapRoute()
        {
            var polyline = new MapPolyline();
            polyline.StrokeColor = Colors.Red;
            polyline.StrokeThickness = 3;


            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".gpx");

            StorageFile file = await picker.PickSingleFileAsync();

            var trackSource = new GpxTrackSource(file);

            polyline.Path = new Geopath(trackSource.Positions);

            Map.MapElements.Add(polyline);
        }
    }
}
