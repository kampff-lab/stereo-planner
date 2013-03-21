using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StereoPlanner.Model
{
    public class StereotacticProtocol
    {
        readonly StereotacticPointCollection points = new StereotacticPointCollection();
        readonly ProtocolVariableCollection variables = new ProtocolVariableCollection();
        readonly ProtocolShapeCollection shapes = new ProtocolShapeCollection();

        [DisplayName("Reference")]
        [TypeConverter(typeof(ReferencePointTypeConverter))]
        public string ReferencePoint { get; set; }

        public StereotacticPointCollection Points
        {
            get { return points; }
        }

        public ProtocolVariableCollection Variables
        {
            get { return variables; }
        }

        public ProtocolShapeCollection Shapes
        {
            get { return shapes; }
        }
    }
}
