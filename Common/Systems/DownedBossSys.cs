using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MOTLMod.Common.Systems
{
    // Acts as a container for "downed boss" flags.
    // Set a flag like this in your bosses OnKill hook:
    //    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

    public class DownedBossSys : ModSystem
    {
        public static bool downedShadowtaur = false;
        // public static bool blahblahblah = false;

        public override void ClearWorld()
        {
            downedShadowtaur = false;
            // blahblahblah = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedShadowtaur)
            {
                tag["downedShadowtaur"] = true;
            }

            // if (downedother) {
            //	tag["downedother"] = true;
            // }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedShadowtaur = tag.ContainsKey("downedShadowtaur");
            //dwonedother = tag.ContainsKey("downedother");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            var flags = new BitsByte();
            flags[0] = downedShadowtaur;
            // flags[1] = downedother;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            BitsByte flags = reader.ReadByte();
            downedShadowtaur = flags[0];
            // downedOtherBoss = flags[1];

            // As mentioned in NetSend, BitBytes can contain up to 8 values. If you have more, be sure to read the additional data:
        }
    }
}
