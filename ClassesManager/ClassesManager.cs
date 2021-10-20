using System.Collections.Generic;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils.Extensions;

namespace ClassesManager {
    public class ClassesManager {
        private static ClassesManager _instance;

        private ClassesManager() {
        }

        public static ClassesManager Instance {
            get {
                _instance = _instance ?? new ClassesManager();
                return _instance;
            }
        }

        private Dictionary<string, CardCategory> _classUpgradeCategories;
        
        public  Dictionary<string, CardCategory> ClassUpgradeCategories {
            get {
                _classUpgradeCategories = _classUpgradeCategories ?? new Dictionary<string, CardCategory>();
                return _classUpgradeCategories;
            }
        }

        private CardCategory _defaultCardCategory;

        public CardCategory DefaultCardCategory {
            get {
                _defaultCardCategory = _defaultCardCategory ?? CustomCardCategories.instance.CardCategory("default");
                return _defaultCardCategory;
            }
        }
        
        private CardCategory _classCategory;

        public CardCategory ClassCategory {
            get {
                _classCategory = _classCategory ?? CustomCardCategories.instance.CardCategory("class");
                return _classCategory;
            }
        }
        
        public void AddClassUpgradeCategory(
            string categoryName
        ) {
            ClassUpgradeCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
        }

        public void RemoveDefaultCardCategoryFromPlayer(CharacterStatModifiers characterStats) {
            characterStats.GetAdditionalData().blacklistedCategories.Remove(DefaultCardCategory);
        }

        public void AddClassCategoryToPlayersBlacklist(
            CharacterStatModifiers characterStats
        ) {
            characterStats.GetAdditionalData().blacklistedCategories.Add(ClassCategory);
        }
    }
}