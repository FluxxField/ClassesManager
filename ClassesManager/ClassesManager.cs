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

        private Dictionary<string, CardCategory> _classProgressionCategories;
        
        public  Dictionary<string, CardCategory> ClassProgressionCategories {
            get {
                _classProgressionCategories = _classProgressionCategories ?? new Dictionary<string, CardCategory>();
                return _classProgressionCategories;
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
        
        public void AddClassProgressionCategories(
            List<string> categoryNames
        ) {
            if (categoryNames.Count == 0) {
                return;
            }

            foreach (var categoryName in categoryNames) {
                ClassProgressionCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
            }
        }

        public void RemoveDefaultCardCategoryFromPlayer(CharacterStatModifiers characterStats) {
            characterStats.GetAdditionalData().blacklistedCategories.Remove(DefaultCardCategory);
        }
        
        public void RemoveProgressionCategoriesFromPlayer(
            CharacterStatModifiers characterStats,
            List<string> categoryNames
        ) {
            if (categoryNames.Count == 0) {
                return;
            }

            var categoriesToRemove = new List<CardCategory>();

            foreach (var progressionCategoryName in categoryNames) {
                categoriesToRemove.Add(ClassProgressionCategories[progressionCategoryName]);
            }

            foreach (var category in categoriesToRemove) {
                characterStats.GetAdditionalData().blacklistedCategories.Remove(category);
            }
        }

        public void AddClassCategoryToPlayer(
            CharacterStatModifiers characterStats
        ) {
            if (EntryPoint.AllowMultiClassesConfig.Value) {
                return;
            }
            
            characterStats.GetAdditionalData().blacklistedCategories.Add(ClassCategory);
        }

        public void OnClassCardSelect(CharacterStatModifiers characterStats, List<string> progressionCategoryNames) {
            RemoveDefaultCardCategoryFromPlayer(characterStats);
            AddClassCategoryToPlayer(characterStats);
            RemoveProgressionCategoriesFromPlayer(characterStats, progressionCategoryNames);
        }
    }
}