using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StereoPlanner.Graphics;
using System.ComponentModel;

namespace StereoPlanner.Model
{
    public abstract class ProtocolShape
    {
        public string Name { get; set; }

        public abstract void Draw(SpriteBatch spriteBatch, StereotacticPointCollection points);
    }
}
