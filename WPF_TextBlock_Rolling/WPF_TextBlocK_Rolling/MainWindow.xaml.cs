using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF_TextBlocK_Rolling
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private double rollingTextOriginalLeft;
        public MainWindow()
        {
            InitializeComponent();
            rollingTextOriginalLeft = rollingText.ActualWidth;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartRollingAnimation();
        }

        private void StartRollingAnimation()
        {
            if (rollingText.ActualWidth > canvas.ActualWidth)
            {

                DoubleAnimation rollingAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = rollingText.ActualWidth,
                    Duration = new Duration(TimeSpan.FromSeconds(30)),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                rollingText.BeginAnimation(Canvas.LeftProperty, rollingAnimation);
            }
            else
            {
                // rollingText의 애니메이션 중지
                rollingText.BeginAnimation(Canvas.LeftProperty, null);

                // rollingText의 위치를 조정하여 텍스트가 고정적으로 표시되도록 함
                Canvas.SetLeft(rollingText, 0);
                //가운데 정렬 표시
                //double left = (canvas.ActualWidth - rollingText.ActualWidth) / 2;
            }
        }

        private void rollingText_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StartRollingAnimation();
        }
    }
}