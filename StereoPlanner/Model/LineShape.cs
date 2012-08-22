using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StereoPlanner.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace StereoPlanner.Model
{
    public class LineShape : ProtocolShape
    {
        public string Line0 { get; set; }

        public string Line1 { get; set; }

        public Color Color { get; set; }

        public override void Draw(SpriteBatch spriteBatch, StereotacticPointCollection points)
        {
            if (!string.IsNullOrEmpty(Line0) && !string.IsNullOrEmpty(Line1) &&
                points.Contains(Line0) && points.Contains(Line1))
            {
                var line0 = Utilities.GetPointPosition(points[Line0]);
                var line1 = Utilities.GetPointPosition(points[Line1]);
                spriteBatch.DrawVertices(new[] { line0, line1 }, BeginMode.Lines, Color);
            }
        }
    }
}
