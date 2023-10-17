using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils
{
    public static class Utils
    {
        public static float GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            return angle;
        }
    }
}
