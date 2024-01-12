using Terraria;
using Terraria.ModLoader;

namespace MOTLMod
{
	public class MOTLMod : Mod
	{
        //click the lil thing on the left side to expand and collapse the region :heart:
        #region Tile Merge Stuff - Zero
        //Tile Merge Stuff
        // - Zero
        private static TileTest v = new();
        public static TileTest tileMerge => v;

        public class TileTest
        {
            public bool this[int tile1, int tile2]
            {
                get => Main.tileMerge[tile1][tile2] || Main.tileMerge[tile2][tile1];
                set => MOTLUtils.Merge(tile1, tile2);
            }
        }
        #endregion
    }
}