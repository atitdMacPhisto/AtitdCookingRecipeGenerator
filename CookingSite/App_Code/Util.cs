using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Web;

namespace CookingSite.App_Code
{
    public class Util
    {
        public static float Sqr(float x)
        {
            return (float)Math.Pow(x, 2);
        }

        public static float Sqr(float x1, float x2)
        {
            return (float)((x1 - x2) * (x1 - x2));
        }

        public static float Sqrt(float x)
        {
            return (float)Math.Sqrt(Math.Abs(x));
        }

        public static float Pow(float x, int p)
        {
            return (float)Math.Pow(x, p);
        }

        public static double Atan2(double x, double y)
        {
            return Math.Atan2(y, x);
        }

        //public static decimal SqrD(decimal d1)
        //{
        //    return d1 * d1;
        //}

        //public static decimal SqrD(decimal d1, decimal d2)
        //{
        //    return (d1 - d2) * (d1 - d2);
        //}

        //public static decimal SqrtD(decimal d)
        //{
        //    return (decimal)Math.Sqrt((double)Math.Abs(d));
        //}

        //public static float SSqrD(float d1)
        //{
        //    float mul = 1.0f;
        //    if (d1 < 0)
        //        mul = -1.0f;
        //    return (float)(Math.Abs(d1 * d1) * mul);
        //}

        //public static float SSqrtD(float d)
        //{
        //    float mul = 1.0f;
        //    if (d < 0)
        //        mul = -1.0f;
        //    return (float)Math.Sqrt(Math.Abs(d)) * mul;
        //}

        public static float SSqr(float x)
        {
            float mult = 1.0f;
            if (x < 0)
                mult = -1.0f;
            return (float)Math.Pow(Math.Abs(x), 2) * mult;
        }

        public static float SSqrt(float x)
        {
            float mult = 1.0f;
            if (x < 0)
                mult = -1.0f;
            return (float)(Math.Sqrt(Math.Abs(x)) * mult);
        }

        public static float CalcDistance(PointF P1, PointF P2)
        {
            return (float)Math.Sqrt(Math.Pow(P1.X - P2.X, 2) + Math.Pow(P1.Y - P2.Y, 2));
        }

