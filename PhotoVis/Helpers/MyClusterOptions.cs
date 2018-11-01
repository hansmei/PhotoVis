using ClusterEngine;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows;

using PhotoVis.Data;

namespace PhotoVis
{
    /// <summary>
    /// A custom class that defines the cluster options for this 
    /// </summary>
    public class MyClusterOptions: ClusterOptions
    {
        public MyClusterOptions(int radius):
            base()
        {
            this.ClusterRadius = radius;
        }

        public override ColoredPushpin RenderEntity(Entity entity)
        {
            var p = new ColoredPushpin();
            MapLayer.SetPosition(p, entity.Location);
            p.Tag = entity;
            p.ToolTip = new ToolTip()
            {
                Content = (entity as ImageAtLocation).Title
            };
            
            ControlTemplate myTemplate = (ControlTemplate)Application.Current.FindResource("PushpinColorTemplate");
            p.Template = myTemplate;
            p.ApplyTemplate();

            return p;
        }
        
        public override ColoredPushpin RenderCluster(ClusteredPoint cluster)
        {
            var p = new ColoredPushpin();
            MapLayer.SetPosition(p, cluster.Location);
            p.Content = "+";
            p.Tag = cluster;
            p.ToolTip = new ToolTip()
            {
                Content = string.Format("{0} Clustered Entities", cluster.EntityIds.Count)
            };

            ControlTemplate myTemplate = (ControlTemplate)Application.Current.FindResource("PushpinColorTemplate");
            p.Template = myTemplate;
            p.ApplyTemplate();

            return p;
        }
    }
}
