﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CtrlCmdCLI;
using CtrlCmdCLI.Def;
using EpgTimer.TunerReserveViewCtrl;

namespace EpgTimer
{
    /// <summary>
    /// TunerReserveMainView.xaml の相互作用ロジック
    /// </summary>
    public partial class TunerReserveMainView : UserControl
    {
        private List<TunerNameViewItem> tunerList = new List<TunerNameViewItem>();
        private List<ReserveViewItem> reserveList = new List<ReserveViewItem>();
        private Point clickPos;
        private CtrlCmdUtil cmd = CommonManager.Instance.CtrlCmd;
        private MenuUtil mutil = CommonManager.Instance.MUtil;

        private bool updateReserveData = true;

        public TunerReserveMainView()
        {
            InitializeComponent();

            tunerReserveView.PreviewMouseWheel += new MouseWheelEventHandler(tunerReserveView_PreviewMouseWheel);
            tunerReserveView.ScrollChanged += new ScrollChangedEventHandler(tunerReserveView_ScrollChanged);
            tunerReserveView.LeftDoubleClick += new TunerReserveView.ProgramViewClickHandler(tunerReserveView_LeftDoubleClick);
            tunerReserveView.RightClick += new TunerReserveView.ProgramViewClickHandler(tunerReserveView_RightClick);

        }

        /// <summary>
        /// 保持情報のクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool ClearInfo()
        {
            tunerReserveView.ClearInfo();
            tunerReserveTimeView.ClearInfo();
            tunerReserveNameView.ClearInfo();
            tunerList.Clear();
            reserveList.Clear();

            return true;
        }

