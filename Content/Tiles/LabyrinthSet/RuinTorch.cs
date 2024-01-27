using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using MOTLMod.Content.Dusts.SpecificDusts;

namespace MOTLMod.Content.Tiles.LabyrinthSet
{
    public class RuinTorch : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.DisableSmartInteract[Type] = true;
            TileID.Sets.Torch[Type] = true;

            DustType = ModContent.DustType<RuinTorchDust>();
            AdjTiles = new int[] { TileID.Torches };

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));

            // This code adds style-specific properties to style 1. Style 1 is used by ExampleWaterTorch. This code allows the tile to be placed in liquids. More info can be found in the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Tile#newsubtile-and-newalternate
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(1);

            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(255, 255, 255), Language.GetText("Ruin Torch"));

            // Assets
            if (!Main.dedServ)
            {
                flameTexture = ModContent.Request<Texture2D>("MOTLMod/Content/Tiles/LabyrinthSet/RuinTorch_Flame");
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Blocks.RuinItemSet.RuinTorch>();
        }

        public override float GetTorchLuck(Player player)
        {

            bool inCav = Main.LocalPlayer.ZoneNormalCaverns;
            return inCav ? 1f : -0.1f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            if (tile.TileFrameX < 66)
            {
                r = 0.45f;
                g = 0.41f;
                b = 0.47f;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // This code slightly lowers the draw position if there is a solid tile above, so the flame doesn't overlap that tile. Terraria torches do this same logic.
            offsetY = 0;

            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 4;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
            {
                return;
            }

            // The following code draws multiple flames on top our placed torch.

            int offsetY = 0;

            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 4;
            }

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.
            Color color = new Color(100, 100, 100, 0);
            int width = 20;
            int height = 20;
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            if (style == 1)
            {
                // ExampleWaterTorch should be a bit greener.
                color.G = 255;
            }

            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
