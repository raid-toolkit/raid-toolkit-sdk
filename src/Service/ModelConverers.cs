using System;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public static class ModelConverters
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num) => (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);

        public static ArtifactStatBonus ToDataModel(this SharedModel.Meta.Artifacts.Bonuses.ArtifactBonus bonus) => new ArtifactStatBonus
        {
            KindId = bonus._kindId,
            Absolute = bonus._value._isAbsolute,
            Value = bonus._value._value.AsFloat(),
            GlyphPower = bonus._powerUpValue.AsFloat(),
            Level = bonus._level
        };

    }
}