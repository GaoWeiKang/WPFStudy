using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace TransferList
{
    public static class UIDispatcherHelper
    {
        public static Dispatcher UI_Dispatcher
        {
            get;
            private set;
        }

        /// <summary>
        /// 必须在App.xaml.cs中初始化,获取当前UI线程
        /// </summary>
        public static void Initialize()
        {
            if(UI_Dispatcher!=null&&UI_Dispatcher.Thread.IsAlive)
            {
                return;
            }
            UI_Dispatcher = Dispatcher.CurrentDispatcher;
            //UI_Dispatcher = Application.Current.Dispatcher;
        }

        public static void CheckInvokeOnUI(Action action)
        {
            if(action==null)
            {
                return;
            }
            if (UI_Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UI_Dispatcher.Invoke(action);
            }
        }

        public static void CheckBeginInvokeOnUI(Action action)
        {
            if(action==null)
            {
                return;
            }
            if (UI_Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UI_Dispatcher.BeginInvoke(action);
            }
                
        }
    }
}
