﻿#pragma checksum "..\..\..\..\EpgView\EpgListMainView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C9D70E62334AED6E2DD8EDB1D0D41846EE6E7EE1"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using EpgTimer.EpgView;
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
    /// EpgListMainView
    /// </summary>
    public partial class EpgListMainView : EpgTimer.EpgView.EpgViewBase, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal EpgTimer.EpgView.TimeJumpView timeJumpView;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal EpgTimer.EpgView.TimeMoveView timeMoveView;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_now;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_chkAll;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_clearAll;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listBox_service;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RichTextBox richTextBox_eventInfo;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listView_event;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\..\EpgView\EpgListMainView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView gridView_event;
        
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
            System.Uri resourceLocater = new System.Uri("/EpgTimer;component/epgview/epglistmainview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\EpgView\EpgListMainView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.timeJumpView = ((EpgTimer.EpgView.TimeJumpView)(target));
            return;
            case 2:
            this.timeMoveView = ((EpgTimer.EpgView.TimeMoveView)(target));
            return;
            case 3:
            this.button_now = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.button_chkAll = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\..\EpgView\EpgListMainView.xaml"
            this.button_chkAll.Click += new System.Windows.RoutedEventHandler(this.button_chkAll_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.button_clearAll = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\EpgView\EpgListMainView.xaml"
            this.button_clearAll.Click += new System.Windows.RoutedEventHandler(this.button_clearAll_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.listBox_service = ((System.Windows.Controls.ListView)(target));
            return;
            case 7:
            this.richTextBox_eventInfo = ((System.Windows.Controls.RichTextBox)(target));
            return;
            case 8:
            this.listView_event = ((System.Windows.Controls.ListView)(target));
            
            #line 61 "..\..\..\..\EpgView\EpgListMainView.xaml"
            this.listView_event.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listView_event_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.gridView_event = ((System.Windows.Controls.GridView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

