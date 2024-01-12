using System;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace MOTLMod.Content.Items.Placeables.Blocks.RuinItemSet
{
    public class RuinPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = 1;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = Mod.Find<ModTile>("RuinPlatform").Type;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<RuinItemSet.RuinBrick>(), 1).Register();
        }
    }
}
