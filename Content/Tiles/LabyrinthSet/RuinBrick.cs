using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MOTLMod.Content.Tiles.LabyrinthSet
{
    public class RuinBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            AddMapEntry(new Color(126, 85, 88));
        }
    }
}
