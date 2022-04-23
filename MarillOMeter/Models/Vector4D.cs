using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarillOMeter.Models
{
    public struct Vector4D
    {
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;

        public Vector4D(double x, double y, double x2, double y2)
        {
            this.X1 = x;
            this.Y1 = y;
            this.X2 = x2;
            this.Y2 = y2;
        }
    }
}
