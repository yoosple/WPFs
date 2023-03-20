using System;
using System.Windows;
using System.Windows.Input;
using WPF_Webbrowser.Models;

namespace WPF_Webbrowser.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private ICommand _ButtonCommand;
        private ICommand _MyEventCommand;
        private string _MapUrl;
        private string _ExecuteScript;
        private string _InputText;

        public string MapUrl
        {
            get { return _MapUrl; }
            set
            {
                _MapUrl = value;
                OnPropertyChanged();
            }
        }

        public string ExecuteScript
        {
            get { return _ExecuteScript; }
            set
            {
                _ExecuteScript = value;
                OnPropertyChanged();
            }
        }

        public string InputText
        {
            get { return _InputText; }
            set
            {
                _InputText = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonCommand
        {
            get
            {
                if (_ButtonCommand == null)
                {
                    _ButtonCommand = new RelayCommand<string>(obj => ButtonAction(obj));
                }
                return _ButtonCommand;
            }
        }

        public ICommand MyEventCommand
        {
            get
            {
                if (_MyEventCommand == null)
                {
                    _MyEventCommand = new RelayCommand<MyEventArgs>(obj => MyEventAction(obj));
                }
                return _MyEventCommand;
            }
        }

        public MainViewModel()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Resources\test.html";
            Uri uri = new Uri(@"file:///" + path);
            MapUrl = uri.ToString();
        }

        /// <summary>
        /// ButtonCommand의 동작을 수행합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected void ButtonAction(string obj)
        {
            Console.WriteLine("ButtonAction : {0}", obj);

            switch(obj)
            {
                case "ToJavaStript":
                    ExecuteScript = $"helloWorld('{InputText}');";
                    break;
            }
        }

        /// <summary>
        /// MyEventCommand의 동작을 수행합니다.
        /// </summary>
        /// <param name="obj"></param>
        protected void MyEventAction(MyEventArgs obj)
        {
            MessageBox.Show(obj.Data);
        }
    }
}
