# ClassesManager

This simple mod is used to centralize the categories used for classes. This allows modders to use existing categories and add their own.

## Settings

### Force Classes First Round

Is used to force classes only cards first round

### Allow Multi Classes

Is used to allow for more than one class card per game

## Public Properties

### DefaultCardCategory

Type: CardCategory

Used to mark cards not apart of classes as default cards

### ClassCategory

Type: CardCategory

Used to mark cards as class cards. NOTE: don't mark class progression cards with this category (Ex: Marksman, Light Gunner, etc..)

### ClassProgressionCategories

Type: Dictionary<strgin, CardCategory>

This is where all of the progression categories are stored.

## Public Methods

### AddClassProgressionCategories

Parameters: List<string> categoryNames
  
Used to add progression categories to ClassProgressionCategories

### RemoveDefaultCardCategoryFromPlayer

Parameters: CharacterStatModifiers characterstats
  
Removes the default card category from the provided players blacklisted categories

### RemoveProgressionCategoriesFromPlayer

Parameters: CharacterStatModifiers characterstats, List<string> categoryNames
  
Removes the passed in category names from the provided players blacklisted categories

### AddClassCategoryToPlayer

Parameters: CharacterStatModifiers characterstats
  
Removes the class category ONLY IF the setting for allowing multiple classes is disabled

### OnClassCardSelect

Parameters: CharacterStatModifiers characterStats, List<string> progressionCategoryNames
  
Calls RemoveDefaultCardCategoryFromPlayer, AddClassCategoryToPlayer, and RemoveProgressionCategoriesFromPlayer
