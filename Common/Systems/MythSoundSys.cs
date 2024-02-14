using MOTLMod.Content.NPCs.Bosses.PreHM.Minotaur;
using Terraria.Audio;
using Terraria.ModLoader;

namespace MOTLMod.Common.Systems
{

    public class MythSoundSys : ModSystem
    {
       //.ogg files please
        public static readonly SoundStyle ShadowtaurDeath;


        static MythSoundSys()
        {
            ShadowtaurDeath = new SoundStyle("MOTLMod/Assets/Sounds/ShadowtaurDeath", (SoundType)0);
        }
    }
}