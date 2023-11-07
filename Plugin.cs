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
    public class PearlventoryMain : BaseUnityPlugin
    {

        /*
        public static int Default(int val, int def)
        {
            if (val == null)
            {
                return def;
            }
            else //if (val != null)
            {
                return val;
            }
        }
        */

        //public static void AbstractStoringItem() { var StorableAbstractItem = AbstractPhysicalObject.AbstractObjectType.DataPearl; }

        //sealed class PlayerData { public int sleepTime; }

        public static PlayerKeybind StoreItem; //= PlayerKeybind.Register("ncpearlventory:StoreItem", "Pearlventory", "Store Pearl", KeyCode.RightShift, KeyCode.JoystickButton3);
        public static PlayerKeybind UnstoreItem; //= PlayerKeybind.Register("ncpearlventory:UnstoreItem", "Pearlventory", "Un-store Pearl", KeyCode.RightControl, KeyCode.JoystickButton4);
        public static PlayerKeybind EatPearls;

        public static int StoredItemAmount = 0; //the amount pearls the player is currently holding
        
        public static int StoredItemLimit = PearlventoryOptions.MaxPearls.Value; //10; //the max amount of pearls the player can hold, default is ten(10)

        public static string modMessage = "from Pearlventory: ";

        public void OnEnable()
        {
            try
            {
                StoreItem = PlayerKeybind.Register("ncpearlventory:StoreItem", "Pearlventory", "Store Pearl", KeyCode.RightShift, KeyCode.JoystickButton3);
            }
            catch (Exception e)
            {
                Logger.LogError(modMessage + e);
            }

            try
            {
                UnstoreItem = PlayerKeybind.Register("ncpearlventory:UnstoreItem", "Pearlventory", "Un-store Pearl", KeyCode.RightControl, KeyCode.JoystickButton4);
            }
            catch (Exception e)
            {
                Logger.LogError(modMessage + e);
            }

            try
            {
                EatPearls = PlayerKeybind.Register("ncpearlventory:UnstoreItem", "Pearlventory", "Un-store Pearl", KeyCode.RightAlt, KeyCode.JoystickButton5);
            }
            catch (Exception e)
            {
                Logger.LogError(modMessage + e);
            }

            On.Player.Update += Player_PearlIntake;
            On.Player.Update += Player_PearlOuttake;
            //On.HUD.HudPart.Draw += HudPart_Draw;
            On.Player.Update += Player_GainFoodFromPearl;
            On.Player.Update += DebugInfo;

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            MachineConnector.SetRegisteredOI("nc.pearlventory", new PearlventoryOptions());
        }

        public static int Timer = 0;
        private void DebugInfo(On.Player.orig_Update orig, Player self, bool eu)
        {
            Timer++;
            int TimerLimit = 160;
            string DebugModNamePrefix = "Pearlventory, ";
            string DebugPrefix = "Pearl";


            if (Timer == TimerLimit)
            {
                Timer = 0;
                Logger.LogInfo(DebugModNamePrefix + DebugPrefix + "Amount:" + StoredItemAmount.ToString());
                Logger.LogInfo(DebugModNamePrefix + DebugPrefix + "Limit:" + StoredItemLimit.ToString());
                Debug.Log(DebugModNamePrefix + DebugPrefix + "Amount:" + StoredItemAmount.ToString());
                Debug.Log(DebugModNamePrefix + DebugPrefix + "Limit:" + StoredItemLimit.ToString());
            }

            orig(self, eu);
        }

        private void Player_GainFoodFromPearl(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room.game.IsStorySession)
            {
                if (PearlventoryOptions.GainFood.Value == true)
                {
                    if (StoredItemAmount > 0)
                    {
                        if (/*Input.GetKeyDown(PearlventoryOptions.StoreItemKey.Value)*/ /*self.IsPressed(EatPearls)*/ Input.GetKey(KeyCode.RightAlt))
                        {
                            StoredItemAmount--;
                            self.AddFood(PearlventoryOptions.FoodGained.Value);
                        }
                    }
                }
            }
        }

        private void Player_PearlOuttake(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.room.game.IsStorySession)
            {
                if (self.objectInStomach == null) //(self.grasps[0] == null)
                {
                    if (StoredItemAmount > 0)
                    {
                        if (/*Input.GetKeyDown(PearlventoryOptions.UnstoreItemKey.Value)*/ /*self.IsPressed(UnstoreItem)*/ Input.GetKey(KeyCode.LeftControl))
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
                            StoredItemAmount--;
                            }
                        }
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
                            if (StoredItemAmount < StoredItemLimit)
                            {
                                if (/*Input.GetKeyDown(PearlventoryOptions.StoreItemKey.Value)*/ /*self.IsPressed(StoreItem)*/ Input.GetKey(KeyCode.LeftShift))
                                {
                                    self.grasps[0].grabbed.Destroy(); //removes pearl from hand
                                    StoredItemAmount++;
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