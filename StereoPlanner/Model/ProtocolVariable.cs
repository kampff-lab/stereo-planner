using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoPlanner.Model
{
    public abstract class ProtocolVariable
    {
        internal ProtocolVariable()
        {
        }

        public string Name { get; set; }
    }
}
