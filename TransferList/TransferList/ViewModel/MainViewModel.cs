using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TransferList.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            DispatcherHelper.Initialize();//MVVMLight中的DispatcherHelper初始化,Demo中没有用到，可以注释掉

            FirstItems = new ObservableCollection<int>();
            SecondItems = new ObservableCollection<int>();

            InitLists();
        }

        private ObservableCollection<int> _firstItems;
        public ObservableCollection<int> FirstItems
        {
            get { return _firstItems; }
            set
            {
                Set(ref _firstItems, value);
            }
        }

        private ObservableCollection<int> _secondItems;
        public ObservableCollection<int> SecondItems
        {
            get { return _secondItems; }
            set
            {
                Set(ref _secondItems, value);
            }
        }

        private void InitLists()
        {
            FirstItems.Clear();
            SecondItems.Clear();
            FirstItems = new ObservableCollection<int>(Enumerable.Range(0, 30).OrderBy(x => Guid.NewGuid()));//生成一系列整数
        }

        /// <summary>
        /// 代表线程正在运行，防止TransferCommand多次运行
        /// </summary>
        private bool _isRunning = false;

        private RelayCommand _transferCommand;
        public RelayCommand TransferCommand
        {
            get
            {
                return _transferCommand = _transferCommand ?? new RelayCommand(async () =>
                    {
                        _isRunning = true;
                        InitLists();

                        //通过Task/线程池创建工作线程
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < 3; i++)
                        {
                            taskList.Add(Task.Factory.StartNew(TransferItem));
                        }

                        //异步等待全部线程执行完毕
                        await Task.WhenAll(taskList);
                        _isRunning = false;
                    }, () => !_isRunning);
            }
        }

        private readonly object _lockObj = new object();

        private void TransferItem()
        {
            while(true)
            {
                int item;
                lock(_lockObj)
                {
                    if (FirstItems.Count == 0) break;
                    int n = new Random(unchecked((int)DateTime.Now.Ticks)).Next(0, FirstItems.Count);//随机抽取出一项
                    item = FirstItems[n];
                    //在UI线程上同步执行，否则可能出现索引越界(会阻塞UI线程，在其中加入sleep可以很明显看出来)
                    UIDispatcherHelper.CheckInvokeOnUI(() =>
                    {
                        FirstItems.RemoveAt(n);
                        //Thread.Sleep(100);
                    });
                }
                UIDispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    SecondItems.Add(item);
                });
                Thread.Sleep(100);
            }
        }
    }
}