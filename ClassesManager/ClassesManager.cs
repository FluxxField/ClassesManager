using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
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
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class ClassesManager : BaseUnityPlugin {
        private const string ModId = "fluxxfield.rounds.plugins.classesmanager";
        private const string ModName = "Classes Manager";
        private const string Version = "1.0.1";

        private static ConfigEntry<bool> _useClassesFirstRoundConfig;
        private static bool _useClassesFirstRound;

        public static CardCategory DefaultCardCategory;
        public static CardCategory ClassCategory;
        public static Dictionary<string, CardCategory> ClassUpgradeCategories = new Dictionary<string, CardCategory>();

        private void Awake() {
            _useClassesFirstRoundConfig =
                Config.Bind("Classes Manager", "Enabled", false, "Enable classes only first round");
        }

        private void Start() {
            _useClassesFirstRound = _useClassesFirstRoundConfig.Value;

            if (CustomCardCategories.instance != null) {
                DefaultCardCategory = CustomCardCategories.instance.CardCategory("defaultCard");
                ClassCategory = CustomCardCategories.instance.CardCategory("class");
            }

            this.ExecuteAfterSeconds(0.4f, HandleBuildDefaultCategory);

            GameModeManager.AddHook(GameModeHooks.HookGameStart, gm => HandlePlayersBlacklistedCategories());

            Unbound.RegisterMenu(ModName, () => { }, NewGUI, null, false);
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
            Unbound.RegisterCredits(ModName,
                new[] {"FluxxField"},
                new[] {"github"},
                new[] {"https://github.com/FluxxField/ClassesManager"});
        }

        public static void AddClassUpgradeCategory(
            string categoryName
        ) {
            ClassUpgradeCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
        }

        private static void HandleBuildDefaultCategory() {
            foreach (Card currentCard in CardManager.cards.Values.ToList()) {
                List<CardCategory> currentCategories = currentCard.cardInfo.categories.ToList();

                if (currentCategories.Contains(DefaultCardCategory)) {
                    return;
                }

                currentCategories.Add(DefaultCardCategory);
                currentCard.cardInfo.categories = currentCategories.ToArray();
            }
        }

        private static IEnumerator HandlePlayersBlacklistedCategories() {
            var players = PlayerManager.instance.players.ToArray();

            foreach (var player in players) {
                var blacklistCategories = CharacterStatModifiersExtension.GetAdditionalData(player.data.stats)
                    .blacklistedCategories;

                // Blacklist default cards if enabled by settings
                if (_useClassesFirstRound) {
                    blacklistCategories.Add(DefaultCardCategory);
                }

                // blacklist all upgrade categories
                blacklistCategories.AddRange(ClassUpgradeCategories.Values.ToList());
            }

            yield break;
        }

        private void NewGUI(
            GameObject menu
        ) {
            MenuHandler.CreateText($"{ModName} Options", menu, out TextMeshProUGUI _);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateToggle(false, "Enable Force classes first round", menu, useClassesFirstRound => {
                _useClassesFirstRound = useClassesFirstRound;
                OnHandShakeCompleted();
            });
        }

        private void OnHandShakeCompleted() {
            if (PhotonNetwork.IsMasterClient) {
                NetworkingManager.RPC_Others(typeof(ClassesManager), nameof(SyncSettings),
                    new object[] {_useClassesFirstRound});
            }
        }

        [UnboundRPC]
        private static void SyncSettings(
            bool hostUseClassesStart
        ) {
            _useClassesFirstRound = hostUseClassesStart;
        }
    }
}