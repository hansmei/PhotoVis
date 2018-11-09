﻿#pragma checksum "..\..\..\Views\MapView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D1ABE3CF69F63E5A7F1472DDD50747158359A44A"
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
using PhotoVis.Helpers;
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
    public partial class MapView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 58 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Overlay;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image currentImage;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock imageCounter;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.Map InnerMap;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PhotoVis.Controls.BrowserControl MyBrowserControl;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImageFileName;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImageSaveDate;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImageTaken;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Creator;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox UnassignedImagesControl;
        
        #line default
        #line hidden
        
        
        #line 203 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.Map MyMap;
        
        #line default
        #line hidden
        
        
        #line 207 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.MapLayer NewPolygonLayer;
        
        #line default
        #line hidden
        
        
        #line 208 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Maps.MapControl.WPF.MapLayer PushpinLayer;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border debugContainer;
        
        #line default
        #line hidden
        
        
        #line 215 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel eventsPanel;
        
        #line default
        #line hidden
        
        
        #line 222 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AgeLowerValue;
        
        #line default
        #line hidden
        
        
        #line 224 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AgeUpperValue;
        
        #line default
        #line hidden
        
        
        #line 227 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PhotoVis.Controls.RangeSlider AgeSlider;
        
        #line default
        #line hidden
        
        
        #line 241 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image GoogleStreetViewMarker;
        
        #line default
        #line hidden
        
        
        #line 248 "..\..\..\Views\MapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.TranslateTransform GoogleStreetViewTranslate;
        
        #line default
        #line hidden
        
        
        #line 266 "..\..\..\Views\MapView.xaml"
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
            this.MyBrowserControl = ((PhotoVis.Controls.BrowserControl)(target));
            return;
            case 6:
            this.ImageFileName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.ImageSaveDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.ImageTaken = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.Creator = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            
            #line 113 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 119 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenStreetViewNewWindow_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.UnassignedImagesControl = ((System.Windows.Controls.ListBox)(target));
            
            #line 144 "..\..\..\Views\MapView.xaml"
            this.UnassignedImagesControl.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Images_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 145 "..\..\..\Views\MapView.xaml"
            this.UnassignedImagesControl.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.Images_MouseMove);
            
            #line default
            #line hidden
            return;
            case 14:
            this.MyMap = ((Microsoft.Maps.MapControl.WPF.Map)(target));
            
            #line 204 "..\..\..\Views\MapView.xaml"
            this.MyMap.Drop += new System.Windows.DragEventHandler(this.MyMap_Drop);
            
            #line default
            #line hidden
            
            #line 205 "..\..\..\Views\MapView.xaml"
            this.MyMap.DragEnter += new System.Windows.DragEventHandler(this.MyMap_DragEnter);
            
            #line default
            #line hidden
            return;
            case 15:
            this.NewPolygonLayer = ((Microsoft.Maps.MapControl.WPF.MapLayer)(target));
            return;
            case 16:
            this.PushpinLayer = ((Microsoft.Maps.MapControl.WPF.MapLayer)(target));
            return;
            case 17:
            this.debugContainer = ((System.Windows.Controls.Border)(target));
            return;
            case 18:
            this.eventsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 19:
            this.AgeLowerValue = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 20:
            this.AgeUpperValue = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 21:
            this.AgeSlider = ((PhotoVis.Controls.RangeSlider)(target));
            
            #line 227 "..\..\..\Views\MapView.xaml"
            this.AgeSlider.AddHandler(System.Windows.Controls.Primitives.Thumb.DragCompletedEvent, new System.Windows.Controls.Primitives.DragCompletedEventHandler(this.AgeSlider_DragCompleted));
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 238 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AerialMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 239 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RoadMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 240 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.HybridMode_Clicked);
            
            #line default
            #line hidden
            return;
            case 25:
            this.GoogleStreetViewMarker = ((System.Windows.Controls.Image)(target));
            
            #line 242 "..\..\..\Views\MapView.xaml"
            this.GoogleStreetViewMarker.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.GoogleStreetView_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 243 "..\..\..\Views\MapView.xaml"
            this.GoogleStreetViewMarker.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.GoogleStreetView_MouseMove);
            
            #line default
            #line hidden
            
            #line 244 "..\..\..\Views\MapView.xaml"
            this.GoogleStreetViewMarker.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.GoogleStreetView_MouseDown);
            
            #line default
            #line hidden
            return;
            case 26:
            this.GoogleStreetViewTranslate = ((System.Windows.Media.TranslateTransform)(target));
            return;
            case 27:
            this.StatusTbx = ((PhotoVis.Controls.ScrollingTextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 13:
            
            #line 168 "..\..\..\Views\MapView.xaml"
            ((System.Windows.Controls.Image)(target)).MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Image_MouseUp);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

