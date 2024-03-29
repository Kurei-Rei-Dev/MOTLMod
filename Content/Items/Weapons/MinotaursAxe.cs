using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

        float yPos = 0f;
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += Math.Abs(player.velocity.X);
        }
        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime <= 0)
            {
                if (yPos == 0f)
                {
                    yPos = (Main.MouseWorld.Y - player.position.Y) / 10;
                }
                player.velocity.X = player.maxRunSpeed * 3 * player.direction;
                player.velocity.Y = (player.velocity.Y + yPos) / 2;

            }
            else
            {
                yPos = 0f;
            }

            return true;
        }
    }
}
