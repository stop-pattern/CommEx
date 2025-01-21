﻿using BveEx.Diagnostics;
using CommEx.Serial.Bids;
using CommEx.Serial.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Input;
using CommEx.Serial.Views;

namespace CommEx.Serial.ViewModels
{
    [XmlRoot("Settings")]
    public class ListViewModel: INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// 子要素の ViewModel
        /// </summary>
        private ObservableCollection<PortViewModel> portViewModels;

        /// <summary>
        /// 現在選択中の ViewModel
        /// </summary>
        private PortViewModel selectedPort;

        #endregion

        #region Properties

        /// <summary>
        /// 子要素の ViewModel
        /// </summary>
        [XmlArray("Port Settings")]
        [XmlArrayItem("Port")]
        public ObservableCollection<PortViewModel> PortViewModels
        {
            get { return portViewModels; }
            set
            {
                portViewModels = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 現在選択中の ViewModel
        /// </summary>
        [XmlIgnore]
        public PortViewModel SelectedPort
        {
            get { return selectedPort; }
            set
            {
                selectedPort = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsButtonAvailable");
                UpdateCommands();
            }
        }

        /// <summary>
        /// ボタンが利用可能か
        /// </summary>
        [XmlIgnore]
        public bool IsButtonAvailable => SelectedPort != null;

        /// <summary>
        /// 追加コマンド
        /// </summary>
        [XmlIgnore]
        public ICommand AddItemCommand { get; }

        /// <summary>
        /// 設定コマンド
        /// </summary>
        [XmlIgnore]
        public ICommand SettingCommand { get; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        [XmlIgnore]
        public ICommand DeleteItemCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// ListViewModel をデフォルト値で初期化
        /// </summary>
        public ListViewModel()
        {
            portViewModels = new ObservableCollection<PortViewModel>
            {
                new PortViewModel()
            };

            AddItemCommand = new RelayCommand(AddItem);
            SettingCommand = new RelayCommand(ShowSettingWindow, IsSelectedItemNotNull);
            DeleteItemCommand = new RelayCommand(ClearSelectedItem, IsSelectedItemNotNull);
        }

        /// <summary>
        /// ListViewModel を <see cref="PortViewModel"/> で初期化
        /// </summary>
        public ListViewModel(PortViewModel viewModel)
        {
            portViewModels = new ObservableCollection<PortViewModel>
            {
                viewModel
            };

            AddItemCommand = new RelayCommand(AddItem);
            SettingCommand = new RelayCommand(ShowSettingWindow, IsSelectedItemNotNull);
            DeleteItemCommand = new RelayCommand(ClearSelectedItem, IsSelectedItemNotNull);
        }

        /// <summary>
        /// ListViewModel を <see cref="Collection<PortViewModel>"/> で初期化
        /// </summary>
        public ListViewModel(Collection<PortViewModel> viewModels)
        {
            portViewModels = new ObservableCollection<PortViewModel>(viewModels);

            AddItemCommand = new RelayCommand(AddItem);
            SettingCommand = new RelayCommand(ShowSettingWindow, IsSelectedItemNotNull);
            DeleteItemCommand = new RelayCommand(ClearSelectedItem, IsSelectedItemNotNull);
        }

        /// <summary>
        /// コマンドの実行可否を更新
        /// </summary>
        private void UpdateCommands()
        {
            (SettingCommand as RelayCommand).RaiseCanExecuteChanged();
            (DeleteItemCommand as RelayCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 選択された内容がnullか判定
        /// </summary>
        /// <returns>選択された内容がnullか否か</returns>
        private bool IsSelectedItemNotNull() => SelectedPort != null;

        /// <summary>
        /// ポート設定を追加
        /// </summary>
        private void AddItem() => portViewModels.Add(new PortViewModel());

        /// <summary>
        /// 設定ウィンドウを表示
        /// </summary>
        private void ShowSettingWindow()
        {
            // 設定ウィンドウをモーダルで表示する
            SelectedPort.ShowSettingWindow(true);
            //SettingWindow settingWindow = new SettingWindow(viewModel);
            //settingWindow.ShowDialog();
        }

        /// <summary>
        /// ポート設定を削除
        /// </summary>
        private void ClearSelectedItem()
        {
            if (SelectedPort != null)
            {
                SelectedPort.Dispose();
                PortViewModels.Remove(SelectedPort);
                SelectedPort = null;
                RaisePropertyChanged(nameof(SelectedPort));
            }
        }

        #endregion

        #region Class

        /// <summary>
        /// 設定ボタンのコマンド
        /// </summary>
        internal class SettingButtonCommand: ICommand
        {
            #region Constructor

            public SettingButtonCommand()
            {
            }

            #endregion

            #region Event Handler

            /// <inheritdoc/>
            public event EventHandler CanExecuteChanged;

            #endregion

            #region Method

            /// <inheritdoc/>
            public bool CanExecute(object parameter)
            {
                // 常に実行可能
                return true;
            }

            /// <inheritdoc/>
            public void Execute(object parameter)
            {
                // 設定ウィンドウをモーダルで表示する
                if (parameter is PortViewModel viewModel)
                {
                    if (viewModel == null)
                    {
                        viewModel = new PortViewModel();
                    }
                    viewModel.ShowSettingWindow(true);
                    //SettingWindow settingWindow = new SettingWindow(viewModel);
                    //settingWindow.ShowDialog();
                }
            }

            #endregion
        }

        #endregion

        #region Interface Implementation

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// View に値の変更を通知
        /// </summary>
        /// <param name="propertyName">呼び出し元のプロパティ名（自動取得）</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
