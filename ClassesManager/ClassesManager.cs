using System.Collections;
using System.Linq;
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
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class ClassesManager : BaseUnityPlugin {
        private const string ModId = "fluxxfield.rounds.plugins.classesmanager";
        private const string ModName = "Classes Manager";
        private const string Version = "1.0.1";

        private static ConfigEntry<bool> _useClassesFirstRoundConfig;
        private static bool _useClassesFirstRound;
        
        private void Awake() {
            _useClassesFirstRoundConfig =
                Config.Bind("Classes Manager", "Enabled", false, "Enable classes only first round");
        }

        private void Start() {
            _useClassesFirstRound = _useClassesFirstRoundConfig.Value;

            this.ExecuteAfterSeconds(0.4f, BuildDefaultCategory);

            GameModeManager.AddHook(GameModeHooks.HookGameStart, gm => HandlePlayersBlacklistedCategories());

            Unbound.RegisterMenu(ModName, () => { }, NewGUI, null, false);
            Unbound.RegisterHandshake(ModId, OnHandShakeCompleted);
            Unbound.RegisterCredits(ModName,
                new[] {"FluxxField"},
                new[] {"github"},
                new[] {"https://github.com/FluxxField/ClassesManager"});
        }

        private static void BuildDefaultCategory() {
            foreach (var currentCard in CardManager.cards.Values.ToList()) {
                var currentCardsCategories = currentCard.cardInfo.categories.ToList();
                
                if (currentCardsCategories.Contains(CategoriesHandler.Instance.DefaultCardCategory)) {
                    return;
                }

                currentCardsCategories.Add(CategoriesHandler.Instance.DefaultCardCategory);

                currentCard.cardInfo.categories = currentCardsCategories.ToArray();
            }
        }

        private static IEnumerator HandlePlayersBlacklistedCategories() {
            var players = PlayerManager.instance.players.ToArray();

            foreach (var player in players) {
                var blacklistCategories = CharacterStatModifiersExtension.GetAdditionalData(player.data.stats)
                    .blacklistedCategories;

                // Blacklist default cards if enabled by settings
                if (_useClassesFirstRound) {
                    blacklistCategories.Add(CategoriesHandler.Instance.DefaultCardCategory);
                }

                // blacklist all upgrade categories
                blacklistCategories.AddRange(CategoriesHandler.Instance.ClassUpgradeCategories.Values.ToList());
            }

            yield break;
        }

        private static void NewGUI(
            GameObject menu
        ) {
            MenuHandler.CreateText($"{ModName} Options", menu, out TextMeshProUGUI _);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateToggle(false, "Enable Force classes first round", menu, value => {
                _useClassesFirstRound = value;
                OnHandShakeCompleted();
            });
        }

        private static void OnHandShakeCompleted() {
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