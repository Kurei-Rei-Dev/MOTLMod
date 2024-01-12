using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace MOTLMod.Content.Items.Placeables.Blocks.RuinItemSet
{
    public class RuinPillar : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = Mod.Find<ModTile>("RuinPillar").Type;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).AddIngredient(ItemID.StoneBlock, 5).AddIngredient(ModContent.ItemType<RuinItemSet.RuinBrick>(), 5).AddTile(TileID.Furnaces).Register();
        }
    }
}
