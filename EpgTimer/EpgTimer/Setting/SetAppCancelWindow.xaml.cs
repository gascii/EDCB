﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EpgTimer
{
    /// <summary>
    /// SetAppCancelWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SetAppCancelWindow : Window
    {
        public List<String> processList = new List<string>();
        public String ngMin = "30";
        public bool ngUsePC = false;
        public String ngUsePCMin = "3";
        public bool ngFileStreaming = false;
        public bool ngShareFile = false;

        public SetAppCancelWindow()
        {
            InitializeComponent();

            if (CommonManager.Instance.NWMode == true)
            {
                button_process_del.IsEnabled = false;
                label2.IsEnabled = false;
                textBox_process.IsEnabled = false;
                button_process_add.IsEnabled = false;
                button_process_open.IsEnabled = false;
                label3.IsEnabled = false;
                textBox_ng_min.IsEnabled = false;
                label4.IsEnabled = false;
                checkBox_ng_fileStreaming.IsEnabled = false;
                checkBox_ng_shareFile.IsEnabled = false;
                textBox_ng_usePC_min.IsEnabled = false;
                label7.IsEnabled = false;
                checkBox_ng_usePC.IsEnabled = false;
                button_OK.IsEnabled = false;
            }
        }

        private void button_process_del_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_process.SelectedItem != null)
            {
                listBox_process.Items.RemoveAt(listBox_process.SelectedIndex);
            }
        }

        private void button_process_open_Click(object sender, RoutedEventArgs e)
        {
            CommonManager.GetFileNameByDialog(textBox_process, true, "", ".exe");
        }

        private void button_process_add_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_process.Text) == false)
            {
                foreach (String info in listBox_process.Items)
                {
                    if (String.Compare(textBox_process.Text, info, true) == 0)
                    {
                        MessageBox.Show("すでに追加されています");
                        return;
                    }
                }
                listBox_process.Items.Add(textBox_process.Text);
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            processList.Clear();
            foreach (String info in listBox_process.Items)
            {
                processList.Add(info);
            }
            ngMin = textBox_ng_min.Text;
            if (checkBox_ng_usePC.IsChecked == true)
            {
                ngUsePC = true;
            }
            else
            {
                ngUsePC = false;
            }
            ngUsePCMin = textBox_ng_usePC_min.Text;
            if (checkBox_ng_fileStreaming.IsChecked == true)
            {
                ngFileStreaming = true;
            }
            else
            {
                ngFileStreaming = false;
            }
            if (checkBox_ng_shareFile.IsChecked == true)
            {
                ngShareFile = true;
            }
            else
            {
                ngShareFile = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string info in processList)
            {
                listBox_process.Items.Add(info);
            }

            textBox_ng_min.Text = ngMin;
            checkBox_ng_usePC.IsChecked = ngUsePC;
            textBox_ng_usePC_min.Text = ngUsePCMin;
            checkBox_ng_fileStreaming.IsChecked = ngFileStreaming;
            checkBox_ng_shareFile.IsChecked = ngShareFile;

        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.button_cancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        break;
                }
            }
            base.OnKeyDown(e);
        }

    }
}
