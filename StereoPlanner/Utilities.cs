using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using StereoPlanner.Model;

namespace StereoPlanner
{
    static class Utilities
    {
        public static Vector2 GetPointPosition(StereotacticPoint point)
        {
            return new Vector2((float)point.MedioLateral, (float)point.AnteriorPosterior);
        }

        public static Vector2 GetPointPosition(StereotacticPoint point, StereotacticPoint reference)
        {
            if (reference == null || reference == point) return GetPointPosition(point);
            else return GetPointPosition(point) + GetPointPosition(reference);
        }

        public static Vector2 GetRenderPosition(StereotacticPoint point, StereotacticPoint reference)
        {
            if (reference == point) return Vector2.Zero;
            else return GetPointPosition(point);
        }
    }
}
