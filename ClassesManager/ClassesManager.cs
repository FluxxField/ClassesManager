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
        
        public static Dictionary<string, CardCategory> ClassCategories;
        public static Dictionary<string, CardCategory> ClassUpgradeCategories;

        private void Awake() {
            _useClassesFirstRoundConfig =
                Config.Bind("Classes Manager", "Enabled", false, "Enable classes only first round");
        }

        private void Start() {
            _useClassesFirstRound = _useClassesFirstRoundConfig.Value;
            
            if (CustomCardCategories.instance != null) {
                DefaultCardCategory = CustomCardCategories.instance.CardCategory("defaultCard");
            }
            
            this.ExecuteAfterSeconds(0.4f, HandleBuildDefaultCategory);

            GameModeManager.AddHook(GameModeHooks.HookGameStart, gm => ForceClassesFirstRound());

            Unbound.RegisterMenu(ModName, () => { }, NewGUI, null, false);
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
            Unbound.RegisterCredits(ModName,
                new[] {"FluxxField"},
                new[] {"github"},
                new[] {"https://github.com/FluxxField/ClassesManager"});
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

        private IEnumerator ForceClassesFirstRound() {
            if (!_useClassesFirstRound) {
                yield break;
            }

            Player[] players = PlayerManager.instance.players.ToArray();

            foreach (Player player in players) {
                // Blacklist Default Cards and all Upgrade Cards for all players
                CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.AddRange(
                    new[] {
                        DefaultCardCategory
                    }
                );
            }
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