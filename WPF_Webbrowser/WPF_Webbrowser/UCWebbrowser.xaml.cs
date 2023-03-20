using Microsoft.Win32;
using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPF_Webbrowser
{
    /// <summary>
    /// UCWebbrowser.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCWebbrowser : UserControl
    {
        public event EventHandler<MyEventArgs> OnGetJavastriptText;

        #region 의존 프로퍼티
        /// <summary>
        /// Map Url이 로드 되었는지 여부 입니다.
        /// </summary>
        public bool IsMapLoaded
        {
            get { return (bool)GetValue(IsMapLoadedProperty); }
            set { SetValue(IsMapLoadedProperty, value); }
        }

        /// <summary>
        /// WebBrowser에서 표시할 Map Url 입니다.
        /// </summary>
        public string MapUrl
        {
            get { return (string)GetValue(MapUrlProperty); }
            set { SetValue(MapUrlProperty, value); }
        }

        /// <summary>
        /// WebBrowser에서 실행할 Script 입니다. 할당 될 때마다 실행합니다.
        /// </summary>
        public string ExecuteScript
        {
            get { return (string)GetValue(ExecuteScriptProperty); }
            set { SetValue(ExecuteScriptProperty, value); }
        }

        /// <summary>
        /// WebBrowser에서 전달되는 도착지 정보를 받기위한 것 입니다.
        /// </summary>
        public MyEventArgs MyEventArgs
        {
            get { return (MyEventArgs)GetValue(MyEventArgsProperty); }
            set { SetValue(MyEventArgsProperty, value); }
        }

        public static readonly DependencyProperty IsMapLoadedProperty =
            DependencyProperty.Register("IsMapLoaded", typeof(bool), typeof(UCWebbrowser), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MapUrlProperty =
            DependencyProperty.Register("MapUrl", typeof(string), typeof(UCWebbrowser), new PropertyMetadata(string.Empty, MapUrlChanged));

        public static readonly DependencyProperty ExecuteScriptProperty =
            DependencyProperty.Register(
                "ExecuteScript",
                typeof(string),
                typeof(UCWebbrowser),
                new PropertyMetadata(ExecuteScriptChanged));

        public static readonly DependencyProperty MyEventArgsProperty =
            DependencyProperty.Register("MyEventArgs", typeof(MyEventArgs), typeof(UCWebbrowser), new PropertyMetadata(null));

        private static void MapUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCWebbrowser;

            if (e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                return;
            }

            control.WebBrowserMain.Navigate(e.NewValue.ToString());
        }

        private static void ExecuteScriptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UCWebbrowser;

            if (e.NewValue == null)
            {
                return;
            }

            if (e.NewValue != null && string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                return;
            }

            if (control.IsMapLoaded)
            {
                control.ExecuteJavaScript(control.WebBrowserMain, e.NewValue.ToString());
            }
        }
        #endregion

        public JavaScriptFuntion javaScriptFuntion;

        public UCWebbrowser()
        {
            InitializeComponent();
            SetUserAgent();
            javaScriptFuntion = new JavaScriptFuntion();
            javaScriptFuntion.OnMessageBoxShow += JavaScriptFuntion_OnMessageBoxShow;
            WebBrowserMain.ObjectForScripting = javaScriptFuntion;
        }

        private void WebBrowserMain_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            IsMapLoaded = true;
            dynamic activeX = WebBrowserMain.GetType().InvokeMember("ActiveXInstance", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic
                                                                    , null, WebBrowserMain, new object[] { });

            activeX.Silent = true;
        }


        private void JavaScriptFuntion_OnMessageBoxShow(object sender, EventArgs e)
        {
            var data = sender.ToString();
            MyEventArgs = new MyEventArgs(data);
            OnGetJavastriptText?.Invoke(this, MyEventArgs);
        }

        /// <summary>
        /// WebBrowser에서 스크립트를 실행합니다.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="script"></param>
        private void ExecuteJavaScript(WebBrowser web, string script)
        {
            try
            {
                Console.WriteLine("ExecuteJavaScript : {0}", script);
                Task.Factory.StartNew(delegate
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        web.InvokeScript("execScript", new object[] { script, "JavaScript" });
                    });
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            javaScriptFuntion.OnMessageBoxShow -= JavaScriptFuntion_OnMessageBoxShow;
            WebBrowserMain.Dispose();
        }

        /// <summary>
        /// WebBrowser가 사용하는 IE 엔진이 구버전일 때 스크립트 오류 발생 방지를 위한
        /// WebBrowser가 사용하는 IE 엔진이 최신 버전으로 갱신될 수 있게 UserAgent 속성을 설정
        /// </summary>
        private void SetUserAgent()
        {
            string regPath = @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            string programName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            try
            {
                using (var regKey = Registry.CurrentUser.OpenSubKey(regPath, true))
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer"))
                    {
                        string version = key.GetValue("svcVersion").ToString();
                        int ieVersion = 0;

                        if (version.StartsWith("11."))
                        {
                            ieVersion = 11001;
                        }
                        else if (version.StartsWith("10."))
                        {
                            ieVersion = 10001;
                        }
                        else if (version.StartsWith("9."))
                        {
                            ieVersion = 9999;
                        }
                        else
                        {
                            Console.WriteLine("현재 시스템에는 웹브라우저를 업그레이드 할 수 없습니다.");
                        }

                        // 키가 이미 등록되어 있는지 확인 후 등록
                        if (regKey.GetValue(programName) == null && ieVersion > 0)
                        {
                            regKey.SetValue(programName, ieVersion, RegistryValueKind.DWord);
                        }

                        key.Close();
                    }

                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    /// 자바스크립트에서 실행 가능한 C#의 메소드를 정의
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class JavaScriptFuntion
    {
        public event EventHandler OnMessageBoxShow;

        public void MessageBoxShow(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = "";
            }
            OnMessageBoxShow?.Invoke(text, null);
        }
    }

    public class MyEventArgs : EventArgs
    {
        public string Data { get; set; }

        public MyEventArgs(string data)
        {
            Data = data;
        }
    }
}
