using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace StereoPlanner.Model
{
    public class StereotacticPointCollection : KeyedCollection<string, StereotacticPoint>
    {
        protected override string GetKeyForItem(StereotacticPoint item)
        {
            return item.Name;
        }
    }
}
