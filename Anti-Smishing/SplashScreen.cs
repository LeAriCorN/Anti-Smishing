﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace Anti_Smishing
{
    [Activity(Label = "Anti-Smishing", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Thread.Sleep(450);
            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}