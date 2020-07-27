using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad
{
    public static class CalculationHelper
    {
        public static double CalculateWowDirection(double px, double py, double targetx, double targety)
        {
            var slope = Math.Atan2(targety - py, px - targetx);
            // slope is the absolute direction to the next point from the player
            slope += Math.PI; // map to 0-2PI range
                              // Rotate by 90 degrees (so that 0 is up, not right)
            slope -= Math.PI * 0.5;
            // Ensures that slope is not less than 0
            if (slope < 0)
            {
                slope += Math.PI * 2;
            }
            // Ensures slope is not greater than 2p
            if (slope > Math.PI * 2)
            {
                slope -= Math.PI * 2;
            }

            return slope;
        }

        public static double CalculateDistance(Coordinate p1, Coordinate p2)
        {
            double xDelta = Math.Abs(p1.X) - Math.Abs(p2.X);
            double yDelta = Math.Abs(p1.Y) - Math.Abs(p2.Y);

            return Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));
        }

        public static double RadToDeg(double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}
