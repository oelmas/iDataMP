﻿using GMap.NET;
using Newtonsoft.Json;
using System;
using static System.Math;

namespace MissionPlanner.Utilities
{
    public class Vector3f : Vector3<float>
    {
        public static readonly Vector3f Zero = new Vector3f(0, 0, 0);
        public static readonly Vector3f One = new Vector3f(1.0f, 1.0f, 1.0f);

        [JsonConstructor]
        public Vector3f(float x = 0, float y = 0, float z = 0) : base(x, y, z)
        {
        }
    }

    public class Vector3 : Vector3<double>
    {
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 One = new Vector3(1.0, 1.0, 1.0);

        [JsonConstructor]
        public Vector3(double x = 0, double y = 0, double z = 0) : base(x, y, z)
        {
        }

        public Vector3(Vector3<double> copyme) : base(copyme)
        {
        }
    }

    public class Vector3<T> : IEquatable<Vector3<T>>, IFormattable where T : struct
    {
        public T x;
        public T y;
        public T z;

        [JsonIgnore]
        public double xd
        {
            get { return (double) (dynamic) x; }
        }
        [JsonIgnore]
        public double yd
        {
            get { return (double) (dynamic) y; }
        }
        [JsonIgnore]
        public double zd
        {
            get { return (double) (dynamic) z; }
        }

        [JsonIgnore]
        public T X
        {
            get { return x; }
            set { x = value; }
        }

        [JsonIgnore]
        public T Y
        {
            get { return y; }
            set { y = value; }
        }

        [JsonIgnore]
        public T Z
        {
            get { return z; }
            set { z = value; }
        }

        [JsonConstructor]
        public Vector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Tuple<T, T, T> inp)
        {
            this.x = inp.Item1;
            this.y = inp.Item2;
            this.z = inp.Item3;
        }

        public Vector3(Vector3<T> copyme)
        {
            this.x = copyme.x;
            this.y = copyme.y;
            this.z = copyme.z;
        }

        private Vector3(double x, double y, double z)
        {
            this.x = (T) (dynamic) x;
            this.y = (T) (dynamic) y;
            this.z = (T) (dynamic) z;
        }

        public bool Equals(Vector3<T> other)
        {
            if (x.Equals(other.x) &&
                y.Equals(other.y) &&
                z.Equals(other.z))
                return true;
            return false;
        }

        public new string ToString()
        {
            return String.Format("Vector3<T>({0}, {1}, {2})", x, y, z);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("Vector3<T>({0}, {1}, {2})", x, y, z);
        }

        public static implicit operator Vector3<T>(PointLatLngAlt a)
        {
            return new Vector3<T>((T) (dynamic) a.Lat, (T) (dynamic) a.Lng, (T) (dynamic) a.Alt);
        }

        public static implicit operator Vector3<T>(PointLatLng a)
        {
            return new Vector3<T>((T) (dynamic) a.Lat, (T) (dynamic) a.Lng, (T) (dynamic) 0);
        }

        public static implicit operator PointLatLng(Vector3<T> a)
        {
            return new PointLatLng((double) (dynamic) a.x, (double) (dynamic) a.y);
        }

        public static implicit operator T[](Vector3<T> a)
        {
            return new T[] {a.x, a.y, a.z};
        }

        public static implicit operator Vector3(Vector3<T> a)
        {
            return new Vector3((double) (dynamic) a.x, (double) (dynamic) a.y, (double) (dynamic) a.z);
        }

        public static implicit operator Vector3f(Vector3<T> a)
        {
            var x = (float) (dynamic) a.x;
            var y = (float) (dynamic) a.y;
            var z = (float) (dynamic) a.z;

            return new Vector3f(x, y, z);
        }

        public T this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                if (index == 1)
                    return y;
                if (index == 2)
                    return z;

