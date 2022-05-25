using MarillOMeter.Extensions;
using MarillOMeter.Models;
using MarillOMeter.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MarillOMeter.Pages
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class TrackListPage : Page
    {
        internal TrackListPage()
        {
            this.InitializeComponent();

            this.BackBtn.Click += (s, e) =>
                MainPage.Instance.CloseSidePane();

            this.NewTrackBtn.Click += async (s, e) =>
            {
                var name = await InputDialog("Track name", "New track", string.Empty);
                MainPage.Instance.Tracks.Add(new Track(name, ColorUtils.RandomBrightBrush()));
            };

            this.EditTrackBtn.Click += (s, e) =>
            {
                if (MainPage.Instance.IsEditing)
                {
                    MainPage.Instance.FinishEditTrack();
                    this.FinishEditTrack();
                    return;
                }

                if (this.TrackList.SelectedItems.Count != 1)
                    return;

                this.StartEditTrack();
                MainPage.Instance.EditTrack(MainPage.Instance.Tracks[this.TrackList.SelectedIndex]);
            };

            this.DelTrackBtn.Click += (s, e) =>
            {
                if (this.TrackList.SelectedItems.Count <= 0)
                    return;

                foreach (var range in this.TrackList.SelectedRanges)
                    for (int i = range.FirstIndex; i <= range.LastIndex; i++)
                        MainPage.Instance.RemoveTrackAt(i);
            };

            this.TrackList.SelectionChanged += (s, e) =>
            {
                this.DelTrackBtn.IsEnabled = this.TrackList.SelectedItems.Count > 0;

                if (MainPage.Instance.IsEditing)
                    return;

                var trigger = this.TrackList.SelectedItems.Count == 1;

                this.EditTrackBtn.IsEnabled = trigger;
                this.RenameTracksBtn.IsEnabled = trigger;
                this.CopyTrackBtn.IsEnabled = trigger;
                this.CenterOnTrackBtn.IsEnabled = trigger;
            };

            this.CopyTrackBtn.Click += async (s, e) =>
            {
                var track = MainPage.Instance.Tracks[this.TrackList.SelectedIndex].DeepClone();
                var name = await InputDialog("Track name", string.Empty, $"{track.Name} 2");
                
                if (name == null)
                    return;

                track.Name = name;
                MainPage.Instance.Tracks.Add(track);
            };

            this.CenterOnTrackBtn.Click += (s, e) =>
                MainPage.Instance.CenterMapOnTrack(MainPage.Instance.Tracks[this.TrackList.SelectedIndex]);

            if (MainPage.Instance.IsEditing)
                this.StartEditTrack();

            this.TrackList.ItemsSource = MainPage.Instance.Tracks;
        }

        internal void StartEditTrack()
        {
            this.EditTrackBtn.IsEnabled = true;
            this.EditTrackBtn.Icon = new SymbolIcon(Symbol.Save);
            this.EditTrackBtn.Label = "Save";
        }

        internal void FinishEditTrack()
        {
            this.EditTrackBtn.Icon = new SymbolIcon(Symbol.Edit);
            this.EditTrackBtn.Label = "Edit";
        }

        private async Task<string> InputDialog(string title, string placeholder, string text)
        {
            TextBox input = new TextBox()
            {
                Height = (double)App.Current.Resources["TextControlThemeMinHeight"],
                Text = text
            };

            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                MaxWidth = this.ActualWidth,
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                Content = input
            };

            dialog.DefaultButton = ContentDialogButton.Primary;
            ContentDialogResult result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                input = (TextBox)dialog.Content;
                return input.Text;
            }

            return null;
        }

        private void OnSwitchToggle(object sender, RoutedEventArgs e)
        {
            var cb = e.OriginalSource as ToggleSwitch;

            foreach (var track in (from x in MainPage.Instance.Tracks where x.Name == cb.AccessKey select x))
                track.IsVisible = cb.IsOn;
        }
    }
}
