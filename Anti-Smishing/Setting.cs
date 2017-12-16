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

namespace Anti_Smishing
{
    [Activity(Label = "Setting")]
    public class Setting : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setting);

            //프레그먼트매니저를 통한 프레퍼런스세팅 불러오기
            FragmentManager.BeginTransaction().Add(Resource.Id.Pref_Frame, new Setting_Fragment()).Commit();
            
        }
    }
}