                throw new Exception("Bad index");
            }
            set
            {
                if (index == 0)
                {
                    x = value;
                    return;
                }

                if (index == 1)
                {
                    y = value;
                    return;
                }

                if (index == 2)
                {
                    z = value;
                    return;
                }

                throw new Exception("Bad index");
            }
        }

        public static Vector3<T> operator +(Vector3<T> self, Vector3<T> v)
        {
            return new Vector3<T>((T) (dynamic) (self.xd + v.xd),
                (T) (dynamic) (self.yd + v.yd),
                (T) (dynamic) (self.zd + v.zd));
        }

        public static Vector3<T> operator -(Vector3<T> self, Vector3<T> v)
        {
            return new Vector3<T>((T) (dynamic) (self.xd - v.xd),
                (T) (dynamic) (self.yd - v.yd),
                (T) (dynamic) (self.zd - v.zd));
        }

        public static Vector3<T> operator -(Vector3<T> self)
        {
            return new Vector3<T>((T) (dynamic) (-self.xd), (T) (dynamic) (-self.yd),
                (T) (dynamic) (-self.zd));
        }

        public static double operator *(Vector3<T> self, Vector3<T> v)
        {
            //  '''dot product'''
            return (self.xd * v.xd + self.yd * v.yd + self.zd * v.zd);
        }

        public static Vector3<T> operator *(Vector3<T> self, double v)
        {
            return new Vector3<T>(self.xd * v,
                self.yd * v,
                self.zd * v);
        }

        public static Vector3<T> operator *(double v, Vector3<T> self)
        {
            return (self * v);
        }

        public static Vector3<T> operator /(Vector3<T> self, double v)
        {
            return new Vector3<T>(self.xd / v,
                self.yd / v,
                self.zd / v);
        }

        public static Vector3<T> operator %(Vector3<T> self, Vector3<T> v)
        {
            //  '''cross product'''
            return new Vector3<T>((self.yd * v.zd - self.zd * v.yd),
                (self.zd * v.xd - self.xd * v.zd),
                (self.xd * v.yd - self.yd * v.xd));
        }

        public Vector3<T> copy()
        {
            return new Vector3<T>(x, y, z);
        }

        public T length()
        {
            return (T) (dynamic) Sqrt((xd * xd + yd * yd + zd * zd));
        }

        public void zero()
        {
            x = y = z = (T) (dynamic) 0;
        }

        //public double angle (Vector3<T> self, Vector3<T> v) {
        //   '''return the angle between this vector and another vector'''
        //  return Math.Acos(self * v) / (self.length() * v.length());
        //}

        public Vector3<T> normalized()
        {
            return this / (double) (dynamic) length();
        }

        public void normalize()
        {
            Vector3<T> v = normalized();
            x = v.x;
            y = v.y;
            z = v.z;
        }

        const double rad2deg = (180 / PI);
        const double deg2rad = (1.0 / rad2deg);

        private double HALF_SQRT_2
        {
            get { return 0.70710678118654757; }
        }

        public Vector3<T> rotate(double rotation)
        {
            rotation *= deg2rad;
            var x1 = xd * Cos(rotation) - yd * Sin(rotation);
            var y1 = xd * Sin(rotation) + yd * Cos(rotation);

            x = (T) (dynamic) x1;
            y = (T) (dynamic) y1;

            return this;
        }

        public Vector3<T> rotate(Rotation rotation)
        {
            T tmp;
            switch (rotation)
            {
                case Rotation.ROTATION_NONE:
                case Rotation.ROTATION_MAX:
                    return this;

                case Rotation.ROTATION_YAW_45:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd + yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_90:
                {
                    tmp = x;
                    x = (T) (dynamic) (-yd);
                    y = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_135:
                {
                    tmp = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_180:
                    x = (T) (dynamic) (-xd);
                    y = (T) (dynamic) (-yd);
                    return this;

                case Rotation.ROTATION_YAW_225:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (yd - xd));
                    y = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_YAW_270:
                {
                    tmp = x;
                    x = y;
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    return this;
                }
                case Rotation.ROTATION_YAW_315:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (yd - xd));
                    x = tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_180:
                {
                    y = (T) (dynamic) (-yd);
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_45:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    x = tmp;
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_90:
                {
                    tmp = x;
                    x = y;
                    y = tmp;
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_135:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (yd - xd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (yd + xd));
                    x = tmp;
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_PITCH_180:
                {
                    x = (T) (dynamic) (-xd);
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_225:
                {
                    tmp = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (yd - xd));
                    x = (T) (dynamic) tmp;
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_270:
                {
                    tmp = (T) (dynamic) xd;
                    x = (T) (dynamic) (-yd);
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_180_YAW_315:
                {
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    x = (T) (dynamic) tmp;
                    z = (T) (dynamic) (-zd);
                    return this;
                }
                case Rotation.ROTATION_ROLL_90:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) yd;
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_45:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) yd;
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd + yd));
                    x = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_90:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) yd;
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    tmp = (T) (dynamic) xd;
                    x = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_90_YAW_135:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) yd;
                    y = (T) (dynamic) (-(double) (dynamic) tmp);
                    tmp = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    x = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_45:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    tmp = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd + yd));
                    x = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_90:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    tmp = (T) (dynamic) xd;
                    x = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_ROLL_270_YAW_135:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) (-yd);
                    y = (T) (dynamic) tmp;
                    tmp = (T) (dynamic) (-HALF_SQRT_2 * (xd + yd));
                    y = (T) (dynamic) (HALF_SQRT_2 * (xd - yd));
                    x = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_90:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) (-xd);
                    x = (T) (dynamic) tmp;
                    return this;
                }
                case Rotation.ROTATION_PITCH_270:
                {
                    tmp = (T) (dynamic) zd;
                    z = (T) (dynamic) xd;
                    x = (T) (dynamic) (-(double) (dynamic) tmp);
                    return this;
                }
            }

            throw new Exception("Invalid Rotation");
            //return this;
        }

    }

    /// <summary>
    /// from libraries\AP_Math\rotations.h
    /// </summary>
    public enum Rotation
    {
        ROTATION_NONE = 0,
        ROTATION_YAW_45,
        ROTATION_YAW_90,
        ROTATION_YAW_135,
        ROTATION_YAW_180,
        ROTATION_YAW_225,
        ROTATION_YAW_270,
        ROTATION_YAW_315,
        ROTATION_ROLL_180,
        ROTATION_ROLL_180_YAW_45,
        ROTATION_ROLL_180_YAW_90,
        ROTATION_ROLL_180_YAW_135,
        ROTATION_PITCH_180,
        ROTATION_ROLL_180_YAW_225,
        ROTATION_ROLL_180_YAW_270,
        ROTATION_ROLL_180_YAW_315,
        ROTATION_ROLL_90,
        ROTATION_ROLL_90_YAW_45,
        ROTATION_ROLL_90_YAW_90,
        ROTATION_ROLL_90_YAW_135,
        ROTATION_ROLL_270,
        ROTATION_ROLL_270_YAW_45,
        ROTATION_ROLL_270_YAW_90,
        ROTATION_ROLL_270_YAW_135,
        ROTATION_PITCH_90,
        ROTATION_PITCH_270,
        ROTATION_MAX
    }
}