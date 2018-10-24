﻿#pragma checksum "..\..\..\Views\MapView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "00927E173AF307581BBBA67F6B242838E7396293"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Maps.MapControl.WPF;
using PhotoVis.Controls;
using PhotoVis.Views;
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


namespace PhotoVis.Views {
    
    
    /// <summary>
    /// MapView
    /// </summary>
    public partial class MapView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Overlay;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image currentImage;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock imageCounter;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.Map InnerMap;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FileName;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImageSaveDate;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImageTaken;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.Map MyMap;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border debugContainer;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel eventsPanel;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.MapLayer NewPolygonLayer;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.MapLayer PushpinLayer;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PhotoVis.Controls.ScrollingTextBox StatusTbx;
        
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
            System.Uri resourceLocater = new System.Uri("/PhotoVis;component/views/mapview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MapView.xaml"
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
            this.Overlay = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.currentImage = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.imageCounter = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.InnerMap = ((Microsoft.Maps.MapControl.WPF.Map)(target));
            return;
            case 5:
            this.FileName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.ImageSaveDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.ImageTaken = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            
            #line 62 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.MyMap = ((Microsoft.Maps.MapControl.WPF.Map)(target));
            return;
            case 10:
            this.debugContainer = ((System.Windows.Controls.Border)(target));
            return;
            case 11:
            this.eventsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 12:
            this.NewPolygonLayer = ((Microsoft.Maps.MapControl.WPF.MapLayer)(target));
            return;
            case 13:
            this.PushpinLayer = ((Microsoft.Maps.MapControl.WPF.MapLayer)(target));
            return;
            case 14:
            
            #line 110 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AerialMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 111 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RoadMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 112 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.HybridMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 114 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PointClusterData_Clicked);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 115 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GridClusterData_Clicked);
            
            #line default
            #line hidden
            return;
            case 19:
            this.StatusTbx = ((PhotoVis.Controls.ScrollingTextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

