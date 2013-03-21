using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace StereoPlanner.Model
{
    public class DateTimeVariable : ProtocolVariable
    {
        [TypeConverter(typeof(NowDateTimeConverter))]
        public DateTime Value { get; set; }

        class NowDateTimeConverter : DateTimeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new[] { DateTime.Now });
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                var text = value as string;
                if (string.Compare(text, "now", true) == 0)
                {
                    return DateTime.Now;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
