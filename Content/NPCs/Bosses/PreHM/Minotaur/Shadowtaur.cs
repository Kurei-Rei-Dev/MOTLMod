using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Xna.Framework;
using MOTLMod.Common.Systems;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            NPC.width = 94;
            NPC.height = 142;
            NPC.damage = 20;
            NPC.defense = 17;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
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
                int finalFrame = 5;

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
                int finalFrame = 14;

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

        public bool Idle = true; // 0
        public bool Walking = false; // 1
        public bool RunWindUp = false; //2
        public bool Run = false; //22
        public bool Swing = false; //3
        public bool Throw = false; //4
        public bool Catch = false; //44
        public bool SlamWindUp = false; //5
        public bool Whirlwind = false; //6
        public bool Roar = false; //7

        public int AttackTime = 0;
        public int AttackType = 0;

        public override bool PreAI()
        {
            
            NPC.TargetClosest();
            NPC.velocity *= 0;
            Vector2 directionToPlayer = Main.player[NPC.target].Center - NPC.Center;
            directionToPlayer.Normalize();

            #region dir stuff
            if (directionToPlayer.X > 0)
            {
                NPC.spriteDirection = 1; // Face right
            }
            else if (directionToPlayer.X < 0)
            {
                NPC.spriteDirection = -1; // Face left
            }
            #endregion

            if (AttackType == 0) //idle
            {
                if (AttackTime < 55)
                {
                    Walking = false;
                    Idle = true;
                    RunWindUp = false;
                    Run = false; 
                    Swing = false; 
                    Throw = false; 
                    Catch = false;
                    SlamWindUp = false;
                    Whirlwind = false;
                    Roar = false; 

                    NPC.aiStyle = 3;
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 1;
                    AttackTime = 0;
                }
            }
            else if (AttackType == 1) // walk
            {
                if (AttackTime < 120)
                {
                    Idle = false;
                    Walking = true;
                    RunWindUp = false;
                    Run = false;
                    Swing = false;
                    Throw = false;
                    Catch = false;
                    SlamWindUp = false;
                    Whirlwind = false;
                    Roar = false;

                    NPC.velocity.X = directionToPlayer.X * 3f;
                    NPC.velocity.Y = 7f;
                    AttackTime += 1;
                }
                else
                {
                    //AttackType = Main.rand.Next(2, 8);
                    //AttackTime = 0;
                    ChooseRandomAttack();
                }
            }
            else if (AttackType == 2)// getting ready to run
            {
                if (AttackTime < 30)
                {
                    Idle = false;
                    Walking = false;
                    RunWindUp = true;
                    NPC.aiStyle = 3;
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 22;
                    AttackTime = 0;
                }
            }
            else if (AttackType == 22) //run
            {
                if (AttackTime < 200)
                {
                    Idle = false;
                    Walking = false;
                    RunWindUp = false;
                    Run = true;
                    NPC.velocity.X = directionToPlayer.X * 6f;
                    NPC.velocity.Y = 7f;
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 0;
                    AttackTime = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                }
            }
            else if (AttackType == 3) //swing
            {
                if (AttackTime < 20)
                {
                    Idle = false;
                    Walking = false;
                    Swing = true;
                    NPC.aiStyle = 3;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    //proj stuff here
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 0;
                    AttackTime = 0;

                }
            }
            else if (AttackType == 4)// throw
            {
                if (AttackTime < 30)
                {
                    Idle = false;
                    Walking = false;
                    Throw = true;
                    NPC.aiStyle = 3;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    //projectile stuff here
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 44; //catch
                    AttackTime = 0;
                }
            }
            else if (AttackType == 44) //catch
            {
                if (AttackTime < 100)
                {
                    Throw = false;
                    Catch = true;
                    NPC.aiStyle = 3;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    //projectile stuff i guess lmao
                    //music
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 0;
                    AttackTime = 0;
                }
            }
            else if (AttackType == 5)// Slam  Wind Up 
            {
                if (AttackTime < 30)
                {
                    Idle = false;
                    Walking = false;
                    SlamWindUp = true;
                    NPC.aiStyle = 3;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    //Proj stuff 😭
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 3;
                    AttackTime = 0;
                }
            }
            else if (AttackType == 6)// whirlwind
            {
                if (AttackTime < 140)
                {
                    Idle = false;
                    Walking = false;
                    Whirlwind = true;
                    NPC.velocity.X = directionToPlayer.X * 5f;
                    NPC.velocity.Y = 7f;
                    AttackTime += 1;
                }
                else
                {
                    AttackType = 0;
                    AttackTime = 0;
                    NPC.AddBuff(BuffID.Dazed, 180);
                }
            }
            else if (AttackType == 7)//Roar goddamn okay lol
            {
                if (AttackTime < 120)
                {
                    Idle = false;
                    Walking = false;
                    Roar = true;
                    NPC.aiStyle = 3;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    AttackTime += 1;
                }
                else
                {
                    AttackType = 0;
                    AttackTime = 0;
                }
            }

            void ChooseRandomAttack()
            {
                AttackType = Main.rand.Next(2, 8); // Choose a random attack type
                AttackTime = 0; // Reset attack timer
            }

            return false;
        }
    }
}
