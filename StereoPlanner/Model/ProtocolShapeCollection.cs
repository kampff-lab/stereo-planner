using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace StereoPlanner.Model
{
    [XmlInclude(typeof(LineShape))]
    [Editor(typeof(ProtocolShapeCollectionEditor), typeof(UITypeEditor))]
    public class ProtocolShapeCollection : Collection<ProtocolShape>
    {
        class ProtocolShapeCollectionEditor : CollectionEditor
        {
            public ProtocolShapeCollectionEditor(Type type)
                : base(type)
            {
            }

            protected override Type[] CreateNewItemTypes()
            {
                return new[]
                {
                    typeof(LineShape),
                };
            }
        }
    }
}
