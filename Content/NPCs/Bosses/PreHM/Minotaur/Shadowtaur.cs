using Microsoft.Xna.Framework;
using MOTLMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace MOTLMod.Content.NPCs.Bosses.PreHM.Minotaur
{
    public class Shadowtaur : ModNPC
    {
        public static int secondPhaseHeadSlot = -1;
        public bool SecondPhase
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value ? 1f : 0f;
        }

        public Vector2 FirstPhaseDestination
        {
            get => new Vector2(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        public Vector2 LastFirstPhaseDestination { get; set; } = Vector2.Zero;

        private const int FirstPhaseTimerMax = 90;
        // This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
        public ref float FirstPhaseTimer => ref NPC.localAI[1];

        // We could also repurpose FirstStageTimer since it's unused in the second stage, or write "=> ref FirstStageTimer", but then we have to reset the timer when the state switch happens
        public ref float SecondPhaseTimer => ref NPC.localAI[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 44;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            /*
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            */
        }
        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 142;
            NPC.damage = 20;
            NPC.defense = 17;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 4286;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = MythSoundSys.ShadowtaurDeath;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;

            NPC.aiStyle = -1;
            //NPC.BossBar = ModContent.GetInstance<ShadowtaurBossBar>();

            /* Nonexistent Music track fr
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Takedown");
            }
            */
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Treasure Bag
            //npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ShadowtaurBag>()));

            // Trophy
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>(), 10));

            // Relic
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.MinionBossRelic>()));

            // Pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<ShadowtaurPet>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ShadowtaurMask>(), 7));

            npcLoot.Add(notExpertRule);
        }
        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSys.downedShadowtaur, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/                                                           //imma just keep this here
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // using the boss immunity cooldown counter to prevent ignoring boss attacks by taking damage from other sources. Damn who coded this in this be devious
            return true;
        }

        #region FrameDefStuff
        public override void FindFrame(int frameHeight)
        {
            if (Idle == true)
            {
                int startFrame = 0;
                int finalFrame = 6;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Walking == true)
            {
                int startFrame = 7;
                int finalFrame = 11;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (RunWindUp == true)
            {
                int startFrame = 13;
                int finalFrame = 13;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Run == true)
            {
                int startFrame = 15;
                int finalFrame = 17;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Swing == true)
            {
                int startFrame = 19;
                int finalFrame = 23;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Throw == true)
            {
                int startFrame = 25;
                int finalFrame = 28;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Catch == true)
            {
                int startFrame = 30;
                int finalFrame = 29;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (SlamWindUp == true)
            {
                int startFrame = 31;
                int finalFrame = 33;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Whirlwind == true)
            {
                int startFrame = 35;
                int finalFrame = 37;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
            else if (Roar == true)
            {
                int startFrame = 39;
                int finalFrame = 43;

                int frameSpeed = 1;
                NPC.frameCounter += 0.4f;
                if (NPC.frameCounter > frameSpeed)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > finalFrame * frameHeight)
                    {
                        NPC.frame.Y = startFrame * frameHeight;
                    }
                }
            }
        }
        #endregion

        public bool Idle = true;
        public bool Walking = false;
        public bool RunWindUp = false;
        public bool Run = false;
        public bool Swing = false;
        public bool Throw = false;
        public bool Catch = false;
        public bool SlamWindUp = false;
        public bool Whirlwind = false;
        public bool Roar = false;

        public override bool PreAI()
        {
            NPC.width = 94;
            NPC.height = 142;
            NPC.TargetClosest();

            if (Idle == true)
            {
                AIType = NPCID.GrayGrunt;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
