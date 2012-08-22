using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using StereoPlanner.Graphics;

namespace StereoPlanner.Model
{
    public class EllipseShape : ProtocolShape
    {
        public string Center { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public Color Color { get; set; }

        public override void Draw(SpriteBatch spriteBatch, StereotacticPointCollection points)
        {
        }
    }
}
