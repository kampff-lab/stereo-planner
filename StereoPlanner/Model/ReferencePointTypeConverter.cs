using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace StereoPlanner.Model
{
    class ReferencePointTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var protocol = context.Instance as StereotacticProtocol;
            if (protocol != null)
            {
                return new StandardValuesCollection(protocol.Points.Select(point => point.Name).ToArray());
            }

            return base.GetStandardValues(context);
        }
    }
}
