using System;
using BepInEx;
using Noise;
using On;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HUD;
using MoreSlugcats;
using RWCustom;
using On.Menu;
using System.Xml;
using IL;
using System.Linq;
using ImprovedInput;

namespace Pearlventory
{
    [BepInPlugin("nc.pearlventory", "Pearlventory", "1.0.0")]
    class PearlventoryMain : BaseUnityPlugin
    {

        /*
        public static int Default(int val, int def)
        {
            if (val == null)
            {
                return def;
            }
            else// if (val != null)
            {
                return val;
            }
        }
        */

        public static readonly PlayerKeybind IntakePearl = PlayerKeybind.Register("ncpearlventory:intakepearl", "Pearlventory", "Explode", KeyCode.LeftShift, KeyCode.JoystickButton3);
        public static readonly PlayerKeybind OuttakePearl = PlayerKeybind.Register("ncpearlventory:outtakepearl", "Pearlventory", "Explode", KeyCode.LeftControl, KeyCode.JoystickButton4);

        public static int PearlAmount = 0; //the amount pearls the player is currently holding
        public static int PearlLimit = PearlventoryOptions.MaxPearls.Value;//10; //the max amount of pearls the player can hold, default is ten(10)

        public void OnEnable()
        {
            On.Player.Update += Player_PearlIntake;
            On.Player.Update += Player_PearlOuttake;
            //On.HUD.HudPart.Draw += HudPart_Draw;
            On.Player.Update += DebugInfo;
        }


        public static int Timer = 0;
        private void DebugInfo(On.Player.orig_Update orig, Player self, bool eu)
        {
            Timer++;

            if(Timer == 160)
            {
                Timer = 0;
                Logger.LogInfo("Pearlventory, PearlAmount:" + PearlAmount.ToString());
                Logger.LogInfo("Pearlventory, PearlLimit:" + PearlLimit.ToString());
                Debug.Log("Pearlventory, PearlAmount:" + PearlAmount.ToString());
                Debug.Log("Pearlventory, PearlLimit:" + PearlLimit.ToString());
            }

            orig(self, eu);
        }

        private void Player_PearlOuttake(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room.game.IsStorySession)
            {
                if (self.objectInStomach == null) //(self.grasps[0] == null)
                {
                    //if (self.grasps[0].grabbed is null) //(self.objectInStomach == null)
                    //{
                        if (PearlAmount > 0)
                        {
                            if (self.JustPressed(OuttakePearl)/*Input.GetKey(KeyCode.LeftControl)*/)
                            {
                                //IntVector2 tilePosition = self.room.game.cameras[0].room.GetTilePosition(Input.mousePosition + camOffset);
                                //WorldCoordinate worldCoordinate = game.cameras[0].room.GetWorldCoordinate(tilePosition);
                            #region PearlToHand
                                //var pearl = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, /*self.abstractCreature.pos*/self.room.GetWorldCoordinate(self.firstChunk.pos), self.room.game.GetNewID()); //creates the pearl
                                //self.room.abstractRoom.AddEntity(pearl); //adds the pearl to the room
                                //pearl.RealizeInRoom(); //realizes the pearl in the room
                                /*
                                if (self.FreeHand() != -1)
                                {
                                    self.SlugcatGrab(pearl.realizedObject, self.FreeHand()); //forces the scug to grab the pearl
                                }
                                */
                            #endregion

                            #region PearlToStomach
                                var stomachPearl = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, self.room.GetWorldCoordinate(self.firstChunk.pos), self.room.game.GetNewID(), -1, -1, null);
                                self.objectInStomach = stomachPearl;
                            #endregion
                                PearlAmount--;
                            }
                        }
                    //}
                }
            }

            orig(self, eu);
        }

        private void Player_PearlIntake(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room.game.IsStorySession)
            {
                if (self.grasps[0] != null)
                {
                    if (self.grasps[0].grabbed is not null)
                    {
                        if (self.grasps[0].grabbed is DataPearl)
                        {
                            if (PearlAmount < PearlLimit)
                            {
                                if (self.JustPressed(IntakePearl)/*Input.GetKey(KeyCode.LeftShift)*/)
                                {
                                    //if (self.grasps[0].grabbed is not null and DataPearl) //grasp redundancy
                                    //{
                                    self.grasps[0].grabbed.Destroy(); //removes pearl from hand
                                                                      //}
                                    PearlAmount++;
                                }
                            }
                        }
                    }
                }
            }
            orig(self, eu);
        }
    }
}