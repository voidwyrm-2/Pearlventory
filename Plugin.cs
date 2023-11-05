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

namespace Pearlventory
{
    [BepInPlugin("nc.pearlventory", "Pearlventory", "1.0.0")]
    class PearlventoryMain : BaseUnityPlugin
    {
        public static int PearlAmount = 0; //the amount pearls the player is currently holding
        public static int PearlLimit = 10; //the max amount of pearls the player can hold

        public void OnEnable()
        {
            On.Player.Update += Player_PearlIntake;
            On.Player.Update += Player_PearlOuttake;
            //On.HUD.HudPart.Draw += HudPart_Draw;
            On.Player.Update += DebugInfo;
        }

        private void DebugInfo(On.Player.orig_Update orig, Player self, bool eu)
        {
            Logger.LogInfo("Pearlventory, PearlAmount:" + PearlAmount.ToString());
            Logger.LogInfo("Pearlventory, PearlLimit:" + PearlLimit.ToString());
            Debug.Log("Pearlventory, PearlAmount:" + PearlAmount.ToString());
            Debug.Log("Pearlventory, PearlLimit:" + PearlLimit.ToString());
            orig(self, eu);
        }

        private void Player_PearlOuttake(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (PearlAmount > 0)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (self.grasps[0].grabbed is null) //(self.objectInStomach == null)
                    {
                        //IntVector2 tilePosition = game.cameras[0].room.GetTilePosition(Input.mousePosition + camOffset);
                        //WorldCoordinate worldCoordinate = game.cameras[0].room.GetWorldCoordinate(tilePosition);
                        var pearl = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, self.abstractCreature.pos, self.room.game.GetNewID()); //creates the pearl
                        self.room.abstractRoom.AddEntity(pearl); //adds the pearl to the room
                        pearl.RealizeInRoom(); //realizes the pearl in the room
                        if (self.FreeHand() != -1)
                        {
                            self.SlugcatGrab(pearl.realizedObject, self.FreeHand()); //forces thee scug to grab the pearl
                        }
                        //self.objectInStomach = pearl;
                        PearlAmount--;
                    }
                }
            }
            orig(self, eu);
        }

        private void Player_PearlIntake(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (PearlAmount < PearlLimit)
            {
                if (self.grasps[0].grabbed is not null)
                {
                    if (self.grasps[0].grabbed is DataPearl)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (self.grasps[0].grabbed is not null and DataPearl) //grasp redundancy
                            {
                                self.grasps[0].grabbed.Destroy(); //removes pearl from hand
                            }
                            PearlAmount++;
                        }
                    }
                }
            }
            orig(self, eu);
        }

        /*
        private void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            orig(self, cam);

            if (self.rainWorld.processManager.currentMainLoop is RainWorldGame game && game.IsStorySession)
            {
                CurrentCycleScore = 10;
                CurrentAverageScore = GetAverageScore(self.rainWorld.progression.currentSaveState);

                if (Options.ShowRealTime.Value)
                {
                    self.AddPart(new ScoreCounter(self)
                    {
                        DisplayedScore = CurrentCycleScore,
                    });
                }
            }
        }
        */
    }

    /*
    sealed class ScoreCounter : HUD.HudPart
    {
        sealed class ScoreBonus
        {
            public int Initial;

            public int Add;
            public bool Stacks;
            public Color Color;
            public int Delay;

            public FLabel label;
            public IconSymbol symbol;
        }
        sealed class DelayedKillBonus
        {
            public int Score;
            public Color Color;
            public IconSymbol.IconSymbolData Icon;
            public int Delay;
        }

        public int DisplayedScore;
        public int TargetScore => Plugin.CurrentAverageScore;

        readonly List<ScoreBonus> bonuses = new();
        readonly List<DelayedKillBonus> delayedBonuses = new();
        readonly FLabel scoreText;
        readonly FSprite darkGradient;
        readonly FSprite lightGradient;

        Vector2 pos;
        Vector2 lastPos;

        public ScoreCounter(HUD.HUD hud) : base(hud)
        {
            pos = new Vector2(hud.rainWorld.screenSize.x - 20f + 0.01f, 20.01f);

            hud.fContainers[0].AddChild(scoreText = new FLabel(Custom.GetDisplayFont(), "0"));

            hud.fContainers[0].AddChild(darkGradient = new("Futile_White", true)
            {
                color = new Color(0f, 0f, 0f),
                shader = hud.rainWorld.Shaders["FlatLight"]
            });

            hud.fContainers[0].AddChild(lightGradient = new("Futile_White", true)
            {
                shader = hud.rainWorld.Shaders["FlatLight"]
            });
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);

            scoreText.color = Plugin.ScoreTextColor(DisplayedScore, TargetScore);
            lightGradient.color = Plugin.ScoreTextColor(DisplayedScore, TargetScore);

            Vector2 pos = Vector2.Lerp(lastPos, this.pos, timeStacker);
            scoreText.x = pos.x;
            scoreText.y = pos.y;

            bool blinkBright = bonuses.Count > 0 && incrementDelay % 16 < 8;

            float alpha = Mathf.Lerp(lastAlpha, this.alpha, timeStacker);
            scoreText.alpha = alpha * (blinkBright ? 1f : 0.5f);
            darkGradient.x = pos.x;
            darkGradient.y = pos.y;
            darkGradient.scale = Mathf.Lerp(35f, 40f, alpha) / 16f;

            float bump = Mathf.Lerp(lastBump, this.bump, timeStacker);
            darkGradient.alpha = 0.17f * Mathf.Pow(alpha, 2f) + 0.1f * bump * (blinkBright ? 1f : 0.5f);
            lightGradient.x = pos.x;
            lightGradient.y = pos.y;
            lightGradient.scale = Mathf.Lerp(40f, 50f, Mathf.Pow(bump, 2f)) / 16f;
            lightGradient.alpha = bump * 0.2f;

            int i = 0;
            foreach (var bonus in bonuses)
            {
                i++;

                pos.x -= 28;

                if (bonus.symbol != null)
                {
                    if (bonus.symbol.symbolSprite == null)
                    {
                        bonus.symbol.Show(true);
                    }

                    pos.x -= 0.5f * bonus.symbol.symbolSprite.element.sourcePixelSize.x;

                    bonus.symbol.Draw(timeStacker, pos - new Vector2(0, 2));
                    bonus.symbol.symbolSprite.color = bonus.Color;
                    bonus.symbol.symbolSprite.alpha = Mathf.Min(alpha, 0.5f + 0.5f * Mathf.Sign(i + 0.5f + clock / Mathf.PI / 40f));

                    pos.x -= 0.5f * bonus.symbol.symbolSprite.element.sourcePixelSize.x + 14;
                }

                if (bonus.Initial > 9)
                {
                    pos.x -= 8;
                }
                if (bonus.Initial > 99)
                {
                    pos.x -= 8;
                }

                bonus.label.scale = 0.8f;
                bonus.label.text = Plugin.FmtAdd(bonus.Add);
                bonus.label.color = bonus.Color;
                bonus.label.alpha = Mathf.Min(alpha, 0.5f + 0.5f * Mathf.Sign(i + clock / Mathf.PI / 40f));
                bonus.label.x = pos.x;
                bonus.label.y = pos.y;
            }
        }

    }
    */
}