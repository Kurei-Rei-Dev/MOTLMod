﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace MOTLMod.Content.Dusts.SpecificDusts
{
    public class RuinTorchDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f; 
            dust.noGravity = false; 
            dust.noLight = true; 
            dust.scale *= 1.2f; 
        }

        public override bool Update(Dust dust)
        { 
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.99f;

            float light = 0.15f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);

            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }

            return false; 
        }
    }
}