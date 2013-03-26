using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace StereoPlanner.Model
{
    public class StereotacticPoint
    {
        double? dorsoVentralReference;
        readonly Collection<StereotacticDepth> dorsoVentralTargets = new Collection<StereotacticDepth>();

        public StereotacticPoint()
        {
            Color = Color.White;
        }

        [Category(Constants.AppearanceCategory)]
        public string Name { get; set; }

        [XmlIgnore]
        [Category(Constants.AppearanceCategory)]
        public Color Color { get; set; }

        [Browsable(false)]
        [XmlElement("Color")]
        public string ColorHtml
        {
            get { return ColorTranslator.ToHtml(Color); }
            set { Color = ColorTranslator.FromHtml(value); }
        }

        [Category(Constants.ProtocolCategory)]
        [TypeConverter(typeof(NowDateTimeConverter))]
        public DateTime Intervention { get; set; }

        [Category(Constants.CoordinatesCategory)]
        public double AnteriorPosterior { get; set; }

        [Category(Constants.CoordinatesCategory)]
        public double MedioLateral { get; set; }

        public Collection<StereotacticDepth> DorsoVentralTargets
        {
            get { return dorsoVentralTargets; }
        }

        public double DorsoVentralReference
        {
            get { return dorsoVentralReference.GetValueOrDefault(); }
            set { dorsoVentralReference = value; }
        }

        [Browsable(false)]
        public bool DorsoVentralReferenceSpecified
        {
            get { return dorsoVentralReference.HasValue; }
        }
    }
}
