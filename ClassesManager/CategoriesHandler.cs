using System.Collections.Generic;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace ClassesManager {
    public class CategoriesHandler {
        private static CategoriesHandler _instance;

        private CategoriesHandler() {
        }

        public static CategoriesHandler Instance {
            get {
                _instance = _instance ?? new CategoriesHandler();
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
    }
}