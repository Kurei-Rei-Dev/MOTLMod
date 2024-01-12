using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MOTLMod.Content.Items.Placeables.Blocks.RuinItemSet
{
    internal class RuinBrick : ModItem
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
            Item.createTile = Mod.Find<ModTile>("RuinBrick").Type;
        }

        public override void AddRecipes()
        {
            //CreateRecipe(1).AddIngredient(ModContent.ItemType<RuinWall>(), 4).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<RuinItemSet.RuinPlatform>(), 2).Register();
        }
    }
}
