using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raid.Service
{
    public static class ModelConverters
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num) => (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
    }
}