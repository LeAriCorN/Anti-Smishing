
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

//FlipProgress API
using Com.Taishi.Flipprogressdialog;

//VirusTotal API
using VirusTotalNET;
using VirusTotalNET.Results;
using VirusTotalNET.Objects;
using VirusTotalNET.ResponseCodes;

namespace Anti_Smishing
{
    [Activity(Label = "Anti_Smishing", Theme = "@style/Theme.AppCompat.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        //바이러스 토탈 API 변수
        private static string ScanUrl;
        static bool hasUrlBeenScannedBefore;

        public static int fcnt;
        public static int total;
        public static string ScanId;

        //설정 변수 불값
        private bool Copy_onoff;
        private bool True_onoff;
        private bool Listen_onoff;

        //플립 API 이미지 리스트
        List<Java.Lang.Integer> imgList = new List<Java.Lang.Integer>();
        private static Android.App.AlertDialog.Builder builder;
        private static Toast mtoast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            
            //토스트 메세지, 얼럿 다이얼로그 미리 생성
            mtoast = Toast.MakeText(this, "", ToastLength.Short);
            builder = new Android.App.AlertDialog.Builder(this,Resource.Style.AlertDialogStyle);

            imgList.Add(Java.Lang.Integer.ValueOf(Resource.Drawable.loading1));
            imgList.Add(Java.Lang.Integer.ValueOf(Resource.Drawable.loading2));
            
            //기본 설정 프레퍼런스 값 로딩, Setting에서 설정한 값 넘겨받기.
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            Copy_onoff = prefs.GetBoolean("Switch_ClipboardListen", true);
            True_onoff = prefs.GetBoolean("Switch_TrueSite", true);
            Listen_onoff = prefs.GetBoolean("Switch_KnowCliphasURL", true);

            //서비스 온오프
            if (Listen_onoff == true)
            {
                StartService(new Android.Content.Intent(this, typeof(Go2AppSvc)));
            }
            else
            {
                StopService(new Android.Content.Intent(this, typeof(Go2AppSvc)));
            }

            //자동 복사 붙여넣기
            if (Copy_onoff == true)
            {
                ClipboardAutoCopy();
            }

            Button btn_analyze = (Button)FindViewById(Resource.Id.btn_analyze);
            ImageButton btn_setting = (ImageButton)FindViewById(Resource.Id.btn_setting);

            btn_setting.Click += Btn_setting_Click;
            btn_analyze.Click += Btn_analyze_Click;

        }

        //버튼 클릭 이벤트
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
                FlipProgressDialog dlg = new FlipProgressDialog();
                dlg.SetImageList(imgList);
                dlg.SetOrientation("rotationY");
                dlg.SetBackgroundAlpha(0);
                dlg.SetDimAmount(0.6f);
                dlg.SetDuration(1600);
                dlg.Show(FragmentManager, "");

                //쓰레드를 이용 API 작동
                new Thread(new ThreadStart(delegate
                {
                    runAPI().Wait();

                    RunOnUiThread(() => { dlg.Dismiss(); URL_Repo();});

                })).Start();
            }
            else
            {
                mtoast.SetText("  URL을 입력하지 않았거나 \n잘못된 URL을 입력했습니다");
                mtoast.Show();
            }

           
        }
        
        //클립보드 자동 카피시 플레인 텍스트만 추출
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



            //바이러스 토탈에서 과거 분석 내역 있으면 과거 분석 내역 갖고오기
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

        //URL 분석결과중 ScanId, 총 분석한 엔진갯수 갖고오기
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
            EditText URL = FindViewById<EditText>(Resource.Id.txt_input_url);

            aTitle = "분석 결과";

            if (URL.Text == "test.com")
            {
                if (fcnt >= 1)
                {
                    aMsg = total + "개에서 " + fcnt + "개가 악성으로 진단했습니다";
                }
                else
                {
                    aMsg = "안전한 URL 입니다";
                }

                builder.SetTitle(aTitle);
                builder.SetMessage(aMsg);
                builder.SetIcon(Resource.Drawable.icon_talk);
                builder.SetCancelable(true);
                builder.SetPositiveButton("확인", delegate {
                    builder.SetTitle("원본 사이트 안내");
                    builder.SetMessage("이 URL의 원본 사이트로 안내 받으실 수 있습니다. 아래 이동버튼을 누르면 원본 사이트로 이동합니다");
                    builder.SetIcon(Resource.Drawable.icon_talk);
                    builder.SetCancelable(true);
                    builder.SetPositiveButton("확인", delegate { });
                    builder.SetNeutralButton("안전한 사이트로 이동", delegate {
                        var uri = Android.Net.Uri.Parse("http://lms.dju.ac.kr");
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    });
                    builder.Show();
                });
                builder.Show();

                

            }
            else
            {
                if (fcnt >= 1)
                {
                    aMsg = total + "개에서 " + fcnt + "개가 악성으로 진단했습니다";
                }
                else
                {
                    aMsg = "안전한 URL 입니다";
                }

                builder.SetTitle(aTitle);
                builder.SetMessage(aMsg);
                builder.SetIcon(Resource.Drawable.icon_talk);
                builder.SetCancelable(true);
                builder.SetPositiveButton("확인", delegate { });
                builder.Show();
            }
        }

        //URL이 맞는지 확인
        private bool isUrlReal(string source)
        {
            return Android.Util.Patterns.WebUrl.Matcher(source).Matches();
        }

        //결과값 다이얼로그로 내보내기
        private void URL_Repot()
        {
            string aTitle = ""; string aMsg = "";
            string Url = "";
            EditText URL = FindViewById<EditText>(Resource.Id.txt_input_url);

            aTitle = "분석 결과";
            
            if (fcnt >= 1)
            {
                Url=Database.GetURL(ScanId);

                aMsg = total + "개에서 " + fcnt + "개가 악성으로 진단했습니다";

                builder.SetTitle(aTitle);
                builder.SetMessage(aMsg);
                builder.SetIcon(Resource.Drawable.icon_talk);
                builder.SetCancelable(true);
                builder.SetPositiveButton("확인", delegate {

                    //확인을 눌렀을때 새로운 다이얼로그 얼럿 생성
                    builder.SetTitle("공식 사이트 안내");
                    builder.SetMessage("이 URL의 원래 사이트로 안내 받으실 수 있습니다. 아래 이동버튼을 누르면 본래 사이트로 이동합니다");
                    builder.SetIcon(Resource.Drawable.icon_talk);
                    builder.SetCancelable(true);
                    builder.SetPositiveButton("확인", delegate { });
                    builder.SetNeutralButton("공식 사이트로 이동", delegate {
                        //인텐트로 Url 접속 실행
                        var uri = Android.Net.Uri.Parse(Url);
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                    });
                    builder.Show();
                });
                builder.Show();
            }
            else
            {
                aMsg = "안전한 URL 입니다";

                builder.SetTitle(aTitle);
                builder.SetMessage(aMsg);
                builder.SetIcon(Resource.Drawable.icon_talk);
                builder.SetCancelable(true);
                builder.SetPositiveButton("확인", delegate { });
                builder.Show();
            }
                     

        }


    }
}

