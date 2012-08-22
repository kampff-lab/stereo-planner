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
    [XmlInclude(typeof(StringVariable))]
    [XmlInclude(typeof(DoubleVariable))]
    [XmlInclude(typeof(DateTimeVariable))]
    [Editor(typeof(ProtocolVariableCollectionEditor), typeof(UITypeEditor))]
    public class ProtocolVariableCollection : KeyedCollection<string, ProtocolVariable>
    {
        protected override string GetKeyForItem(ProtocolVariable item)
        {
            return item.Name;
        }

        class ProtocolVariableCollectionEditor : CollectionEditor
        {
            public ProtocolVariableCollectionEditor(Type type)
                : base(type)
            {
            }

            protected override Type[] CreateNewItemTypes()
            {
                return new[]
                {
                    typeof(StringVariable),
                    typeof(DoubleVariable),
                    typeof(DateTimeVariable)
                };
            }
        }
    }
}
