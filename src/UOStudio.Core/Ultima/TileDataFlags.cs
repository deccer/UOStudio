using System;

namespace UOStudio.Core.Ultima
{
    [Flags]
    public enum TileDataFlags : uint
    {
        Background,
        Weapon,
        Transparent,
        Translucent,
        Wall,
        Damaging,
        Impassable,
        Wet,
        Unknown1,
        Surface,
        Bridge,
        Generic,
        Window,
        NoShoot,
        ArticleA,
        ArticleAn,
        Internal,
        Foliage,
        ParticleHue,
        Unknown2,
        Map,
        Container,
        Wearable,
        LightSource,
        Animation,
        NoDiagonal,
        ArtUsed,
        Armor,
        Roof,
        Door,
        StairBack,
        StairRight
    }
}
