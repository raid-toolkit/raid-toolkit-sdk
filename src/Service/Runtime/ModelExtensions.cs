using System;

namespace Raid.Service
{
    public static partial class ModelExtensions
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num) => (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
    }
}