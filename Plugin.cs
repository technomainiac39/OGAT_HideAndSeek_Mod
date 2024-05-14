using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using CodeStage.AntiCheat.Detectors;
using SG.OGAT;
using SG.OGAT.State;
using SG.Util;
using System.Reflection;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System;
using SG.Transport;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using Steamworks;
using OGAT_modding_API;
using BepInEx.Logging;

namespace OGAT_HideAndSeek_Mod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("OGAT_modding_API", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            var harmony = new Harmony("com.technomainiac.OGAT_HideAndSeek_Mod");
            harmony.PatchAll();
        }

        public void Start()
        {

        }

        public void Update()
        {

        }
    }

    [HarmonyPatch]
    public class Patches        //for now will patch all of the custom shit to VIP gamemode but in the future will try just to make its own class and then patch it into the load game mode and menu shit
    {
        public static short VipClassID;
        public static PlayerClass VipClass; //remember still need to set = to vip class

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "gm_LoadConfig")]
        public static bool LoadCustomGameConfig(Mode_VIP __instance)        //loads all of the custom configs instead of the default ones
        {
            __instance.config = new GameMode.HJCKCFECIJA();
            __instance.config.KHEAJNIDEEO = GameMode.APMCPNJHDOK.COMPETITIVE;   //apparently there is a fun gamemode ??
            __instance.config.EPJPDICMEMG = false;  //still no clue what these are for
            __instance.config.PAKJCKDPCPI = false;
            __instance.config.EDBOAIJDGIK = false;
            __instance.config.IJABMNADOGJ = true;
            __instance.config.FEHOOCPAFCF = true;
            __instance.config.JCCBBGLCFGJ = true;
            __instance.config.PLEKGMCLICL = 0.5f;
            __instance.config.DMOLKIODAEF = true;
            __instance.config.FDLIEBOMBAK = "Hide & seek";
            __instance.config.KHPDMLLJJFP = "VIP";  //you cant change this part of config because OGAT checks for each of the game modes short names and will crash if it doesnt find them
            __instance.config.CGHIIEIBFJB = "In this mode, there are two teams Hiders and Seekers";
            GameMode.HJCKCFECIJA config = __instance.config;
            config.CGHIIEIBFJB += "\n\nThe objective of this game for the Seekers to kill all Hiders. However, if you are a seeker, you objective is to hide until time runs out. ";
            GameMode.HJCKCFECIJA config2 = __instance.config;
            config2.CGHIIEIBFJB += "\n\nThere is a time limit of 5 minutes if Hiders are not all killed by then they win. As soon as the Hiders die, the game is automatically over. ";
            GameMode.HJCKCFECIJA config3 = __instance.config;
            config3.CGHIIEIBFJB += "\n\nAs a guard, you have the task to clear the way so that the VIP can safely reach the escape point. Make use of all your weapons such as grenades and flares and make sure to reload often. As a killer, you do not need to kill all the guards. Your task is to kill the VIP. Only kill guards if necessary. It is a good idea to hide around the escape point however it is also predictable. Be smart.";
            __instance.config.ODHNCOOEHED = I18n.GetString("Seekers");
            __instance.config.BDHNLJMCPNL = I18n.GetString("Hiders");
        
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "gm_OnInit")]
        public static bool OnCustomGameInit(Mode_VIP __instance)        //sets all of the spawn stuff and also steals the VIP class
        {
            Match i = Singleton<Match>.I;
            i.respawnTime_blue.val = 13f;
            i.respawnTime_red.val = 10f;
            i.spawnKillProtectionTime_blue.val = 3f;
            i.spawnKillProtectionTime_red.val = 3f;
            i.matchLength.val = 360f;

            VipClass = __instance.vip_class;

            if(VipClass != null )
            {
                var myLogSource = new ManualLogSource("OGAT_HideAndSeek_Mod");
                BepInEx.Logging.Logger.Sources.Add(myLogSource);
                myLogSource.LogInfo("Vip class succesfully stolen");
                BepInEx.Logging.Logger.Sources.Remove(myLogSource);
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "S_onMatchStart")]
        public static bool OnMatchStart(Mode_VIP __instance)    //called when a match begins
        {
            __instance.BDEBLBJKCDF();   //is what the base.method calls
            __instance.vip = null;
            VipClassID = __instance.vip_class.GetClassId(); //gets the vip class ID very important

            SGNet.SendSystemChatMsg(IDJNNEJNMMO.All, "VIP CLASS ID IS {0}", new string[] { VipClassID.ToString() });

            List<NetPlayer> list = __instance.NDKACENNHIL(); //returns list of all red team players instead of KJDNHIIHFAG() which returns blue

            __instance.IMMDJHIGGBO();   //runs the start messages when the match begins
            SGNet.send_to_all(HFKPNENBLJI.CILJHAGDKOK, new KLEPJCJNCIJ[0]); //starts the match
            return false;       //HFKPNENBLJI.FDMNENNBHKB is set team
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClassSelection), "Show")]
        public static bool RestrictRedClassOnShow(ClassSelection __instance)
        {
            if (Singleton<Match>.I.GetGameModeIndex() == 2) //checks if game mode is VIP or not
            { 

                __instance.GGDNOHEGMGH = true;                          //turns on all of the UI stuff
                __instance.AGLMLJACABD.gameObject.SetActive(true);
                __instance.OMKHKJBFPNG.gameObject.SetActive(true);
                __instance.avatar_overlay.gameObject.SetActive(true);

                __instance.KCDANMPACMD();   //also turns on the UI and I think renders it too

                if (NetPlayer.Mine.GGOBEEOMBGG == IDJNNEJNMMO.Red)      //restricts class if red team but doesnt seem to be working 
                {
                    __instance.GGDNOHEGMGH = true;                              /////////////////////////////////////////////////////////////////
                    __instance.setUsableClass(VipClass);
                }
                else
                {
                    __instance.NEAFMMOJLAH();
                    __instance.GLIKEMJDBPB();
                    __instance.DIBIGLFNAIN();
                }

                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mode_VIP), "S_onPlayerConnected")]
        public static void OnPLayerConnected(Mode_VIP __instance, NetPlayer MFJOABACBHP)    //just to make sure there is no VIP though it isnt really needed since on death is altered
        {
            __instance.vip = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "S_onPlayerJoinMatch")]
        public static bool OnPlayerJoinMatch(Mode_VIP __instance, NetPlayer MFJOABACBHP)
        {
            SGNet.send_to_player(MFJOABACBHP, HFKPNENBLJI.CILJHAGDKOK, new KLEPJCJNCIJ[0]); //starts players match

            string B_message = I18n.Source("Find and kill all of the seekers before time is up");        //sends and sets both teams objectives again
            __instance.ACHKDONBLHJ(IDJNNEJNMMO.Blue, B_message, null, string.Empty);
            string R_message = I18n.GetString("Survive the seekers for the rest of the time limit");
            __instance.ACHKDONBLHJ(IDJNNEJNMMO.Red, R_message, null, string.Empty);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "IMMDJHIGGBO")]
        public static bool CustomStartMessages(Mode_VIP __instance)     //handles the custom start messages and objectives that are used in the match
        {
            __instance.vip = null;

            //blue team messages            
            string B_message = I18n.Source("Find and kill all of the seekers before time is up");
            __instance.GBCLBOJOEBA(IDJNNEJNMMO.Blue, B_message, null);
            __instance.ACHKDONBLHJ(IDJNNEJNMMO.Blue, B_message, null, string.Empty);

            //red team messages
            string R_message = I18n.Source("Survive the seekers for the rest of the time limit");
            __instance.GBCLBOJOEBA(IDJNNEJNMMO.Red, R_message, null);
            __instance.ACHKDONBLHJ(IDJNNEJNMMO.Red, R_message, null, string.Empty);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "S_onMatchTimeout")]
        public static bool OnTimeUp(Mode_VIP __instance, out bool CAHBHNFBEGJ, out bool JKBJAFMIPOP)
        {
            List<NetPlayer> redPlayers = __instance.NDKACENNHIL();
            if (redPlayers.Count != 0)
            {
                CAHBHNFBEGJ = false;    //think this is blue and red team win bool (the false one is blue team)
                JKBJAFMIPOP = true;

                Singleton<Match>.I.nWin_red.val++;
                SGNet.I.BroadcastPlayerStats();
                SGNet.SendSystemChatMsg(IDJNNEJNMMO.All, I18n.Source("The Hiders survived !!!"), new string[0]);

            }
            else                        //this sets blue team to win because ogat is funky in how it triggers wins
            {
                CAHBHNFBEGJ = true;     //this one I think sets the winning team
                JKBJAFMIPOP = true;     //this one always needs to be true

                Singleton<Match>.I.nWin_blue.val++;
                SGNet.I.BroadcastPlayerStats();
                SGNet.SendSystemChatMsg(IDJNNEJNMMO.All, I18n.Source("The Seekers win !!!"), new string[0]);
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mode_VIP), "S_onPlayerDeath")]
        public static bool OnPlayerDeath(Mode_VIP __instance, NetPlayer LIKDOCJJCBO, NetPlayer GPHOKCNEPPC)
        {
            List<NetPlayer> redPlayers = __instance.NDKACENNHIL();
            if (redPlayers.Count == 1 && GPHOKCNEPPC.GGOBEEOMBGG == IDJNNEJNMMO.Red)
            {
                SGNet.send_to_all(HFKPNENBLJI.FDMNENNBHKB, new KLEPJCJNCIJ[]
                {
                    KLEPJCJNCIJ.BPIFHLOBCLE((double)GPHOKCNEPPC.NetId),
                    KLEPJCJNCIJ.BPIFHLOBCLE((double)IDJNNEJNMMO.Blue)
                });
                SGNet.send_to_all(HFKPNENBLJI.OPINPPIFLLA, new KLEPJCJNCIJ[] {KLEPJCJNCIJ.BPIFHLOBCLE((double)IDJNNEJNMMO.Blue)});
            }
            else if (GPHOKCNEPPC.GGOBEEOMBGG == IDJNNEJNMMO.Red)
            {
                //sets scores
                float num = 100f;
                if (LIKDOCJJCBO.GGOBEEOMBGG == GPHOKCNEPPC.GGOBEEOMBGG)
                {
                    num *= -1f;
                }
                MatchPlayer matchPlayer = (!LIKDOCJJCBO.is_bot) ? Singleton<Match>.I.GetPlayerByUserID(LIKDOCJJCBO.profile.userID) : null;
                if (matchPlayer != null)
                {
                    matchPlayer.stat_score = Mathf.Max(0f, matchPlayer.stat_score + num);
                    SGNet.send_to_all(HFKPNENBLJI.NNKOKHKGEMC, new KLEPJCJNCIJ[]
                    {
                        KLEPJCJNCIJ.BPIFHLOBCLE((double)LIKDOCJJCBO.NetId),
                        KLEPJCJNCIJ.BPIFHLOBCLE((double)matchPlayer.stat_score)
                    });
                }

                //runs OGATS death logic and respawn stuff
                __instance.ALJAOGMGOPH(GPHOKCNEPPC, LIKDOCJJCBO);

                //swaps teams
                SGNet.send_to_all(HFKPNENBLJI.FDMNENNBHKB, new KLEPJCJNCIJ[]
                {
                    KLEPJCJNCIJ.BPIFHLOBCLE((double)GPHOKCNEPPC.NetId),
                    KLEPJCJNCIJ.BPIFHLOBCLE((double)IDJNNEJNMMO.Blue)
                });
                SGNet.SendSystemChatMsg(IDJNNEJNMMO.All, I18n.Source("{0} has become a seeker"), new string[]
                {
                GPHOKCNEPPC.profile.username,
                });

                //send the blue team objective again
                string B_message = I18n.Source("Find and kill all of the seekers before time is up");
                __instance.GBCLBOJOEBA(IDJNNEJNMMO.Blue, B_message, null);
                __instance.ACHKDONBLHJ(IDJNNEJNMMO.Blue, B_message, null, string.Empty);

                //shows class selection stuff if you are the one who died
                if (GPHOKCNEPPC == NetPlayer.Mine)
                {
                    Singleton<ClassSelection>.I.Show();

                    Singleton<ClassSelection>.I.selectedClassId = __instance.vip_class.GetClassId();
                    Singleton<ClassSelection>.I.selectedClassSkinId = ClassSelection.LOMMDIOPLKA((int)__instance.vip_class.GetClassId());
                    Singleton<ClassSelection>.I.KCDANMPACMD();
                    Singleton<ClassSelection>.I.GLIKEMJDBPB();
                    Singleton<ClassSelection>.I.DIBIGLFNAIN();
                }
            }
            else
            {
                //sets scores
                float num = 100f;
                if (LIKDOCJJCBO.GGOBEEOMBGG == GPHOKCNEPPC.GGOBEEOMBGG)
                {
                    num *= -1f;
                }
                MatchPlayer matchPlayer = (!LIKDOCJJCBO.is_bot) ? Singleton<Match>.I.GetPlayerByUserID(LIKDOCJJCBO.profile.userID) : null;
                if (matchPlayer != null)
                {
                    matchPlayer.stat_score = Mathf.Max(0f, matchPlayer.stat_score + num);
                    SGNet.send_to_all(HFKPNENBLJI.NNKOKHKGEMC, new KLEPJCJNCIJ[]
                    {
                        KLEPJCJNCIJ.BPIFHLOBCLE((double)LIKDOCJJCBO.NetId),
                        KLEPJCJNCIJ.BPIFHLOBCLE((double)matchPlayer.stat_score)
                    });
                }
                __instance.ALJAOGMGOPH(GPHOKCNEPPC, LIKDOCJJCBO);
            }


            return false;
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mode_Training), "S_onPlayerDeath")]
        public static void OnTrainingDeath(Mode_Training __instance, NetPlayer LIKDOCJJCBO, NetPlayer GPHOKCNEPPC) //player G.. is the killed one
        {
            short StartClass;
            IDJNNEJNMMO iDJNNEJNMMO = (GPHOKCNEPPC.GGOBEEOMBGG != IDJNNEJNMMO.Blue) ? IDJNNEJNMMO.Blue : IDJNNEJNMMO.Red;

            SGNet.send_to_all(HFKPNENBLJI.FDMNENNBHKB, new KLEPJCJNCIJ[]
            {
                KLEPJCJNCIJ.BPIFHLOBCLE((double)GPHOKCNEPPC.NetId),
                KLEPJCJNCIJ.BPIFHLOBCLE((double)iDJNNEJNMMO)
            });
            SGNet.SendSystemChatMsg(IDJNNEJNMMO.All, (iDJNNEJNMMO != IDJNNEJNMMO.Blue) ? I18n.Source("{0} has become a {2}") : I18n.Source("{0} has become a {1}"), new string[]
            {
                GPHOKCNEPPC.profile.username,
                Singleton<Game>.I.PKELIPPMFJE.config.ODHNCOOEHED,
                Singleton<Game>.I.PKELIPPMFJE.config.BDHNLJMCPNL
            });

            
            if (iDJNNEJNMMO == IDJNNEJNMMO.Blue)
            {
                StartClass = 0;
            }
            else
            {
                StartClass = 20;    //havent figured out a thieve id yet but its fine dont really need 
            }
            Singleton<ClassSelection>.I.Show();

            Singleton<ClassSelection>.I.selectedClassId = StartClass;
            Singleton<ClassSelection>.I.selectedClassSkinId = ClassSelection.LOMMDIOPLKA((int)StartClass);
            Singleton<ClassSelection>.I.KCDANMPACMD();
            Singleton<ClassSelection>.I.GLIKEMJDBPB();
            Singleton<ClassSelection>.I.DIBIGLFNAIN();

        }*/

    }
}