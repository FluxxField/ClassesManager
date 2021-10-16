using System.Collections.Generic;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace ClassesManager {
    public class CategoriesHandler {
        private static CategoriesHandler _instance;

        private CategoriesHandler() {
        }

        public static CategoriesHandler Instance {
            get => _instance ?? new CategoriesHandler();
        }

        private Dictionary<string, CardCategory> _classUpgradeCategories;
        
        public  Dictionary<string, CardCategory> ClassUpgradeCategories {
            get => _classUpgradeCategories ?? new Dictionary<string, CardCategory>();
        }

        private CardCategory _defaultCardCategory;

        public CardCategory DefaultCardCategory {
            get => _defaultCardCategory ?? CustomCardCategories.instance.CardCategory("default");
        }
        
        private CardCategory _classCategory;

        public CardCategory ClassCategory {
            get => _classCategory ?? CustomCardCategories.instance.CardCategory("class");
        }
        
        public void AddClassUpgradeCategory(
            string categoryName
        ) {
            ClassUpgradeCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
        }
    }
}