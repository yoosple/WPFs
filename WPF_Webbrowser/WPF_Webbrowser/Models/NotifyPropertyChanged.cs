using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_Webbrowser.Models
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 바인딩 프로퍼티의 변경을 통지 합니다.
        /// <para>propertyName 파라미터가 빈 값일 경우 호출된 멤버의 이름을 인식하여 동작합니다.</para>
        /// </summary>
        /// <param name="propertyName">프로퍼티 이름</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
