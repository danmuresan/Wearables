using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Commons.Models
{
    public class Rotation
    {
        /// <summary>
        /// Angular speed around x-axis
        /// </summary>
        public double X_Roll { get; }

        /// <summary>
        /// Angular speed around y-axis
        /// </summary>
        public double Y_Pitch { get; }

        /// <summary>
        /// Angular speed around z-axis
        /// </summary>
        public double Z_Yawn { get; }

        public Rotation(double x, double y, double z)
        {
            X_Roll = x;
            Y_Pitch = y;
            Z_Yawn = z;
        }
    }
}