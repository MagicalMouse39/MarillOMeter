using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MarillOMeter.Models
{
    public struct Vector2P
    {
        public Point X, Y;

        public Vector2P(Point x, Point y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
