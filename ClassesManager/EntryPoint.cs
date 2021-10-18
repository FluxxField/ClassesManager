﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using ModdingUtils.Extensions;
using TMPro;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnityEngine;
using Photon.Pun;
using UnboundLib.GameModes;
using UnboundLib.Networking;
using UnboundLib.Utils;

namespace ClassesManager {
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInDependency("pykess.rounds.plugins.moddingutils")]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch")]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class EntryPoint : BaseUnityPlugin {
        private const string ModId = "fluxxfield.rounds.plugins.classesmanager";
        private const string ModName = "Classes Manager";
        private const string Version = "1.0.1";

        private static ConfigEntry<bool> _useClassesFirstRoundConfig;
        private static bool _isRoundOne = true;

        private static List<CardInfo> ActiveCards {
            get {
                return ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList();
            }
        }

        private static List<CardInfo> InactiveCards {
            get {
                return (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            }
        }

        internal static List<CardInfo> AllCards {
            get {
                return ActiveCards.Concat(InactiveCards).ToList();
            }
        }


        private void Start() {
            _useClassesFirstRoundConfig =
                Config.Bind("Classes Manager", "Enabled", false, "Enable classes only first round");

            Unbound.Instance.ExecuteAfterSeconds(0.5f, BuildDefaultCategory);

            // Has to be at pick start since ModdingUtils has a hook that clears the players blacklistedCategories at game start
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, gm => HandlePlayersBlacklistedCategories());
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, gm => GameEndCleanup());

            Unbound.RegisterMenu(ModName, () => { }, NewGUI, null, false);
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
            Unbound.RegisterCredits(ModName,
                new[] {"FluxxField"},
                new[] {"github"},
                new[] {"https://github.com/FluxxField/ClassesManager"});
        }

        private static void BuildDefaultCategory() {
            UnityEngine.Debug.Log(AllCards.Count);
            foreach (var currentCard in AllCards) {
                UnityEngine.Debug.Log(currentCard.cardName);
                
                var currentCardsCategories = currentCard.categories.ToList();

                if (currentCardsCategories.Contains(ClassesManager.Instance.DefaultCardCategory) ||
                    currentCardsCategories.Contains(ClassesManager.Instance.ClassCategory)) {
                    continue;
                }

                currentCardsCategories.Add(ClassesManager.Instance.DefaultCardCategory);

                currentCard.categories = currentCardsCategories.ToArray();
            }
        }

        private static IEnumerator HandlePlayersBlacklistedCategories() {
            if (_isRoundOne) {
                var players = PlayerManager.instance.players.ToArray();

                foreach (var player in players) {
                    // Blacklist default cards if enabled by settings
                    if (_useClassesFirstRoundConfig.Value) {
                        CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories
                            .Add(ClassesManager.Instance.DefaultCardCategory);
                    }

                    // blacklist all upgrade categories
                    CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories
                        .AddRange(ClassesManager.Instance.ClassUpgradeCategories.Values.ToList());
                }

                _isRoundOne = false;
            }

            yield break;
        }

        private static IEnumerator GameEndCleanup() {
            _isRoundOne = true;
            yield break;
        }

        private static void NewGUI(
            GameObject menu
        ) {
            MenuHandler.CreateText($"{ModName} Options", menu, out TextMeshProUGUI _);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateToggle(_useClassesFirstRoundConfig.Value, "Enable Force classes first round", menu,
                value => {
                    _useClassesFirstRoundConfig.Value = value;
                    OnHandShakeCompleted();
                });
        }

        private static void OnHandShakeCompleted() {
            if (PhotonNetwork.IsMasterClient) {
                NetworkingManager.RPC_Others(typeof(EntryPoint), nameof(SyncSettings),
                    new object[] {_useClassesFirstRoundConfig.Value});
            }
        }

        [UnboundRPC]
        private static void SyncSettings(
            bool hostUseClassesStart
        ) {
            _useClassesFirstRoundConfig.Value = hostUseClassesStart;
        }
    }
}