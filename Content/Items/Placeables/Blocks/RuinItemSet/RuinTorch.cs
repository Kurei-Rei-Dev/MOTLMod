using MOTLMod.Content.Tiles.LabyrinthSet;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using MOTLMod.Content.Dusts.SpecificDusts;

namespace MOTLMod.Content.Items.Placeables.Blocks.RuinItemSet
{
    public class RuinTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.Torches[Type] = true;
        }

        public override void HoldItem(Player player)
        {
            // This torch cannot be used in water, so it shouldn't spawn particles or light either
            if (player.wet)
            {
                return;
            }
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30))
            {
                Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<RuinTorchDust>(), 0f, 0f, 100);
                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                }

                dust.velocity *= 0.3f;
                dust.velocity.Y -= 1.5f;
                dust.position = player.RotatedRelativePoint(dust.position);
            }

            // Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 1f, 1f, 1f);
        }

        public override void PostUpdate()
        {
            // Create a white (1.0, 1.0, 1.0) light when the item is in world, and isn't underwater.
            if (!Item.wet)
            {
                Lighting.AddLight(Item.Center, 1f, 1f, 1f);
            }
        }

        public override void SetDefaults()
        {
            Item.DefaultToTorch(ModContent.TileType<Tiles.LabyrinthSet.RuinTorch>(), 0, false);
            Item.value = 25;
            Item.createTile = Mod.Find<ModTile>("RuinTorch").Type;
        }
    }
}