        /// <summary>
        /// 表示スクロールイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tunerReserveView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(TunerReserveView))
                {
                    //時間軸の表示もスクロール
                    tunerReserveTimeView.scrollViewer.ScrollToVerticalOffset(tunerReserveView.scrollViewer.VerticalOffset);
                    //サービス名表示もスクロール
                    tunerReserveNameView.scrollViewer.ScrollToHorizontalOffset(tunerReserveView.scrollViewer.HorizontalOffset);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// マウスホイールイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tunerReserveView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (sender.GetType() == typeof(TunerReserveView))
                {
                    TunerReserveView view = sender as TunerReserveView;
                    if (Settings.Instance.MouseScrollAuto == true)
                    {
                        view.scrollViewer.ScrollToVerticalOffset(view.scrollViewer.VerticalOffset - e.Delta);
                    }
                    else
                    {
                        if (e.Delta < 0)
                        {
                            //下方向
                            view.scrollViewer.ScrollToVerticalOffset(view.scrollViewer.VerticalOffset + Settings.Instance.ScrollSize);
                        }
                        else
                        {
                            //上方向
                            if (view.scrollViewer.VerticalOffset < Settings.Instance.ScrollSize)
                            {
                                view.scrollViewer.ScrollToVerticalOffset(0);
                            }
                            else
                            {
                                view.scrollViewer.ScrollToVerticalOffset(view.scrollViewer.VerticalOffset - Settings.Instance.ScrollSize);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// マウス位置から予約情報を取得する
        /// </summary>
        /// <param name="cursorPos">[IN]マウス位置</param>
        /// <param name="reserve">[OUT]予約情報</param>
        /// <returns>falseで存在しない</returns>
        private bool GetReserveItem(Point cursorPos, ref ReserveData reserve)
        {
            try
            {
                foreach (ReserveViewItem resInfo in reserveList)
                {
                    if (resInfo.LeftPos <= cursorPos.X && cursorPos.X < resInfo.LeftPos + resInfo.Width &&
                        resInfo.TopPos <= cursorPos.Y && cursorPos.Y < resInfo.TopPos + resInfo.Height)
                    {
                        reserve = resInfo.ReserveInfo;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// 左ボタンダブルクリックイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cursorPos"></param>
        void tunerReserveView_LeftDoubleClick(object sender, Point cursorPos)
        {
            //まず予約情報あるかチェック
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(cursorPos, ref reserve) == false) return;

            //予約変更ダイアログ表示
            ChangeReserve(reserve);
        }

        /// <summary>
        /// 右ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cursorPos"></param>
        void tunerReserveView_RightClick(object sender, Point cursorPos)
        {
            try
            {
                //右クリック表示メニューの作成
                clickPos = cursorPos;
                ReserveData reserve = new ReserveData();
                if (GetReserveItem(cursorPos, ref reserve) == false) return;
                ContextMenu menu = new ContextMenu();

                Separator separate2 = new Separator();
                MenuItem menuItemChg = new MenuItem();
                menuItemChg.Header = "変更";
                MenuItem menuItemChgDlg = new MenuItem();
                menuItemChgDlg.Header = "ダイアログ表示";
                menuItemChgDlg.Click += new RoutedEventHandler(cm_chg_Click);

                menuItemChg.Items.Add(menuItemChgDlg);
                menuItemChg.Items.Add(separate2);

                MenuItem menuItemChgRecMode0 = new MenuItem();
                menuItemChgRecMode0.Header = "全サービス (_0)";
                menuItemChgRecMode0.DataContext = (uint)0;
                menuItemChgRecMode0.Click += new RoutedEventHandler(cm_chg_recmode_Click);
                MenuItem menuItemChgRecMode1 = new MenuItem();
                menuItemChgRecMode1.Header = "指定サービス (_1)";
                menuItemChgRecMode1.DataContext = (uint)1;
                menuItemChgRecMode1.Click += new RoutedEventHandler(cm_chg_recmode_Click);
                MenuItem menuItemChgRecMode2 = new MenuItem();
                menuItemChgRecMode2.Header = "全サービス（デコード処理なし） (_2)";
                menuItemChgRecMode2.DataContext = (uint)2;
                menuItemChgRecMode2.Click += new RoutedEventHandler(cm_chg_recmode_Click);
                MenuItem menuItemChgRecMode3 = new MenuItem();
                menuItemChgRecMode3.Header = "指定サービス（デコード処理なし） (_3)";
                menuItemChgRecMode3.DataContext = (uint)3;
                menuItemChgRecMode3.Click += new RoutedEventHandler(cm_chg_recmode_Click);
                MenuItem menuItemChgRecMode4 = new MenuItem();
                menuItemChgRecMode4.Header = "視聴 (_4)";
                menuItemChgRecMode4.DataContext = (uint)4;
                menuItemChgRecMode4.Click += new RoutedEventHandler(cm_chg_recmode_Click);
                MenuItem menuItemChgRecMode5 = new MenuItem();
                menuItemChgRecMode5.Header = "無効 (_5)";
                menuItemChgRecMode5.DataContext = (uint)5;
                menuItemChgRecMode5.Click += new RoutedEventHandler(cm_chg_recmode_Click);

                menuItemChg.Items.Add(menuItemChgRecMode0);
                menuItemChg.Items.Add(menuItemChgRecMode1);
                menuItemChg.Items.Add(menuItemChgRecMode2);
                menuItemChg.Items.Add(menuItemChgRecMode3);
                menuItemChg.Items.Add(menuItemChgRecMode4);
                menuItemChg.Items.Add(menuItemChgRecMode5);

                menuItemChg.Items.Add(new Separator());

                MenuItem menuItemChgRecPri = new MenuItem();
                menuItemChgRecPri.Tag = "優先度 {0}";

                MenuItem menuItemChgRecPri1 = new MenuItem();
                menuItemChgRecPri1.Header = "1 (_1)";
                menuItemChgRecPri1.DataContext = (uint)1;
                menuItemChgRecPri1.Click += new RoutedEventHandler(cm_chg_priority_Click);
                MenuItem menuItemChgRecPri2 = new MenuItem();
                menuItemChgRecPri2.Header = "2 (_2)";
                menuItemChgRecPri2.DataContext = (uint)2;
                menuItemChgRecPri2.Click += new RoutedEventHandler(cm_chg_priority_Click);
                MenuItem menuItemChgRecPri3 = new MenuItem();
                menuItemChgRecPri3.Header = "3 (_3)";
                menuItemChgRecPri3.DataContext = (uint)3;
                menuItemChgRecPri3.Click += new RoutedEventHandler(cm_chg_priority_Click);
                MenuItem menuItemChgRecPri4 = new MenuItem();
                menuItemChgRecPri4.Header = "4 (_4)";
                menuItemChgRecPri4.DataContext = (uint)4;
                menuItemChgRecPri4.Click += new RoutedEventHandler(cm_chg_priority_Click);
                MenuItem menuItemChgRecPri5 = new MenuItem();
                menuItemChgRecPri5.Header = "5 (_5)";
                menuItemChgRecPri5.DataContext = (uint)5;
                menuItemChgRecPri5.Click += new RoutedEventHandler(cm_chg_priority_Click);

                menuItemChgRecPri.Items.Add(menuItemChgRecPri1);
                menuItemChgRecPri.Items.Add(menuItemChgRecPri2);
                menuItemChgRecPri.Items.Add(menuItemChgRecPri3);
                menuItemChgRecPri.Items.Add(menuItemChgRecPri4);
                menuItemChgRecPri.Items.Add(menuItemChgRecPri5);

                menuItemChg.Items.Add(menuItemChgRecPri);

                MenuItem menuItemDel = new MenuItem();
                menuItemDel.Header = "削除";
                menuItemDel.Click += new RoutedEventHandler(cm_del_Click);
                MenuItem menuItemPTable = new MenuItem();
                menuItemPTable.Header = "番組表へジャンプ";
                menuItemPTable.Click += new RoutedEventHandler(cm_programtable_Click);
                MenuItem menuItemAutoAdd = new MenuItem();
                menuItemAutoAdd.Header = "自動予約登録";
                menuItemAutoAdd.ToolTip = mutil.EpgKeyword_TrimMode();
                menuItemAutoAdd.Click += new RoutedEventHandler(cm_autoadd_Click);
                MenuItem menuItemTimeshift = new MenuItem();
                menuItemTimeshift.Header = "追っかけ再生";
                menuItemTimeshift.Click += new RoutedEventHandler(cm_timeShiftPlay_Click);

                MenuItem menuItemCopy = new MenuItem();
                menuItemCopy.Header = "番組名をコピー";
                menuItemCopy.ToolTip = mutil.CopyTitle_TrimMode();
                menuItemCopy.Click += new RoutedEventHandler(cm_CopyTitle_Click);
                MenuItem menuItemContent = new MenuItem();
                menuItemContent.Header = "番組情報をコピー";
                menuItemContent.ToolTip = mutil.CopyContent_Mode();
                menuItemContent.Click += new RoutedEventHandler(cm_CopyContent_Click);
                MenuItem menuItemSearch = new MenuItem();
                menuItemSearch.Header = "番組名をネットで検索";
                menuItemSearch.ToolTip = mutil.SearchText_TrimMode();
                menuItemSearch.Click += new RoutedEventHandler(cm_SearchTitle_Click);

                menuItemChg.IsEnabled = true;
                ((MenuItem)menuItemChg.Items[menuItemChg.Items.IndexOf(menuItemChgRecMode0) + Math.Min((int)reserve.RecSetting.RecMode, 5)]).IsChecked = true;
                ((MenuItem)menuItemChgRecPri.Items[Math.Min((int)(reserve.RecSetting.Priority - 1), 4)]).IsChecked = true;
                menuItemChgRecPri.Header = string.Format((string)menuItemChgRecPri.Tag, reserve.RecSetting.Priority);
                
                //フォーカスに問題があり、一度クリックしないと正しく動かない‥。
                //仮対策コード
                menuItemPTable.IsEnabled = IsViewOnceClicked;
                if (menuItemPTable.IsEnabled == false)
                {
                    menuItemPTable.ToolTip = "一度画面を左クリックすると使用可能";
                    ToolTipService.SetShowOnDisabled(menuItemPTable, true);
                }

                menuItemDel.IsEnabled = true;
                menuItemAutoAdd.IsEnabled = true;
                menuItemTimeshift.IsEnabled = true;

                menu.Items.Add(menuItemChg);
                menu.Items.Add(menuItemDel);
                menu.Items.Add(menuItemPTable);
                menu.Items.Add(menuItemAutoAdd);
                menu.Items.Add(menuItemTimeshift);

                if (Settings.Instance.CmAppendMenu == true)
                {
                    menu.Items.Add(new Separator());
                    if (Settings.Instance.CmCopyTitle == true)
                    {
                        menu.Items.Add(menuItemCopy);
                    }
                    if (Settings.Instance.CmCopyContent == true)
                    {
                        menu.Items.Add(menuItemContent);
                    }
                    if (Settings.Instance.CmSearchTitle == true)
                    {
                        menu.Items.Add(menuItemSearch);
                    }
                }

                menu.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 右クリックメニュー 予約変更クリックイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_chg_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            ChangeReserve(reserve);
        }

        /// <summary>
        /// 右クリックメニュー 予約削除クリックイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_del_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.ReserveDelete(reserve);
        }

        /// <summary>
        /// 右クリックメニュー 予約モード変更イベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_chg_recmode_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.ReserveChangeRecmode(reserve, sender);
        }

        /// <summary>
        /// 右クリックメニュー 優先度変更イベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_chg_priority_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.ReserveChangePriority(reserve, sender);
        }

        /// <summary>
        /// 右クリックメニュー 番組表へジャンプイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_programtable_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;

            BlackoutWindow.selectedReserveItem = new ReserveItem(reserve);
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.moveTo_tabItem_epg();
        }

        //フォーカスがうまくいかない仮対策
        private bool IsViewOnceClicked = false;

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.IsViewOnceClicked = false;
        }

        private void tunerReserveView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsViewOnceClicked = true;
        }

        /// <summary>
        /// 右クリックメニュー 自動予約登録イベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_autoadd_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.SendAutoAdd(reserve, this);
        }

        /// <summary>
        /// 右クリックメニュー 追っかけ再生イベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_timeShiftPlay_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            CommonManager.Instance.TVTestCtrl.StartTimeShift(reserve.ReserveID);
        }

        /// <summary>
        /// 右クリックメニュー 番組名をコピーイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_CopyTitle_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.CopyTitle2Clipboard(reserve.Title);
        }

