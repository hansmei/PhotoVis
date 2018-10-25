using System;
using Microsoft.Maps.MapControl.WPF;

namespace PhotoVis.Interfaces
{
    interface IImageAtLocation
    {
        int ID { get; }

        int ProjectId { get; set; }
        string ImagePath { get; set; }

        Location Location { get; set; }
        int Heading { get; set; }
        int Rotation { get; set; }

        DateTime TimeImageTaken { get; set; }
        DateTime TimeIndexed { get; }

    }
}
