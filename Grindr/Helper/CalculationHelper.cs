using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
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
            //double xDelta = Math.Abs(p1.X) - Math.Abs(p2.X);
            //double yDelta = Math.Abs(p1.Y) - Math.Abs(p2.Y);

            double xDelta = p1.X - p2.X;
            double yDelta = p1.Y - p2.Y;

            //return Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));
            return xDelta * xDelta + yDelta * yDelta;
        }

        public static double RadToDeg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double GetAngle(Coordinate A, Coordinate B)
        {
            // |A·B| = |A| |B| COS(θ)
            // |A×B| = |A| |B| SIN(θ)

            return Math.Atan2(Cross(A, B), Dot(A, B));
        }

        public static double GetAngle(Coordinate A, Coordinate B, Coordinate center)
        {
            // |A·B| = |A| |B| COS(θ)
            // |A×B| = |A| |B| SIN(θ)

            var a2 = new Coordinate(A.X - center.X, A.Y - center.Y);
            var b2 = new Coordinate(B.X - center.X, B.Y - center.Y);

            return Math.Atan2(Cross(a2, b2), Dot(a2, b2));
        }

        public static double GetAngle(double ax, double ay, double bx, double by, double centerx, double centery)
        {
            // |A·B| = |A| |B| COS(θ)
            // |A×B| = |A| |B| SIN(θ)

            var a2 = new Coordinate(ax - centerx, ay + centery);
            var b2 = new Coordinate(bx - centerx, by - centery);

            var angle = Math.Atan2(Cross(a2, b2), Dot(a2, b2)) + Math.PI;

            if (angle < 0)
            {
                angle += Math.PI * 2;
            }

            if (angle > Math.PI * 2)
            {
                angle -= Math.PI * 2;
            }

            return angle  ;
        }


        public static double Dot(Coordinate A, Coordinate B)
        {
            return A.X * B.X + A.Y * B.Y;
        }

        public static double Cross(Coordinate A, Coordinate B)
        {
            return A.X * B.Y - A.Y * B.X;
        }
    }
}
