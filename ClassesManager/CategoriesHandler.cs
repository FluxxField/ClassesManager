using System.Collections.Generic;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace ClassesManager {
    public class CategoriesHandler {
        private static CategoriesHandler _instance;

        private CategoriesHandler() {
        }

        public static CategoriesHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new CategoriesHandler();
                }

                return _instance;
            }
        }

        private static Dictionary<string, CardCategory> _classUpgradeCategories;
        
        public static  Dictionary<string, CardCategory> ClassUpgradeCategories {
            get {
                if (_classUpgradeCategories == null) {
                    _classUpgradeCategories = new Dictionary<string, CardCategory>();
                }

                return _classUpgradeCategories;
            }
        }

        private static CardCategory _defaultCardCategory;

        public static CardCategory DefaultCardCategory {
            get {
                if (_defaultCardCategory == null) {
                    _defaultCardCategory = CustomCardCategories.instance.CardCategory("default");
                }

                return _defaultCardCategory;
            }
        }
        
        private static CardCategory _classCategory;

        public static CardCategory ClassCategory {
            get {
                if (_classCategory == null) {
                    _classCategory = CustomCardCategories.instance.CardCategory("class");
                }

                return _classCategory;
            }
        }
        
        public static void AddClassUpgradeCategory(
            string categoryName
        ) {
            ClassUpgradeCategories.Add(categoryName, CustomCardCategories.instance.CardCategory(categoryName));
        }
    }
}