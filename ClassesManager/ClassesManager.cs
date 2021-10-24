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
                _defaultCardCategory = _defaultCardCategory ?? CustomCardCategories.instance.CardCategory("Default");
                return _defaultCardCategory;
            }
        }
        
        private CardCategory _classCategory;

        public CardCategory ClassCategory {
            get {
                _classCategory = _classCategory ?? CustomCardCategories.instance.CardCategory("Class");
                return _classCategory;
            }
        }
        
        public void AddClassUpgradeCategories(
            List<string> categoryNames
        ) {
            if (categoryNames.Count == 0) {
                return;
            }

            foreach (var categoryName in categoryNames) {
                ClassUpgradeCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
            }
        }

        public void RemoveDefaultCardCategoryFromPlayer(CharacterStatModifiers characterStats) {
            characterStats.GetAdditionalData().blacklistedCategories.Remove(DefaultCardCategory);
        }
        
        public void RemoveUpgradeCategoriesFromPlayer(
            CharacterStatModifiers characterStats,
            List<string> upgradeCategoryNames
        ) {
            if (upgradeCategoryNames.Count == 0) {
                return;
            }

            var categoriesToRemove = new List<CardCategory>();

            foreach (var upgradeCategoryName in upgradeCategoryNames) {
                categoriesToRemove.Add(ClassUpgradeCategories[upgradeCategoryName]);
            }

            foreach (var category in categoriesToRemove) {
                characterStats.GetAdditionalData().blacklistedCategories.Remove(category);
            }
        }

        public void AddClassCategoryToPlayersBlacklist(
            CharacterStatModifiers characterStats
        ) {
            characterStats.GetAdditionalData().blacklistedCategories.Add(ClassCategory);
        }

        public void OnClassCardSelect(CharacterStatModifiers characterStats, List<string> upgradeCategoryNames) {
            RemoveDefaultCardCategoryFromPlayer(characterStats);
            AddClassCategoryToPlayersBlacklist(characterStats);
            RemoveUpgradeCategoriesFromPlayer(characterStats, upgradeCategoryNames);
        }
    }
}