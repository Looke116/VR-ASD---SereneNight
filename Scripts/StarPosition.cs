using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace stardata
{
    // NOTE: All parameter values are degrees. Return values as well. Conversion to radians is internal to the methods.
    static class StarPosition
    {

        public static float getAzimuth(float ra, float dec, float lat, float lon, DateTime time)
        {
            float altitude = getAltitude(ra, dec, lat, lon, time);
            float ha = getHA(ra, time.Year, time.Month, time.Day, time.Hour + time.Minute / 60f + time.Second / 3600f, lon);
            ha *= Mathf.Deg2Rad;
            ra *= Mathf.Deg2Rad;
            dec *= Mathf.Deg2Rad;
            altitude *= Mathf.Deg2Rad;
            lat *= Mathf.Deg2Rad;

            float A = Mathf.Acos((Mathf.Sin(dec) - Mathf.Sin(altitude) * Mathf.Sin(lat)) / (Mathf.Cos(altitude) * Mathf.Cos(lat))) * Mathf.Rad2Deg;
            return Mathf.Sin(ha) < 0 ? A : 360 - A;
            // return 180 + Mathf.Atan2(Mathf.Sin(ha), Mathf.Cos(ha) * Mathf.Sin(lat) - Mathf.Tan(dec) * Mathf.Cos(lat)) * Mathf.Rad2Deg;
        }

        public static float getAltitude(float ra, float dec, float lat, float lon, DateTime time)
        {
            float ha = getHA(ra, time.Year, time.Month, time.Day, time.Hour + time.Minute / 60f + time.Second / 3600f, lon);
            ra *= Mathf.Deg2Rad;
            dec *= Mathf.Deg2Rad;
            ha *= Mathf.Deg2Rad;
            lat *= Mathf.Deg2Rad;

            return Mathf.Asin(Mathf.Sin(dec) * Mathf.Sin(lat) + Mathf.Cos(dec) * Mathf.Cos(lat) * Mathf.Cos(ha)) * Mathf.Rad2Deg;
        }

        public static float ComputeLST(int year, int month, int day, float hour, float lon)
        {
            float d = 367 * year - (int)(7 * (year + (month + 9) / 12) / 4) + (275 * month) / 9 + day - 730531.5f;
            d += hour / 24;
            return Rev(100.46f + 0.985647f * d + lon + hour * 15.04107f);
        }

        private static float Rev(float x)
        {
            float rv;
            rv = x - ((int)x / 360) * 360;
            if (rv < 0)
            {
                rv = rv + 360;
            }
            return rv;
        }

        private static float getHA(float ra, int year, int month, int day, float hour, float longitude)
        {
            return Rev(ComputeLST(year, month, day, hour, longitude) - ra);
        }

    }

    public class Star
    {
        string name;
        float ra, dec, azimuth, altitude, visualMagnitude, distance, x, y, z;

        public Star(string name, float ra, float dec, float altitude, float azimuth, float visualMagnitude, float distance, float x, float y, float z)
        {
            this.name = name;
            this.ra = ra;
            this.dec = dec;
            this.altitude = altitude;
            this.azimuth = azimuth;
            this.visualMagnitude = visualMagnitude;
            this.distance = distance;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void setAzimuth(float azimuth)
        {
            this.azimuth = azimuth;
        }

        public void setAltitude(float altitude)
        {
            this.altitude = altitude;
        }

        public float getAzimuth()
        {
            return this.azimuth;
        }

        public float getAltitude()
        {
            return this.altitude;
        }

        public float getVisualMagnitude()
        {
            return this.visualMagnitude;
        }

        public float getRA()
        {
            return this.ra;
        }

        public float getDec()
        {
            return this.dec;
        }

        public float getDistance()
        {
            return this.distance;
        }

        public string getName()
        {
            return this.name;
        }

        public float getX()
        {
            return this.x;
        }

        public float getY()
        {
            return this.y;
        }

        public float getZ()
        {
            return this.z;
        }

        public void setX(float x)
        {
            this.x = x;
        }

        public void setY(float y)
        {
            this.y = y;
        }

        public void setZ(float z)
        {
            this.z = z;
        }
    }

    public static class Stars
    {
        private static List<Star> stars = new List<Star>();

        public static void addStar(string name, float ra, float dec, float altitude, float azimuth, float visualMagnitude, float distance, float x, float y, float z)
        {
            stars.Add(new Star(name, ra, dec, altitude, azimuth, visualMagnitude, distance, x, y, z));
        }

        public static Star GetStar(int index)
        {
            return stars[index];
        }

        public static void UpdateStar(int index, float altitude, float azimuth, float x, float y, float z)
        {
//            Debug.Log(index + " " + stars[index].getAltitude() + " " + stars[index].getAzimuth());
            stars[index].setAltitude(altitude);
            stars[index].setAzimuth(azimuth);
            stars[index].setX(x);
            stars[index].setY(y);
            stars[index].setZ(z);
            //           Debug.Log(index + " " + stars[index].getAltitude() + " " + stars[index].getAzimuth());
        }

        public static int getNumberOfStars()
        {
            return stars.Count;
        }

        public static float getVisualMagnitudeAfterExtinction(int index)
        {
            if (stars[index].getAltitude() < -2) // due to how the scene is rendered we sometimes see starts under the mathematical horizon so we make them dissapear by applying the extinction to stars below the horizon as well.
            {
                return stars[index].getVisualMagnitude();
            }
            else
            {
                float z = (90 - Mathf.Abs(stars[index].getAltitude())) * Mathf.Deg2Rad;
                float extinction = 0.2f / Mathf.Cos(z) * (1 - 0.0012f * (1 / (Mathf.Pow(Mathf.Cos(z),2) - 1))); // atmospheric extinction
                return stars[index].getVisualMagnitude() + extinction;
            }
        }
    }
}