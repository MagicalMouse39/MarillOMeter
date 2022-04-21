using MarillOMeter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

            this.NewTrackBtn.Click += (s, e) =>
                MainPage.Instance.LoadTrackFile();

            this.DelTrackBtn.Click += (s, e) =>
            {
                if (this.TrackList.SelectedItems.Count <= 0)
                    return;

                MainPage.Instance.RemoveTrackAt(this.TrackList.SelectedIndex);
            };

            this.TrackList.SelectionChanged += (s, e) =>
            {
                this.DelTrackBtn.IsEnabled = this.TrackList.SelectedItems.Count > 0;
                this.EditTrackBtn.IsEnabled = this.TrackList.SelectedItems.Count == 1;
            };

            this.TrackList.ItemsSource = MainPage.Instance.Tracks;
        }

        private async void OnCheckBoxCheckChanged(object sender, RoutedEventArgs e)
        {
            var cb = e.OriginalSource as CheckBox;

            foreach (var track in (from x in MainPage.Instance.Tracks where x.Name == cb.AccessKey select x))
                track.IsVisible = cb.IsChecked ?? true;
        }
    }
}
