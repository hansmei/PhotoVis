using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;
using System.Windows;


namespace ClusterEngine
{
    public class ColoredPushpin : Pushpin
    {
        //private string _strokeColor = "#c222";
        //public string _fillColor = "#E40E9700";


        public string FillColor
        {
            get { return (string)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register("FillColor", typeof(string), typeof(ColoredPushpin), new UIPropertyMetadata("#E40E9700"));

        public string StrokeColor
        {
            get { return (string)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(string), typeof(ColoredPushpin), new UIPropertyMetadata("#c222"));

        
        public ColoredPushpin() : base()
        {
        }

    }
}