        /// <summary>
        /// 右クリックメニュー 番組情報をコピーイベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_CopyContent_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.CopyContent2Clipboard(reserve);
        }

        /// <summary>
        /// 右クリックメニュー 番組名で検索イベント呼び出し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cm_SearchTitle_Click(object sender, RoutedEventArgs e)
        {
            ReserveData reserve = new ReserveData();
            if (GetReserveItem(clickPos, ref reserve) == false) return;
            mutil.SearchText(reserve.Title);
        }

        /// <summary>
        /// 予約変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeReserve(ReserveData reserveInfo)
        {
            mutil.OpenChangeReserveWindow(reserveInfo, this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVisible == true)
            {
                if (updateReserveData == true)
                {
                    if (ReloadReserveData() == true)
                    {
                        updateReserveData = false;
                    }
                }
            }
        }

        private bool ReloadReserveData()
        {
            try
            {
                if (CommonManager.Instance.NWMode == true)
                {
                    if (CommonManager.Instance.NW.IsConnected == false)
                    {
                        return false;
                    }
                } 
                ErrCode err = CommonManager.Instance.DB.ReloadReserveInfo();
                if (CommonManager.CmdErrMsgTypical(err, "予約情報の取得", this) == false)
                {
                    return false;
                }

                ReloadReserveViewItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            return true;
        }

        /// <summary>
        /// 予約情報更新通知
        /// </summary>
        public void UpdateReserveData()
        {
            updateReserveData = true;
            if (this.IsVisible == true)
            {
                if (ReloadReserveData() == true)
                {
                    updateReserveData = false;
                }
            }
        }

        /// <summary>
        /// 予約情報の再描画
        /// </summary>
        private void ReloadReserveViewItem()
        {
            tunerReserveView.ClearInfo();
            tunerReserveTimeView.ClearInfo();
            tunerReserveNameView.ClearInfo();
            List<DateTime> timeList = new List<DateTime>();
            tunerList.Clear();
            reserveList.Clear();
            try
            {
                double leftPos = 0;
                for (int i = 0; i < CommonManager.Instance.DB.TunerReserveList.Count; i++)
                {
                    double width = 150;
                    TunerReserveInfo info = CommonManager.Instance.DB.TunerReserveList.Values.ElementAt(i);
                    TunerNameViewItem tunerInfo = new TunerNameViewItem(info, width);
                    tunerList.Add(tunerInfo);

                    List<ReserveViewItem> tunerAddList = new List<ReserveViewItem>();
                    for (int j = 0; j < info.reserveList.Count; j++ )
                    {
                        UInt32 reserveID = (UInt32)info.reserveList[j];
                        if (CommonManager.Instance.DB.ReserveList.ContainsKey(reserveID) == false)
                        {
                            continue;
                        }
                        ReserveData reserveInfo = CommonManager.Instance.DB.ReserveList[reserveID];
                        ReserveViewItem viewItem = new ReserveViewItem(CommonManager.Instance.DB.ReserveList[reserveID]);

                        Int32 duration = (Int32)reserveInfo.DurationSecond;
                        DateTime startTime = reserveInfo.StartTime;
                        if (reserveInfo.RecSetting.UseMargineFlag == 1)
                        {
                            if (reserveInfo.RecSetting.StartMargine < 0)
                            {
                                startTime = reserveInfo.StartTime.AddSeconds(reserveInfo.RecSetting.StartMargine*-1);
                                duration += reserveInfo.RecSetting.StartMargine;
                            }
                            if (reserveInfo.RecSetting.EndMargine < 0)
                            {
                                duration += reserveInfo.RecSetting.EndMargine;
                            }
                        }
                        DateTime EndTime;
                        EndTime = startTime.AddSeconds(duration);
                        //if ((duration / 60) < Settings.Instance.MinHeight)
                        //{
                        //    duration = (int)Settings.Instance.MinHeight;
                        //}

                        viewItem.Height = Math.Floor((duration / 60) * Settings.Instance.MinHeight);
                        if (viewItem.Height == 0)
                        {
                            viewItem.Height = Settings.Instance.MinHeight;
                        }
                        viewItem.Width = 150;
                        viewItem.LeftPos = leftPos;

                        foreach (ReserveViewItem addItem in tunerAddList)
                        {
                            ReserveData addInfo = addItem.ReserveInfo;
                            Int32 durationAdd = (Int32)addInfo.DurationSecond;
                            DateTime startTimeAdd = addInfo.StartTime;
                            if (addInfo.RecSetting.UseMargineFlag == 1)
                            {
                                if (addInfo.RecSetting.StartMargine < 0)
                                {
                                    startTimeAdd = addInfo.StartTime.AddSeconds(addInfo.RecSetting.StartMargine*-1);
                                    durationAdd += addInfo.RecSetting.StartMargine;
                                }
                                if (addInfo.RecSetting.EndMargine < 0)
                                {
                                    durationAdd += addInfo.RecSetting.EndMargine;
                                }
                            }
                            DateTime endTimeAdd;
                            endTimeAdd = startTimeAdd.AddSeconds(durationAdd);

                            if ((startTimeAdd <= startTime && startTime < endTimeAdd) ||
                                (startTimeAdd < EndTime && EndTime <= endTimeAdd) || 
                                (startTime <= startTimeAdd && startTimeAdd < EndTime) ||
                                (startTime < endTimeAdd && endTimeAdd <= EndTime)
                                )
                            {
                                if (addItem.LeftPos >= viewItem.LeftPos)
                                {
                                    viewItem.LeftPos += 150;
                                    if (viewItem.LeftPos - leftPos >= width)
                                    {
                                        width += 150;
                                    }
                                }
                            }
                        }

                        reserveList.Add(viewItem);
                        tunerAddList.Add(viewItem);

                        //必要時間リストの構築

                        DateTime chkStartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0);
                        while (chkStartTime <= EndTime)
                        {
                            int index = timeList.BinarySearch(chkStartTime);
                            if (index < 0)
                            {
                                timeList.Insert(~index, chkStartTime);
                            }
                            chkStartTime = chkStartTime.AddHours(1);
                        }

                    }
                    tunerInfo.Width = width;
                    leftPos += width;
                }

                //表示位置設定
                foreach (ReserveViewItem item in reserveList)
                {
                    DateTime startTime = item.ReserveInfo.StartTime;
                    if (item.ReserveInfo.RecSetting.UseMargineFlag == 1)
                    {
                        if (item.ReserveInfo.RecSetting.StartMargine < 0)
                        {
                            startTime = item.ReserveInfo.StartTime.AddSeconds(item.ReserveInfo.RecSetting.StartMargine*-1);
                        }
                    }

                    DateTime chkStartTime = new DateTime(startTime.Year,
                        startTime.Month,
                        startTime.Day,
                        startTime.Hour,
                        0,
                        0);
                    int index = timeList.BinarySearch(chkStartTime);
                    if (index >= 0)
                    {
                        item.TopPos = (index * 60 + (startTime - chkStartTime).TotalMinutes) * Settings.Instance.MinHeight;
                    }
                }

                tunerReserveTimeView.SetTime(timeList, true);
                tunerReserveNameView.SetTunerInfo(tunerList);
                tunerReserveView.SetReserveList(reserveList,
                    leftPos,
                    timeList.Count * 60 * Settings.Instance.MinHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }
}
