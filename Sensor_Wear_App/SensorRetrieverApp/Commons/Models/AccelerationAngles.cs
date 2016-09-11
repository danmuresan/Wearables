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
    public class AccelerationAngles
    {
        /// <summary>
        /// Angular speed around x-axis
        /// </summary>
        public double Roll { get; }

        /// <summary>
        /// Angular speed around y-axis
        /// </summary>
        public double Pitch { get; }

        /// <summary>
        /// Angular speed around z-axis
        /// </summary>
        public double Yaw { get; }

        public AccelerationAngles(double roll, double pitch, double yaw)
        {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }
    }
}