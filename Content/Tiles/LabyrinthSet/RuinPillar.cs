using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MOTLMod.Content.Tiles.LabyrinthSet
{
    public class RuinPillar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            MOTLMod.tileMerge[Type, TileID.Stone] = true;
            MOTLMod.tileMerge[Type, TileID.ClayBlock] = true;
            MOTLMod.tileMerge[Type, Mod.Find<ModTile>("RuinBrick").Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            AddMapEntry(new Color(78, 45, 56));
        }
    }
}
