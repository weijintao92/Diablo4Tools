using GalaSoft.MvvmLight.Messaging;
using game_tools.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;

namespace game_tools.ViewModel
{
    public  class HomePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Equipments> _entities;
        private SQLiteDb sQLiteDb;

 
        public ObservableCollection<Equipments> Entities
        {
            get { return _entities; }
            set
            {
                _entities = value;
                OnPropertyChanged(nameof(Entities));
            }
        }

        public HomePageViewModel(SQLiteDb sQLiteDb)
        {

            this.sQLiteDb = sQLiteDb;
            // 初始化数据
            LoadData();
            Messenger.Default.Register<HomePageRefreshMessage>(this, Refresh);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="message"></param>
        private void Refresh(HomePageRefreshMessage message)
        {
            // 重新加载数据
            LoadData();
        }

        private void LoadData()
        {
            // 从数据库加载数据到 Entities
            Entities = new ObservableCollection<Equipments>(this.sQLiteDb.Equipments.ToList().OrderByDescending(item => item.TaskBatch));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
