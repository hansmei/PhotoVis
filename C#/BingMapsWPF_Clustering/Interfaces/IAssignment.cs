using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVis.Interfaces
{
    interface IAssignment
    {
        int AssignmentNumber { get; set; }
        string AssignmentName { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        DateTime TimeIndexed { get; }
    }
}
