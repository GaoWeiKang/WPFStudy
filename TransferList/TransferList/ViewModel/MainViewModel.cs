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
            DispatcherHelper.Initialize();//MVVMLight�е�DispatcherHelper��ʼ��,Demo��û���õ�������ע�͵�

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
            FirstItems = new ObservableCollection<int>(Enumerable.Range(0, 30).OrderBy(x => Guid.NewGuid()));//����һϵ������
        }

        /// <summary>
        /// �����߳��������У���ֹTransferCommand�������
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

                        //ͨ��Task/�̳߳ش��������߳�
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < 3; i++)
                        {
                            taskList.Add(Task.Factory.StartNew(TransferItem));
                        }

                        //�첽�ȴ�ȫ���߳�ִ�����
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
                    int n = new Random(unchecked((int)DateTime.Now.Ticks)).Next(0, FirstItems.Count);//�����ȡ��һ��
                    item = FirstItems[n];
                    //��UI�߳���ͬ��ִ�У�������ܳ�������Խ��(������UI�̣߳������м���sleep���Ժ����Կ�����)
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