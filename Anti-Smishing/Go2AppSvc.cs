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
    [Service]
    public class Go2AppSvc : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }


        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            //클립보드매니저를 이용, 시스템에서 클립보드가 변경됐을때 알림.
            ClipboardManager clip = (ClipboardManager)GetSystemService(Context.ClipboardService);
            clip.PrimaryClipChanged += URLinClip;

            return StartCommandResult.Sticky;
        }

        private void URLinClip(object sender, EventArgs e)
        {
            string chkUri;

            ClipboardManager clip = (ClipboardManager)GetSystemService(Context.ClipboardService);
            chkUri = clip.Text;

            if (chkurl(chkUri) == true)
            {
                Intent go = new Intent(this, typeof(Go2App));
                go.AddFlags(ActivityFlags.NewTask);
                StartActivity(go);
            }
            
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }

        private bool chkurl(string source)
        {
            return Android.Util.Patterns.WebUrl.Matcher(source).Matches();
        }
    }
}