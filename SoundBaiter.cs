using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using Rust;
using RustNative;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("SoundBaiter", "Manvaril", "1.0.0")]
    [Description("Play rust sounds to nearby players.")]

    class SoundBaiter : RustPlugin
    {
        private new void LoadDefaultMessages()
        {
            // English
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["PluginName"] = "[<color=#6275a4>SoundBaiter</color>]",
                ["NotAllowed"] = "You are not allowed to use the '{0}' command",
                ["Invalid"] = "Invalid Syntax: /{command} {args[0]}",
                ["Usage"] = " /{0} show/hide",
                ["PanelHidden"] = " Hidden",
                ["PanelRefresh"] = " Refreshed/Shown",
                ["Title"] = "Sound Baiter [<color=#FF0000>CLOSE</color>]",
                ["TBuild"] = "Twig Build",
                ["TBuild"] = "Twig Build",
                ["TBreak"] = "Twig Break",
                ["WDOpen"] = "Wood Door Open",
                ["WDClose"] = "Wood Door Close",
                ["MDOpen"] = "Metal Door Open",
                ["MDClose"] = "Metal Door Close",
                ["CLWDOpen"] = "CL Wood Door Open",
                ["CLWDClose"] = "CL Wood Door Close",
                ["CLMDOpen"] = "CL Metal Door Open",
                ["CLMDClose"] = "CL Metal Door Close",
                ["LHOpen"] = "Ladder Hatch Open",
                ["LHClose"] = "Ladder Hatch Close",
                ["CLDenied"] = "CL Denied",
                ["CLShock"] = "CL Shock",
                ["CLUnlock"] = "CL Unlock",
                ["CLChanged"] = "CL Changed",
                ["SHTree"] = "Stone Hatchet Tree",
                ["SPOre"] = "Stone Pick Ore",
                ["MHTree"] = "Metal Hatchet Tree",
                ["MHOre"] = "Metal Pick Ore",
                ["SATree"] = "Sal Axe Tree",
                ["SPOre"] = "Sal Pick Ore",
                ["LMTrigger"] = "Land Mine Trigger",
                ["LMBlow"] = "Land Mine Blow",
                ["GTShoot"] = "Gun Trap Shoot",
                ["RBlow"] = "Rocket Blow",
                ["C4Blow"] = "C4 Blow",
                ["SBlow"] = "Satchel Blow",
                ["BCBlow"] = "Beancan Blow",
                ["F1Blow"] = "F1 Granade Blow",
                ["BTSnap"] = "Bear Trap Snap",
                ["DScream"] = "Death Scream",
                ["BHit"] = "Barrel Hit",
                ["BBreak"] = "Barrel Break",
                ["BMImpact"] = "Bullet Metal Impact",
                ["BRImpact"] = "Bullet Rock Impact",
                ["HShot"] = "Headshot",
                ["Murderer"] = "Murderer Breathing",
                ["CloseGUI"] = "Close"
            }, this);
        }

        private const string permAllow = "soundbaiter.allow";

        private void Init()
        {
            permission.RegisterPermission(permAllow, this);
        }

        [ChatCommand("soundbait")]
        private void cmdSoundBait(BasePlayer player, string command, string[] args)
        {
            if (!IsAllowed(player.UserIDString, permAllow))
            {
                Reply(player, Lang("NotAllowed", null, command));
                return;
            }

            if (args.Length == 0)
            {
                Reply(player, Lang("Usage", null, command));
                return;
            }

            switch (args[0])
            {
                case "hide":
                    CuiHelper.DestroyUi(player, "sbGUI");
                    Reply(player, Lang("PanelHidden"));
                    break;

                case "show":
                    BaitGui(player);
                    Reply(player, Lang("PanelRefresh"));
                    break;

                default:
                    SendReply(player, $"[<color=#6275a4>SoundBaiter</color>]: Invalid Syntax /{command} {args[0]}");
                    return;
            }
        }

        [ConsoleCommand("soundbait")]
        private void ccmdSoundBait(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (player == null) return;
            var args = arg?.Args ?? null;
            if (IsAllowed(player.UserIDString, permAllow))
            {
                switch (args[0])
                {
                    case "action":
                        if (args[1] == "twig")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "thatch")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/building/thatch_gib.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "beartrap")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/beartrap/fire.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "headshot")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "barrelhit")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/entities/loot_barrel/impact.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "barrelbreak")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/entities/loot_barrel/gib.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "bulletmetal")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/impacts/bullet/metal/metal1.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "bulletrock")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/impacts/bullet/rock/bullet_impact_rock.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "scream")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/player/beartrap_scream.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "wooddooropen")
                        {
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-open-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(3, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-open-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "wooddoorclose")
                        {
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-close-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(0.5f, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-close-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "metaldooropen")
                        {
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-open-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(1, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-open-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "metaldoorclose")
                        {
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-close-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(0.5f, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-close-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "ladderhatchopen")
                        {
                            Effect.server.Run("assets/prefabs/building/floor.ladder.hatch/effects/door-ladder-hatch-open-start.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "ladderhatchclose")
                        {
                            Effect.server.Run("assets/prefabs/building/floor.ladder.hatch/effects/door-ladder-hatch-close-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(0.8f, () => Effect.server.Run("assets/prefabs/building/floor.ladder.hatch/effects/door-ladder-hatch-close-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "wooddooropenlock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-open-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(3, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-open-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "wooddoorcloselock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-close-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(0.3f, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-wood-close-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "metaldooropenlock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-open-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(1, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-open-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "metaldoorcloselock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-close-start.prefab", player.transform.position, Vector3.zero);
                            timer.Once(0.5f, () => Effect.server.Run("assets/prefabs/building/door.hinged/effects/door-metal-close-end.prefab", player.transform.position, Vector3.zero));
                        }
                        else if (args[1] == "lockdenied")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.denied.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "lockshock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.shock.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "lockunlock")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "lockupdated")
                        {
                            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.updated.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "stonehatchtree")
                        {
                            Effect.server.Run("assets/prefabs/weapons/stone hatchet/effects/strike-muted.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/entities/tree/tree-impact.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "stonepickore")
                        {
                            Effect.server.Run("assets/prefabs/weapons/stone pickaxe/effects/strike.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/impacts/slash/metalore/slash_metalore_01.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "hatchettree")
                        {
                            Effect.server.Run("assets/prefabs/weapons/hatchet/effects/strike-soft.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/entities/tree/tree-impact.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "pickore")
                        {
                            Effect.server.Run("assets/prefabs/weapons/pickaxe/effects/strike.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/impacts/slash/metalore/slash_metalore_01.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "salvagedaxetree")
                        {
                            Effect.server.Run("assets/prefabs/weapons/salvaged_axe/effects/strike-muted.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/entities/tree/tree-impact.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "salvagedpickore")
                        {
                            Effect.server.Run("assets/prefabs/weapons/salvaged_icepick/effects/strike.prefab", player.transform.position, Vector3.zero);
                            Effect.server.Run("assets/bundled/prefabs/fx/impacts/slash/metalore/slash_metalore_01.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "murdererbreathing")
                        {
                            Effect.server.Run("assets/prefabs/npc/murderer/sound/breathing.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "landminetrigger")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/weapons/landmine/landmine_trigger.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "landmineblow")
                        {
                            Effect.server.Run("assets/bundled/prefabs/fx/weapons/landmine/landmine_explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "c4blow")
                        {
                            Effect.server.Run("assets/prefabs/tools/c4/effects/c4_explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "beancanblow")
                        {
                            Effect.server.Run("assets/prefabs/weapons/beancan grenade/effects/beancan_grenade_explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "f1blow")
                        {
                            Effect.server.Run("assets/prefabs/weapons/f1 grenade/effects/f1grenade_explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "guntrap")
                        {
                            Effect.server.Run("assets/prefabs/deployable/single shot trap/fired.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "rocketblow")
                        {
                            Effect.server.Run("assets/prefabs/weapons/rocketlauncher/effects/rocket_explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        else if (args[1] == "satchelblow")
                        {
                            Effect.server.Run("assets/prefabs/weapons/satchelcharge/effects/satchel-charge-explosion.prefab", player.transform.position, Vector3.zero);
                        }
                        break;

                    case "toggle":
                        if (IsAllowed(player.UserIDString, permAllow))
                        {
                            if (args[1] == "show")
                            {
                                BaitGui(player);
                                Reply(player, Lang("PanelRefresh"));
                            }
                            else if (args[1] == "hide")
                            {
                                CuiHelper.DestroyUi(player, "sbGUI");
                                Reply(player, Lang("PanelHidden"));
                            }
                        }
                        break;

                    default:
                        SendReply(player, $"[<color=#6275a4>SoundBaiter</color>]: Invalid Syntax /soundbait {args[0]} {args[1]}");
                        return;
                }
            }
            else { Reply(player, null); }
        }

        #region GUI
        private void BaitGui(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "sbGUI");
            var elements = new CuiElementContainer();
            var mainName = elements.Add(new CuiPanel
            {
                Image =
                {
                    Color = "0.20 0.50 0.80 0.5"
                },
                RectTransform =
                {
                    AnchorMin = "0.8 0.125",
                    AnchorMax = "0.995 0.99"
                },
                CursorEnabled = true
            }, "Hud", "sbGUI");

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait toggle hide",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("Title"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.0 0.97",
                    AnchorMax = "1.0 1.0"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action twig",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("TBuild"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.92",
                    AnchorMax = "0.48 0.96"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action thatch",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("TBreak"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.92",
                    AnchorMax = "0.97 0.96"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action wooddooropen",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("WDOpen"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.87",
                    AnchorMax = "0.48 0.91"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action wooddoorclose",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("WDClose"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.87",
                    AnchorMax = "0.97 0.91"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action metaldooropen",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("MDOpen"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.82",
                    AnchorMax = "0.48 0.86"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action metaldoorclose",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("MDClose"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.82",
                    AnchorMax = "0.97 0.86"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action wooddooropenlock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLWDOpen"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.77",
                    AnchorMax = "0.48 0.81"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action wooddoorcloselock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLWDClose"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.77",
                    AnchorMax = "0.97 0.81"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action metaldooropenlock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLMDOpen"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.72",
                    AnchorMax = "0.48 0.76"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action metaldoorcloselock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLMDClose"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.72",
                    AnchorMax = "0.97 0.76"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action ladderhatchopen",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("LHOpen"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.67",
                    AnchorMax = "0.48 0.71"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action ladderhatchclose",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("LHClose"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.67",
                    AnchorMax = "0.97 0.71"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action lockdenied",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLDenied"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.62",
                    AnchorMax = "0.48 0.66"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action lockshock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLShock"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.62",
                    AnchorMax = "0.97 0.66"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action lockunlock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLUnlock"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.57",
                    AnchorMax = "0.48 0.61"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action lockupdated",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("CLChanged"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.57",
                    AnchorMax = "0.97 0.61"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action stonehatchtree",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("SHTree"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.52",
                    AnchorMax = "0.48 0.56"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action stonepickore",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("SPOre"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.52",
                    AnchorMax = "0.97 0.56"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action hatchettree",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("MHTree"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.47",
                    AnchorMax = "0.48 0.51"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action pickore",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("MHOre"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.47",
                    AnchorMax = "0.97 0.51"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action salvagedaxetree",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("SATree"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.42",
                    AnchorMax = "0.48 0.46"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action salvagedpickore",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("SPOre"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.42",
                    AnchorMax = "0.97 0.46"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action landminetrigger",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("LMTrigger"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.37",
                    AnchorMax = "0.48 0.41"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action landmineblow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("LMBlow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.37",
                    AnchorMax = "0.97 0.41"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action guntrap",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("GTShoot"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.32",
                    AnchorMax = "0.48 0.36"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action rocketblow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("RBlow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.32",
                    AnchorMax = "0.97 0.36"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action c4blow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("C4Blow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.27",
                    AnchorMax = "0.48 0.31"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action satchelblow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("SBlow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.27",
                    AnchorMax = "0.97 0.31"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action beancanblow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BCBlow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.22",
                    AnchorMax = "0.48 0.26"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action f1blow",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("F1Blow"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.22",
                    AnchorMax = "0.97 0.26"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action beartrap",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BTSnap"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.17",
                    AnchorMax = "0.48 0.21"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action scream",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("DScream"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.17",
                    AnchorMax = "0.97 0.21"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action barrelhit",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BHit"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.12",
                    AnchorMax = "0.48 0.16"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action barrelbreak",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BBreak"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.12",
                    AnchorMax = "0.97 0.16"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action bulletmetal",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BMImpact"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.07",
                    AnchorMax = "0.48 0.11"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action bulletrock",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("BRImpact"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.07",
                    AnchorMax = "0.97 0.11"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action headshot",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("HShot"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.03 0.02",
                    AnchorMax = "0.48 0.06"
                }
            }, mainName);

            elements.Add(new CuiButton
            {
                Button =
                {
                    Command = "soundbait action murdererbreathing",
                    Color = "0.20 0.50 0.80 0.5"
                },
                Text =
                {
                    Text = Lang("Murderer"),
                    FontSize = 12,
                    Align = TextAnchor.MiddleCenter,
                    Color = "1 1 1 1"
                },
                RectTransform =
                {
                    AnchorMin = "0.52 0.02",
                    AnchorMax = "0.97 0.06"
                }
            }, mainName);

            CuiHelper.AddUi(player, elements);
        }
        #endregion GUI

        private bool IsAllowed(string id, string perm) => permission.UserHasPermission(id, perm);

        private string Lang(string key, string id = null, params object[] args) => string.Format(lang.GetMessage(key, this, id), args);

        private void Reply(BasePlayer player, string Getmessage)
        { rust.SendChatMessage(player, "[<color=#6275a4>SoundBaiter</color>]", Getmessage); }
    }
}