        public static PointF CalculateCoord(Pair[] pairs, string a)
        {
            if (pairs.Length < 3)
                return new PointF();

            float tolerance = 10.0f;

            float H15 = pairs[0].Distance;
            float I15 = pairs[0].Base.Name != a ? pairs[0].Base.X : pairs[0].Additive.X;
            float J15 = pairs[0].Base.Name != a ? pairs[0].Base.Y : pairs[0].Additive.Y;

            float H16 = pairs[1].Distance;
            float I16 = pairs[1].Base.Name != a ? pairs[1].Base.X : pairs[1].Additive.X;
            float J16 = pairs[1].Base.Name != a ? pairs[1].Base.Y : pairs[1].Additive.Y;

            float H17 = pairs[2].Distance;
            float I17 = pairs[2].Base.Name != a ? pairs[2].Base.X : pairs[2].Additive.X;
            float J17 = pairs[2].Base.Name != a ? pairs[2].Base.Y : pairs[2].Additive.Y;

            float M15 = (float)(I15 + H15 * Math.Cos(Atan2(I16 - I15, J16 - J15) + (Math.Acos((Pow((Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))), 2) + Pow(H15, 2) - Pow(H16, 2)) / (2 * (Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))) * H15)))));
            //if (M15 is float.NaN) continue;
            float M16 = (float)(I15 + H15 * Math.Cos(Atan2(I16 - I15, J16 - J15) - (Math.Acos((Pow((Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))), 2) + Pow(H15, 2) - Pow(H16, 2)) / (2 * (Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))) * H15)))));
            //if (M16 is float.NaN) continue;
            float M17 = (float)(I15 + H15 * Math.Cos(Atan2(I17 - I15, J17 - J15) + (Math.Acos((Pow((Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))), 2) + Pow(H15, 2) - Pow(H17, 2)) / (2 * (Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))) * H15)))));
            //if (M17 is float.NaN) continue;

            float N15 = (float)(J15 + H15 * Math.Sin(Atan2(I16 - I15, J16 - J15) + (Math.Acos((Pow((Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))), 2) + Pow(H15, 2) - Pow(H16, 2)) / (2 * (Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))) * H15)))));
            //if (N15 is float.NaN) continue;
            float N16 = (float)(J15 + H15 * Math.Sin(Atan2(I16 - I15, J16 - J15) - (Math.Acos((Pow((Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))), 2) + Pow(H15, 2) - Pow(H16, 2)) / (2 * (Sqrt(Pow((I15 - I16), 2) + Pow((J15 - J16), 2))) * H15)))));
            //if (N16 == float.NaN) continue;
            float N17 = (float)(J15 + H15 * Math.Sin(Atan2(I17 - I15, J17 - J15) + (Math.Acos((Pow((Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))), 2) + Pow(H15, 2) - Pow(H17, 2)) / (2 * (Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))) * H15)))));
            //if (N17 == float.NaN) continue;

            float O15 = (float)(I15 + H15 * Math.Cos(Atan2(I17 - I15, J17 - J15) - (Math.Acos((Pow((Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))), 2) + Pow(H15, 2) - Pow(H17, 2)) / (2 * (Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))) * H15)))));
            //if (O15 == float.NaN) continue;
            float P15 = (float)(J15 + H15 * Math.Sin(Atan2(I17 - I15, J17 - J15) - (Math.Acos((Pow((Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))), 2) + Pow(H15, 2) - Pow(H17, 2)) / (2 * (Sqrt(Pow((I15 - I17), 2) + Pow((J15 - J17), 2))) * H15)))));
            //if (P15 == float.NaN) continue;
            float Q15 = (Math.Abs(N15 - N17) > tolerance) ? 0 : ((N15 + N17) / 2);
            float R15 = (Math.Abs(N16 - N17) > tolerance) ? 0 : ((N16 + N17) / 2);

            float O16 = (Math.Abs(M15 - M17) > tolerance) ? 0 : ((M15 + M17) / 2);
            float P16 = (Math.Abs(M16 - M17) > tolerance) ? 0 : ((M16 + M17) / 2);
            float Q16 = (Math.Abs(N15 - P15) > tolerance) ? 0 : ((N15 + P15) / 2);
            float R16 = (Math.Abs(N16 - P15) > tolerance) ? 0 : ((N16 + P15) / 2);

            float O17 = (Math.Abs(M15 - O15) > tolerance) ? 0 : ((M15 + O15) / 2);
            float P17 = (Math.Abs(M16 - O15) > tolerance) ? 0 : ((M16 + O15) / 2);
            float Q17 = 0;
            float R17 = 0;

            if (O16 != 0)
                Q17 = O16;
            else if (O17 != 0)
                Q17 = O17;
            else if (P16 != 0)
                Q17 = P16;
            else if (P17 != 0)
                Q17 = P17;

            if (Q15 != 0)
                R17 = Q15;
            else if (Q16 != 0)
                R17 = Q16;
            else if (R15 != 0)
                R17 = R15;
            else if (R16 != 0)
                R17 = R16;

            if (Q17 == 0 || R17 == 0 || Q17 is float.NaN || R17 is float.NaN)
                return new PointF();

            return new PointF(Q17, R17);
        }

        public static List<PointF> Trilat(List<Circle> circles)
        {
            List<PointF> list = new List<PointF>();
            if (circles.Count < 3) return null;


            double[] A = { circles[0].Center.X, circles[0].Center.Y };
            double[] B = { circles[1].Center.X, circles[1].Center.Y };
            double[] C = { circles[2].Center.X, circles[2].Center.Y };

            double a = circles[0].Radius;
            double b = circles[1].Radius;
            double c = circles[2].Radius;

            double dA = a;
            double dB = b;
            double dC = c;

            double[] AB = { B[0] - A[0], B[1] - A[1] };
            double[] AC = { C[0] - A[0], C[1] - A[1] };

            // Calculate the proportion of each difference based on the distances
            double alpha1 = (dA * dA - dB * dB + (AB[0] * AB[0] + AB[1] * AB[1])) / (2 * (AB[0] * AB[0] + AB[1] * AB[1]));
            double beta1 = (dA * dA - dC * dC + (AC[0] * AC[0] + AC[1] * AC[1])) / (2 * (AC[0] * AC[0] + AC[1] * AC[1]));

            // Calculate the coordinates of the unknown point (P)
            double Px = A[0] + alpha1 * AB[0] + beta1 * AC[0];
            double Py = A[1] + alpha1 * AB[1] + beta1 * AC[1];


            // Calculate angles using the law of cosines
            double alpha = Math.Acos((Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c));
            double beta = Math.Acos((Math.Pow(c, 2) + Math.Pow(a, 2) - Math.Pow(b, 2)) / (2 * c * a));
            double gamma = Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));

            // Calculate direction angles from reference points to unknown point
            double thetaA = Math.Atan2(A[1], A[0]);
            double thetaB = Math.Atan2(B[1], B[0]);
            double thetaC = Math.Atan2(C[1], C[0]);

            // Calculate coordinates of the unknown point P
            double PxA = A[0] + a * Math.Cos(thetaA);
            double PyA = A[1] + a * Math.Sin(thetaA);

            double PxB = B[0] + b * Math.Cos(thetaB);
            double PyB = B[1] + b * Math.Sin(thetaB);

            double PxC = C[0] + c * Math.Cos(thetaC);
            double PyC = C[1] + c * Math.Sin(thetaC);

            list.Add(new PointF((float)PxA, (float)PyA));
            list.Add(new PointF((float)PxB, (float)PyB));
            list.Add(new PointF((float)PxC, (float)PyC));

            return list;
        }

        public static PointF Triangulate(List<Circle> circles)
        {
            List<PointF> vertices = new List<PointF>();
            List<Segment> segments = new List<Segment>();

            for (int i = 0; i < circles.Count - 1; i++)
            {
                for (int j = i + 1; j < circles.Count; j++)
                {
                    PointF[] points = GetCircleIntersections(circles[i], circles[j]);
                    if (points.Length == 2)
                        vertices.AddRange(points);
                }
            }

            if (vertices.Count < 6)
                return new PointF();

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    segments.Add(new Segment(vertices[i], vertices[j]));
                }
            }
            segments.Sort(CompareLinesByLength);
            if (segments.Count >= 3 && segments[0].Distance == 0 && segments[1].Distance == 0 && segments[2].Distance == 0)
                return new PointF(segments[0].P1.X, segments[0].P1.Y);

            List<Segment> newSegments = new List<Segment>();
            for (int i = 0; i < segments.Count && newSegments.Count < circles.Count; i++)
            {
                if (newSegments.Contains(new Segment(segments[i].P1, segments[i].P2)) || newSegments.Contains(new Segment(segments[i].P2, segments[i].P1))) continue;
                newSegments.Add(segments[i]);
            }

            List<PointF> polygonVertices = new List<PointF>();
            for (int i = 0; i < newSegments.Count; i++)
            {
                if (polygonVertices.Contains(segments[i].P1)) continue;
                polygonVertices.Add(segments[i].P1);
                if (polygonVertices.Contains(segments[i].P2)) continue;
                polygonVertices.Add(segments[i].P2);
            }

            float minx = polygonVertices.Select(p => p.X).Min();
            float maxx = polygonVertices.Select(p => p.X).Max();
            float miny = polygonVertices.Select(p => p.Y).Min();
            float maxy = polygonVertices.Select(p => p.Y).Max();

            float x = (maxx + minx) / 2;
            float y = (maxy + miny) / 2;
            PointF result = new PointF(x, y);

            foreach (Circle circle in circles)
            {
                if (Math.Abs(new Segment(circle.Center, result).Distance - circle.Radius) > 5)
                    return new PointF();
            }

            return result;
        }

        public static PointF[] GetCircleIntersections(Circle c1, Circle c2)
        {
            float x1 = c1.Center.X;
            float y1 = c1.Center.Y;
            float x2 = c2.Center.X;
            float y2 = c2.Center.Y;
            float r1 = c1.Radius;
            float r2 = c2.Radius;
            float d = Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            if ((d > r1 + r2) || (d == 0 && r1 == r2) || (d + Math.Min(r1, r2) < Math.Max(r1, r2)))
                return new PointF[0];

            float a = (r1 * r1 - r2 * r2 + d * d) / (2.0f * d);
            float h = Sqrt(r1 * r1 - a * a);

            PointF p2 = new PointF(x1 + (a * (x2 - x1)) / d, y1 + (a * (y2 - y1)) / d);
            List<PointF> list = new List<PointF>();
            list.Add(new PointF((float)Math.Round(p2.X + (h * (y2 - y1) / d), 2), (float)Math.Round(p2.Y - (h * (x2 - x1) / d), 2)));
            list.Add(new PointF((float)Math.Round(p2.X - (h * (y2 - y1) / d), 2), (float)Math.Round(p2.Y + (h * (x2 - x1) / d), 2)));
            if (d == r1 + r2)
                return new PointF[0];
            return list.ToArray();

            //PointD center1 = new PointD(c1.Center.X, c1.Center.Y);
            //decimal radius1 = c1.Radius;
            //PointD center2 = new PointD(c2.Center.X, c2.Center.Y);
            //decimal radius2 = c2.Radius;

            ////PointF center1, PointF center2, double radius1, double? radius2 = null
            //var (r1, r2) = (radius1, radius2);
            //(decimal x1, decimal y1, decimal x2, decimal y2) = (center1.X, center1.Y, center2.X, center2.Y);
            //// d = distance from center1 to center2
            //decimal d = SqrtD(SqrD(x1 - x2) + SqrD(y1 - y2));
            //// Return an empty array if there are no intersections
            //if (!(Math.Abs(r1 - r2) <= d && d <= r1 + r2)) { return new PointD[0]; }

            //// Intersections i1 and possibly i2 exist
            //var dsq = d * d;
            //var (r1sq, r2sq) = (r1 * r1, r2 * r2);
            //var r1sq_r2sq = r1sq - r2sq;
            //var a = r1sq_r2sq / (2 * dsq);
            //var c = SqrtD(2 * (r1sq + r2sq) / dsq - (r1sq_r2sq * r1sq_r2sq) / (dsq * dsq) - 1);

            //var fx = (x1 + x2) / 2 + a * (x2 - x1);
            //var gx = c * (y2 - y1) / 2;

            //var fy = (y1 + y2) / 2 + a * (y2 - y1);
            //var gy = c * (x1 - x2) / 2;

            //var i1 = new PointD((fx + gx), (fy + gy));
            //var i2 = new PointD((fx - gx), (fy - gy));

            //return i1 == i2 ? new PointD[] { new PointD(i1.X, i1.Y) } : new PointD[] { new PointD(i1.X, i1.Y), new PointD(i2.X, i2.Y) };
        }

        private static int CompareLinesByLength(Segment x, Segment y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    return x.Distance.CompareTo(y.Distance);
                }
            }
        }
    }

    public class Circle : IEquatable<Circle>
    {
        public PointF Center;
        public float Radius;

        public float X => Center.X;
        public float Y => Center.Y;

        public Circle(PointF origin, float radius)
        {
            Center = origin;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            Center = new PointF(x, y);
            Radius = radius;
        }

        public bool Equals(Circle other)
        {
            return Center.Equals(other.Center) && Radius.Equals(other.Radius);
        }

        public override string ToString()
        {
            return $"{Center}:{Radius}";
        }
    }

    public class Segment : IEquatable<Segment>
    {
        public PointF P1;
        public PointF P2;

        public float X1 => P1.X;
        public float Y1 => P1.Y;
        public float X2 => P2.X;
        public float Y2 => P2.Y;

        public float Distance => (float)Math.Sqrt(Math.Pow(P1.X - P2.X, 2) + Math.Pow(P1.Y - P2.Y, 2));

        public Segment(PointF p1, PointF p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public bool Equals(Segment other)
        {
            return P1.Equals(other.P1) && P2.Equals(other.P2);
        }

        public override string ToString()
        {
            return $"{P1}:{P2}";
        }
    }
}