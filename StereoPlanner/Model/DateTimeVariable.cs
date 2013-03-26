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
    }
}
