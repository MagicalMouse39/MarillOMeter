using MarillOMeter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MarillOMeter.Controls
{
    public sealed partial class RectangleSelector : UserControl
    {
        private Point startPoint;

        private bool isPointerDown;

        public Vector4D Selection =>
                new Vector4D(this.LeftCol.Width.Value, this.TopRow.Height.Value, this.CentralCol.Width.Value, this.CentralRow.Height.Value);

        public Vector2P SelectionPoints =>
            new Vector2P(new Point(this.LeftCol.Width.Value, this.TopRow.Height.Value), new Point(this.CentralCol.Width.Value, this.CentralRow.Height.Value));

        public RectangleSelector()
        {
            this.isPointerDown = false;

            this.InitializeComponent();

            this.PointerPressed += (s, e) =>
            {
                this.isPointerDown = true;

                var point = e.GetCurrentPoint(this).RawPosition;

                this.startPoint = point;

                this.LeftCol.Width = new GridLength(point.X);
                this.TopRow.Height = new GridLength(point.Y);
            };

            this.PointerMoved += (s, e) =>
            {
                if (!this.isPointerDown)
                    return;

                var point = e.GetCurrentPoint(this).RawPosition;

                var deltaX = point.X - this.startPoint.X;
                var deltaY = point.Y - this.startPoint.Y;

                if (deltaY < 0)
                {
                    this.TopRow.Height = new GridLength(point.Y);
                    this.CentralRow.Height = new GridLength(this.startPoint.Y - point.Y);
                }
                else
                    this.CentralRow.Height = new GridLength(deltaY);

                if (deltaX < 0)
                {
                    this.LeftCol.Width = new GridLength(point.X);
                    this.CentralCol.Width = new GridLength(this.startPoint.X - point.X);
                }
                else
                    this.CentralCol.Width = new GridLength(deltaX);
            };

            this.PointerReleased += (s, e) =>
            {
                this.isPointerDown = false;

                this.CentralCol.Width = new GridLength(0);
                this.CentralRow.Height = new GridLength(0);

                this.Visibility = Visibility.Collapsed;
            };
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
