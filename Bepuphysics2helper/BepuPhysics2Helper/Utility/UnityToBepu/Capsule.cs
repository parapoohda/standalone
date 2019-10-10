using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Utility.UnityToBepu
{
    public static class Capsule
    {
        public static float ChangeSizeToBepu(Vector2 size) {
            return size.Y - 2 * size.X;
        }
    }
}
