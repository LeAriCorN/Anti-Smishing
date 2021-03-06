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
    [Activity(Theme = "@android:style/Theme.Translucent.NoTitleBar", NoHistory = true)]
    public class Go2App : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Go2AppBtn);
            
            Button gotoapp = (Button)FindViewById(Resource.Id.btn_Go2App);

            gotoapp.Click += Gotoapp_Click;

            //버튼이 떴을때 멈추는 시간 설정. 기본 1.5초.
            new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(1500);
                RunOnUiThread(() => { this.Finish(); });
            })).Start();

        }

        private void Gotoapp_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}