using Microsoft.Xna.Framework;
using System;
using System.Drawing.Drawing2D;
using Terraria;
using Terraria.ModLoader;


namespace MOTLMod.Content.Items.Weapons
{
    public class MinotaursAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 81;
            Item.height = 87;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = 1;
            Item.useTime = 10;
            Item.useAnimation = 80;
            Item.knockBack = 7;
            Item.crit = 6;
        }


        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += Math.Abs(player.velocity.X);
        }
        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime <= 0)
            {
                player.velocity.X = player.maxRunSpeed * 2 * player.direction;
            }

            return true;
        }
    }
}
