using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.Design;
using System.Threading;
using System.Threading.Tasks;
using VirusTotalNET;
using VirusTotalNET.Results;
using VirusTotalNET.ResponseCodes;
using System.Collections.Generic;
using VirusTotalNET.Objects;
using Android.Content;
using Android.Preferences;

namespace Anti_Smishing
{
    [Activity(Label = "Anti_Smishing")]
    public class MainActivity : Activity
    {
        private static string ScanUrl;
        static bool hasUrlBeenScannedBefore;

        public static int fcnt;
        public static int total;
        public static string ScanId;

        private bool Clip_onoff;
        private bool True_onoff;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mtoast = Toast.MakeText(this, "", ToastLength.Short);
            builder = new AlertDialog.Builder(this);
            SetContentView(Resource.Layout.Main);
            
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            Clip_onoff = prefs.GetBoolean("Switch_ClipboardListen", true);
            True_onoff = prefs.GetBoolean("Switch_TrueSite", true);

            if (Clip_onoff == true)
            {
                ClipboardAutoCopy();
            }

            Button btn_analyze = (Button)FindViewById(Resource.Id.btn_analyze);
            ImageButton btn_setting = (ImageButton)FindViewById(Resource.Id.btn_setting);

            btn_setting.Click += Btn_setting_Click;
            btn_analyze.Click += Btn_analyze_Click;

        }

        private void Btn_setting_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(Setting));
        }

        private void Btn_analyze_Click(object sender, System.EventArgs e)
        {
            EditText URL = FindViewById<EditText>(Resource.Id.txt_input_url);

            ScanUrl = URL.Text;

            if (isUrlReal(ScanUrl))
            {
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetMessage("URL을 분석중입니다");
                progress.SetCancelable(true);
                progress.Show();

                new Thread(new ThreadStart(delegate
                {
                    runAPI().Wait();

                    RunOnUiThread(() => { progress.Hide(); URL_Repo();});

                })).Start();
            }
            else
            {
                mtoast.SetText("  URL을 입력하지 않았거나 \n잘못된 URL을 입력했습니다");
                mtoast.Show();
            }

           
        }

        private void ClipboardAutoCopy()
        {
            EditText upeditor = FindViewById<EditText>(Resource.Id.txt_input_url);
            var clipboard = (Android.Content.ClipboardManager)GetSystemService(ClipboardService);

            var pData = "";

            if (!(clipboard.HasPrimaryClip))
            {
                //데이터 존재시 핸들링
            }
            else if (!(clipboard.PrimaryClipDescription.HasMimeType(Android.Content.ClipDescription.MimetypeTextPlain)))
            {
                // 클립보드 내 데이터 플레인 텍스트 필터
            }
            else
            {
                //플레인 텍스트만 추출
                var item = clipboard.PrimaryClip.GetItemAt(0);
                pData = item.Text;

                if (isUrlReal(pData))
                {
                    upeditor.Text = pData.ToString();
                    mtoast.SetText("URL을 자동으로 입력했습니다");
                    mtoast.Show();
                }
            }
        }

        private static async Task runAPI()
        {
            VirusTotal virusTotal = new VirusTotal("7c2ea7a71fa28fe564f9d6ffb63ac6ca11984067052e2fa40bc9cdec24d232f7");

            //https 사용
            virusTotal.UseTLS = true;

            UrlReport urlReport = await virusTotal.GetUrlReport(ScanUrl);

            hasUrlBeenScannedBefore = urlReport.ResponseCode == ReportResponseCode.Present;



            //If the url has been scanned before, the results are embedded inside the report.
            if (hasUrlBeenScannedBefore)
            {
                PrintScan(urlReport);
            }
            else
            {
                UrlScanResult urlResult = await virusTotal.ScanUrl(ScanUrl);
                //PrintScan(urlResult);
                await Task.Delay(500);

            }
        }

        private static void PrintScan(UrlReport urlReport)
        {
            int temp1 = 0;

            fcnt = 0;
            total = urlReport.Total;
            ScanId = urlReport.ScanId;

            if (urlReport.ResponseCode == ReportResponseCode.Present)
            {
                foreach (KeyValuePair<string, ScanEngine> scan in urlReport.Scans)
                {
                    if (scan.Value.Detected)
                    {
                        temp1 += 1;
                    }
                }
                fcnt = temp1;
            }


        }

        private void URL_Repo()
        {
            string aTitle = ""; string aMsg = "";

            aTitle = "결과 안내";
            aMsg = "탐지 결과 : " + fcnt + " / " + total + "\n" 
                + "" + total + "개의 검사 중 " + fcnt + "개에서 악성으로 판정\n"+ScanId;

            builder.SetTitle(aTitle);
            builder.SetMessage(aMsg);
            builder.SetCancelable(true);
            builder.SetPositiveButton("확인", delegate { });
            builder.Show();
        }

        private bool isUrlReal(string source)
        {
            return Android.Util.Patterns.WebUrl.Matcher(source).Matches();
        }
        
        private static Toast mtoast;
        private static AlertDialog.Builder builder;
    }
}

