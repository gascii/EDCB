﻿#pragma checksum "..\..\..\ConnectWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DCE2EC4A1BD5F09A41F58773AE9BAA3CFAA0126D"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EpgTimer {
    
    
    /// <summary>
    /// ConnectWindow
    /// </summary>
    public partial class ConnectWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmb_preset;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_reload;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_add;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_delete;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_srvIP;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_srvPort;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_clientPort;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_clientPort;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_connect;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_mac;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_wake;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\ConnectWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock label_wakeResult;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EpgTimer;component/connectwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ConnectWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\..\ConnectWindow.xaml"
            ((EpgTimer.ConnectWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 4 "..\..\..\ConnectWindow.xaml"
            ((EpgTimer.ConnectWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cmb_preset = ((System.Windows.Controls.ComboBox)(target));
            
            #line 8 "..\..\..\ConnectWindow.xaml"
            this.cmb_preset.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.btn_reload_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btn_reload = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\..\ConnectWindow.xaml"
            this.btn_reload.Click += new System.Windows.RoutedEventHandler(this.btn_reload_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btn_add = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\ConnectWindow.xaml"
            this.btn_add.Click += new System.Windows.RoutedEventHandler(this.btn_add_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btn_delete = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\..\ConnectWindow.xaml"
            this.btn_delete.Click += new System.Windows.RoutedEventHandler(this.btn_delete_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.textBox_srvIP = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.textBox_srvPort = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.checkBox_clientPort = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this.textBox_clientPort = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.button_connect = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\ConnectWindow.xaml"
            this.button_connect.Click += new System.Windows.RoutedEventHandler(this.button_connect_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.textBox_mac = ((System.Windows.Controls.TextBox)(target));
            return;
            case 12:
            this.button_wake = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\ConnectWindow.xaml"
            this.button_wake.Click += new System.Windows.RoutedEventHandler(this.button_wake_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.label_wakeResult = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

