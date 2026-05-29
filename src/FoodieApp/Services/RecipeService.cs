namespace FoodieApp.Services;

public class RecipeService : IRecipeService
{
    private List<Recipe> _recipes;
    private List<FoodCategory> _categories;
    private readonly Random _random = new();

    public RecipeService()
    {
        _recipes = new List<Recipe>();
        _categories = new List<FoodCategory>();
        InitializeData();
    }

    public Task<List<Recipe>> GetAllRecipesAsync()
    {
        try
        {
            return Task.FromResult(_recipes.ToList());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting recipes: {ex.Message}");
            return Task.FromResult(new List<Recipe>());
        }
    }

    public Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Task.FromResult(new List<Recipe>());
        }

        try
        {
            var result = _recipes
                .Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error filtering by category: {ex.Message}");
            return Task.FromResult(new List<Recipe>());
        }
    }

    public Task<Recipe?> GetRecipeByIdAsync(int id)
    {
        try
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(recipe);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting recipe {id}: {ex.Message}");
            return Task.FromResult<Recipe?>(null);
        }
    }

    public Task<List<Recipe>> SearchRecipesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(new List<Recipe>());
        }

        try
        {
            var result = _recipes
                .Where(r =>
                    r.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    r.Ingredients.Any(i => i.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    r.DietaryTags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching recipes: {ex.Message}");
            return Task.FromResult(new List<Recipe>());
        }
    }

    public Task<List<Recipe>> GetFavouriteRecipesAsync()
    {
        try
        {
            var favourites = _recipes.Where(r => r.IsFavourite).ToList();
            return Task.FromResult(favourites);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting favourites: {ex.Message}");
            return Task.FromResult(new List<Recipe>());
        }
    }

    public Task<Recipe> GetRandomRecipeAsync()
    {
        try
        {
            int index = _random.Next(0, _recipes.Count);
            return Task.FromResult(_recipes[index]);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting random recipe: {ex.Message}");
            return Task.FromResult(new Recipe { Name = "Unknown Recipe" });
        }
    }

    public Task<List<FoodCategory>> GetCategoriesAsync()
    {
        try
        {
            return Task.FromResult(_categories.ToList());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting categories: {ex.Message}");
            return Task.FromResult(new List<FoodCategory>());
        }
    }

    public Task ToggleFavouriteAsync(int recipeId)
    {
        try
        {
            var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe != null)
            {
                recipe.IsFavourite = !recipe.IsFavourite;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling favourite: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    private void InitializeData()
    {
        _categories = new List<FoodCategory>
        {
            new() { Name = "Breakfast", Icon = "🍳", Description = "Start your day right", RecipeCount = 4 },
            new() { Name = "Lunch", Icon = "🥗", Description = "Midday meals", RecipeCount = 4 },
            new() { Name = "Dinner", Icon = "🍝", Description = "Evening feasts", RecipeCount = 4 },
            new() { Name = "Desserts", Icon = "🍰", Description = "Sweet treats", RecipeCount = 3 },
            new() { Name = "Beverages", Icon = "🍹", Description = "Drinks and smoothies", RecipeCount = 3 },
            new() { Name = "Snacks", Icon = "🥨", Description = "Quick bites", RecipeCount = 3 }
        };

        _recipes = new List<Recipe>
        {
            new()
            {
                Id = 1, Name = "Classic Pancakes", Category = "Breakfast",
                Description = "Fluffy golden pancakes perfect for a weekend breakfast. Light, airy, and delicious with maple syrup and fresh berries.",
                ImageUrl = "pancakes.png", PrepTimeMinutes = 10, CookTimeMinutes = 15, Servings = 4,
                Difficulty = "Easy", Rating = 4.5,
                Ingredients = new List<string> { "1½ cups all-purpose flour", "3½ tsp baking powder", "1 tbsp sugar", "¼ tsp salt", "1¼ cups milk", "1 egg", "3 tbsp melted butter" },
                Instructions = new List<string> { "Sift flour, baking powder, sugar, and salt together.", "Make a well and pour in milk, egg, and melted butter.", "Mix until smooth.", "Heat a griddle and pour ¼ cup batter for each pancake.", "Cook until bubbles form, then flip.", "Serve with maple syrup and fresh berries." },
                Nutrition = new NutritionInfo { Calories = 350, ProteinGrams = 8, CarbohydratesGrams = 45, FatGrams = 15, FiberGrams = 2, SugarGrams = 10, SodiumMilligrams = 400 },
                DietaryTags = new List<string> { "Vegetarian" }
            },
            new()
            {
                Id = 2, Name = "Avocado Toast", Category = "Breakfast",
                Description = "Simple yet satisfying smashed avocado on sourdough with a perfectly poached egg.",
                ImageUrl = "avocado_toast.png", PrepTimeMinutes = 5, CookTimeMinutes = 10, Servings = 2,
                Difficulty = "Easy", Rating = 4.2,
                Ingredients = new List<string> { "2 slices sourdough bread", "1 ripe avocado", "2 eggs", "Salt and pepper", "Red pepper flakes", "Lemon juice" },
                Instructions = new List<string> { "Toast the sourdough bread.", "Mash the avocado with salt, pepper, and lemon juice.", "Poach the eggs for 3-4 minutes.", "Spread avocado on toast and top with poached egg.", "Sprinkle red pepper flakes." },
                Nutrition = new NutritionInfo { Calories = 320, ProteinGrams = 12, CarbohydratesGrams = 28, FatGrams = 18, FiberGrams = 7, SugarGrams = 2, SodiumMilligrams = 350 },
                DietaryTags = new List<string> { "Vegetarian", "High Fiber" }
            },
            new()
            {
                Id = 3, Name = "Chicken Caesar Salad", Category = "Lunch",
                Description = "Crisp romaine lettuce with grilled chicken, parmesan, and homemade Caesar dressing.",
                ImageUrl = "caesar_salad.png", PrepTimeMinutes = 15, CookTimeMinutes = 15, Servings = 2,
                Difficulty = "Easy", Rating = 4.6,
                Ingredients = new List<string> { "2 chicken breasts", "1 head romaine lettuce", "½ cup parmesan cheese", "Croutons", "Caesar dressing", "Lemon", "Olive oil" },
                Instructions = new List<string> { "Season and grill chicken breasts for 6-7 minutes each side.", "Chop romaine lettuce.", "Slice grilled chicken.", "Toss lettuce with dressing, add chicken, parmesan, and croutons.", "Finish with a squeeze of lemon." },
                Nutrition = new NutritionInfo { Calories = 450, ProteinGrams = 35, CarbohydratesGrams = 15, FatGrams = 28, FiberGrams = 3, SugarGrams = 3, SodiumMilligrams = 750 },
                DietaryTags = new List<string> { "High Protein", "Gluten-Free Option" }
            },
            new()
            {
                Id = 4, Name = "Spaghetti Bolognese", Category = "Dinner",
                Description = "Rich, hearty Italian meat sauce served over al dente spaghetti. A family favourite.",
                ImageUrl = "spaghetti.png", PrepTimeMinutes = 15, CookTimeMinutes = 45, Servings = 4,
                Difficulty = "Medium", Rating = 4.8,
                Ingredients = new List<string> { "500g spaghetti", "500g ground beef", "1 onion", "3 garlic cloves", "2 cans crushed tomatoes", "2 tbsp tomato paste", "Italian herbs", "Parmesan cheese" },
                Instructions = new List<string> { "Sauté diced onion and garlic in olive oil.", "Add ground beef and brown.", "Stir in tomato paste, crushed tomatoes, and herbs.", "Simmer for 30 minutes.", "Cook spaghetti al dente.", "Serve sauce over pasta with grated parmesan." },
                Nutrition = new NutritionInfo { Calories = 650, ProteinGrams = 38, CarbohydratesGrams = 72, FatGrams = 22, FiberGrams = 5, SugarGrams = 8, SodiumMilligrams = 580 },
                DietaryTags = new List<string> { "High Protein" }
            },
            new()
            {
                Id = 5, Name = "Chocolate Lava Cake", Category = "Desserts",
                Description = "Decadent individual chocolate cakes with a warm, molten center. Restaurant quality at home.",
                ImageUrl = "lava_cake.png", PrepTimeMinutes = 15, CookTimeMinutes = 14, Servings = 4,
                Difficulty = "Hard", Rating = 4.9,
                Ingredients = new List<string> { "200g dark chocolate", "½ cup butter", "2 eggs", "2 egg yolks", "¼ cup sugar", "2 tbsp flour" },
                Instructions = new List<string> { "Melt chocolate and butter together.", "Whisk eggs, yolks, and sugar until thick.", "Fold chocolate mixture into eggs.", "Sift in flour and fold gently.", "Pour into greased ramekins.", "Bake at 200°C for 12-14 minutes.", "Invert and serve immediately." },
                Nutrition = new NutritionInfo { Calories = 480, ProteinGrams = 8, CarbohydratesGrams = 42, FatGrams = 32, FiberGrams = 2, SugarGrams = 30, SodiumMilligrams = 120 },
                DietaryTags = new List<string> { "Vegetarian" }
            },
            new()
            {
                Id = 6, Name = "Mango Smoothie Bowl", Category = "Breakfast",
                Description = "Tropical mango smoothie bowl topped with granola, coconut, and fresh fruit.",
                ImageUrl = "smoothie_bowl.png", PrepTimeMinutes = 10, CookTimeMinutes = 0, Servings = 1,
                Difficulty = "Easy", Rating = 4.3,
                Ingredients = new List<string> { "2 frozen mangoes", "1 banana", "½ cup coconut milk", "Granola", "Fresh berries", "Chia seeds" },
                Instructions = new List<string> { "Blend mango, banana, and coconut milk until thick and smooth.", "Pour into a bowl.", "Top with granola, berries, and chia seeds." },
                Nutrition = new NutritionInfo { Calories = 380, ProteinGrams = 6, CarbohydratesGrams = 72, FatGrams = 10, FiberGrams = 8, SugarGrams = 48, SodiumMilligrams = 80 },
                DietaryTags = new List<string> { "Vegan", "Gluten-Free", "High Fiber" }
            },
            new()
            {
                Id = 7, Name = "Thai Green Curry", Category = "Dinner",
                Description = "Aromatic Thai green curry with tender chicken, bamboo shoots, and coconut milk.",
                ImageUrl = "green_curry.png", PrepTimeMinutes = 20, CookTimeMinutes = 25, Servings = 4,
                Difficulty = "Medium", Rating = 4.5,
                Ingredients = new List<string> { "500g chicken thigh", "2 tbsp green curry paste", "400ml coconut milk", "1 cup bamboo shoots", "Thai basil", "Fish sauce", "Brown sugar", "Jasmine rice" },
                Instructions = new List<string> { "Fry curry paste in a little coconut milk until fragrant.", "Add chicken and cook until sealed.", "Pour in remaining coconut milk and simmer 20 minutes.", "Add bamboo shoots, fish sauce, and sugar.", "Garnish with Thai basil.", "Serve with steamed jasmine rice." },
                Nutrition = new NutritionInfo { Calories = 550, ProteinGrams = 32, CarbohydratesGrams = 48, FatGrams = 26, FiberGrams = 3, SugarGrams = 6, SodiumMilligrams = 850 },
                DietaryTags = new List<string> { "Gluten-Free", "High Protein" }
            },
            new()
            {
                Id = 8, Name = "Berry Smoothie", Category = "Beverages",
                Description = "Refreshing antioxidant-rich berry smoothie with yogurt and honey.",
                ImageUrl = "berry_smoothie.png", PrepTimeMinutes = 5, CookTimeMinutes = 0, Servings = 1,
                Difficulty = "Easy", Rating = 4.0,
                Ingredients = new List<string> { "1 cup mixed berries", "½ cup Greek yogurt", "1 tbsp honey", "½ cup milk", "Ice cubes" },
                Instructions = new List<string> { "Add all ingredients to blender.", "Blend until smooth.", "Pour into glass and serve." },
                Nutrition = new NutritionInfo { Calories = 250, ProteinGrams = 12, CarbohydratesGrams = 35, FatGrams = 5, FiberGrams = 5, SugarGrams = 25, SodiumMilligrams = 100 },
                DietaryTags = new List<string> { "Vegetarian", "Gluten-Free" }
            },
            new()
            {
                Id = 9, Name = "Hummus with Pita", Category = "Snacks",
                Description = "Creamy homemade hummus served with warm pita bread and vegetable sticks.",
                ImageUrl = "hummus.png", PrepTimeMinutes = 10, CookTimeMinutes = 0, Servings = 4,
                Difficulty = "Easy", Rating = 4.1,
                Ingredients = new List<string> { "400g chickpeas", "3 tbsp tahini", "2 garlic cloves", "Lemon juice", "Olive oil", "Paprika", "Pita bread" },
                Instructions = new List<string> { "Drain and rinse chickpeas.", "Blend chickpeas, tahini, garlic, and lemon juice.", "Add olive oil gradually while blending.", "Season with salt and paprika.", "Serve with warm pita and veggie sticks." },
                Nutrition = new NutritionInfo { Calories = 180, ProteinGrams = 7, CarbohydratesGrams = 22, FatGrams = 8, FiberGrams = 5, SugarGrams = 1, SodiumMilligrams = 300 },
                DietaryTags = new List<string> { "Vegan", "High Fiber" }
            },
            new()
            {
                Id = 10, Name = "Grilled Salmon", Category = "Dinner",
                Description = "Perfectly grilled Atlantic salmon with lemon butter sauce and asparagus.",
                ImageUrl = "salmon.png", PrepTimeMinutes = 10, CookTimeMinutes = 15, Servings = 2,
                Difficulty = "Medium", Rating = 4.7,
                Ingredients = new List<string> { "2 salmon fillets", "Lemon", "Butter", "Garlic", "Asparagus", "Olive oil", "Dill" },
                Instructions = new List<string> { "Season salmon with salt, pepper, and dill.", "Grill salmon skin-side down for 5 minutes.", "Flip and cook 3-4 more minutes.", "Sauté asparagus in olive oil and garlic.", "Top salmon with lemon butter sauce." },
                Nutrition = new NutritionInfo { Calories = 420, ProteinGrams = 40, CarbohydratesGrams = 5, FatGrams = 26, FiberGrams = 2, SugarGrams = 1, SodiumMilligrams = 200 },
                DietaryTags = new List<string> { "Gluten-Free", "High Protein", "Low Carb" }
            },
            new()
            {
                Id = 11, Name = "Tiramisu", Category = "Desserts",
                Description = "Classic Italian coffee-flavoured layered dessert with mascarpone cream and cocoa.",
                ImageUrl = "tiramisu.png", PrepTimeMinutes = 30, CookTimeMinutes = 0, Servings = 8,
                Difficulty = "Medium", Rating = 4.8,
                Ingredients = new List<string> { "500g mascarpone", "4 eggs", "100g sugar", "300ml strong espresso", "300g ladyfinger biscuits", "Cocoa powder", "Dark chocolate shavings" },
                Instructions = new List<string> { "Separate eggs. Beat yolks with sugar until pale.", "Fold in mascarpone.", "Whip egg whites to stiff peaks and fold in.", "Dip ladyfingers in cooled espresso.", "Layer biscuits and cream.", "Refrigerate 4+ hours.", "Dust with cocoa before serving." },
                Nutrition = new NutritionInfo { Calories = 350, ProteinGrams = 8, CarbohydratesGrams = 30, FatGrams = 22, FiberGrams = 1, SugarGrams = 18, SodiumMilligrams = 150 },
                DietaryTags = new List<string> { "Vegetarian" }
            },
            new()
            {
                Id = 12, Name = "Matcha Latte", Category = "Beverages",
                Description = "Vibrant green tea latte with ceremonial grade matcha and steamed oat milk.",
                ImageUrl = "matcha_latte.png", PrepTimeMinutes = 5, CookTimeMinutes = 5, Servings = 1,
                Difficulty = "Easy", Rating = 4.2,
                Ingredients = new List<string> { "2 tsp matcha powder", "2 tbsp hot water", "1 cup oat milk", "1 tsp honey (optional)" },
                Instructions = new List<string> { "Sift matcha powder into a bowl.", "Add hot water and whisk until smooth.", "Steam oat milk until frothy.", "Pour matcha into a cup and add steamed milk.", "Sweeten with honey if desired." },
                Nutrition = new NutritionInfo { Calories = 120, ProteinGrams = 3, CarbohydratesGrams = 18, FatGrams = 3, FiberGrams = 1, SugarGrams = 12, SodiumMilligrams = 90 },
                DietaryTags = new List<string> { "Vegan", "Gluten-Free" }
            },
            new()
            {
                Id = 13, Name = "Vegetable Stir Fry", Category = "Lunch",
                Description = "Quick and healthy mixed vegetable stir fry with tofu and soy ginger sauce.",
                ImageUrl = "stir_fry.png", PrepTimeMinutes = 15, CookTimeMinutes = 10, Servings = 2,
                Difficulty = "Easy", Rating = 4.3,
                Ingredients = new List<string> { "200g firm tofu", "Broccoli", "Bell peppers", "Carrots", "Snow peas", "Soy sauce", "Ginger", "Garlic", "Sesame oil" },
                Instructions = new List<string> { "Press and cube tofu.", "Stir fry tofu until golden.", "Add garlic and ginger.", "Add vegetables and stir fry 3-4 minutes.", "Add soy sauce and sesame oil.", "Serve over steamed rice." },
                Nutrition = new NutritionInfo { Calories = 320, ProteinGrams = 16, CarbohydratesGrams = 25, FatGrams = 14, FiberGrams = 6, SugarGrams = 8, SodiumMilligrams = 600 },
                DietaryTags = new List<string> { "Vegan", "High Fiber" }
            },
            new()
            {
                Id = 14, Name = "Iced Caramel Macchiato", Category = "Beverages",
                Description = "Coffee shop-style iced caramel macchiato with vanilla syrup and caramel drizzle.",
                ImageUrl = "caramel_macchiato.png", PrepTimeMinutes = 5, CookTimeMinutes = 5, Servings = 1,
                Difficulty = "Easy", Rating = 4.4,
                Ingredients = new List<string> { "2 shots espresso", "1 cup milk", "1 tbsp vanilla syrup", "Caramel sauce", "Ice" },
                Instructions = new List<string> { "Fill glass with ice.", "Add vanilla syrup and milk.", "Pour espresso over the top.", "Drizzle with caramel sauce." },
                Nutrition = new NutritionInfo { Calories = 200, ProteinGrams = 6, CarbohydratesGrams = 30, FatGrams = 5, FiberGrams = 0, SugarGrams = 25, SodiumMilligrams = 130 },
                DietaryTags = new List<string> { "Vegetarian", "Gluten-Free" }
            },
            new()
            {
                Id = 15, Name = "Trail Mix Energy Bites", Category = "Snacks",
                Description = "No-bake energy bites packed with oats, nuts, dried fruit, and dark chocolate.",
                ImageUrl = "energy_bites.png", PrepTimeMinutes = 15, CookTimeMinutes = 0, Servings = 12,
                Difficulty = "Easy", Rating = 4.0,
                Ingredients = new List<string> { "1 cup rolled oats", "½ cup peanut butter", "¼ cup honey", "¼ cup dark chocolate chips", "¼ cup dried cranberries", "2 tbsp chia seeds" },
                Instructions = new List<string> { "Mix all ingredients in a bowl.", "Refrigerate for 30 minutes.", "Roll into 12 balls.", "Store in the fridge." },
                Nutrition = new NutritionInfo { Calories = 120, ProteinGrams = 4, CarbohydratesGrams = 14, FatGrams = 6, FiberGrams = 2, SugarGrams = 7, SodiumMilligrams = 40 },
                DietaryTags = new List<string> { "Vegetarian", "High Fiber" }
            },
            new()
            {
                Id = 16, Name = "French Onion Soup", Category = "Lunch",
                Description = "Rich and savoury French onion soup topped with a golden, bubbly Gruyère crouton.",
                ImageUrl = "onion_soup.png", PrepTimeMinutes = 15, CookTimeMinutes = 45, Servings = 4,
                Difficulty = "Medium", Rating = 4.5,
                Ingredients = new List<string> { "4 large onions", "50g butter", "1L beef stock", "1 cup white wine", "Fresh thyme", "Baguette slices", "Gruyère cheese" },
                Instructions = new List<string> { "Slice onions thinly.", "Caramelize in butter for 25-30 minutes.", "Deglaze with white wine.", "Add beef stock and thyme, simmer 15 minutes.", "Ladle into oven-safe bowls.", "Top with baguette and Gruyère, broil until bubbly." },
                Nutrition = new NutritionInfo { Calories = 380, ProteinGrams = 14, CarbohydratesGrams = 35, FatGrams = 18, FiberGrams = 3, SugarGrams = 10, SodiumMilligrams = 900 },
                DietaryTags = new List<string> { "Vegetarian Option" }
            },
            new()
            {
                Id = 17, Name = "Overnight Oats", Category = "Breakfast",
                Description = "Creamy overnight oats with chia seeds, almond milk, and fresh fruit toppings.",
                ImageUrl = "overnight_oats.png", PrepTimeMinutes = 10, CookTimeMinutes = 0, Servings = 1,
                Difficulty = "Easy", Rating = 4.1,
                Ingredients = new List<string> { "½ cup rolled oats", "1 tbsp chia seeds", "¾ cup almond milk", "1 tbsp maple syrup", "Fresh berries", "Sliced almonds" },
                Instructions = new List<string> { "Combine oats, chia seeds, almond milk, and maple syrup in a jar.", "Stir well and seal.", "Refrigerate overnight.", "Top with fresh berries and almonds before serving." },
                Nutrition = new NutritionInfo { Calories = 310, ProteinGrams = 8, CarbohydratesGrams = 48, FatGrams = 10, FiberGrams = 10, SugarGrams = 14, SodiumMilligrams = 120 },
                DietaryTags = new List<string> { "Vegan", "High Fiber" }
            },
            new()
            {
                Id = 18, Name = "Caprese Salad", Category = "Lunch",
                Description = "Fresh Italian salad with ripe tomatoes, buffalo mozzarella, basil, and balsamic glaze.",
                ImageUrl = "caprese.png", PrepTimeMinutes = 10, CookTimeMinutes = 0, Servings = 2,
                Difficulty = "Easy", Rating = 4.3,
                Ingredients = new List<string> { "2 large tomatoes", "200g buffalo mozzarella", "Fresh basil leaves", "Balsamic glaze", "Extra virgin olive oil", "Sea salt" },
                Instructions = new List<string> { "Slice tomatoes and mozzarella.", "Arrange alternating on a plate.", "Tuck basil leaves between slices.", "Drizzle with olive oil and balsamic glaze.", "Season with sea salt." },
                Nutrition = new NutritionInfo { Calories = 280, ProteinGrams = 15, CarbohydratesGrams = 8, FatGrams = 20, FiberGrams = 2, SugarGrams = 5, SodiumMilligrams = 350 },
                DietaryTags = new List<string> { "Vegetarian", "Gluten-Free", "Low Carb" }
            },
            new()
            {
                Id = 19, Name = "Apple Crumble", Category = "Desserts",
                Description = "Warm cinnamon-spiced apple crumble with a buttery oat topping and vanilla custard.",
                ImageUrl = "apple_crumble.png", PrepTimeMinutes = 20, CookTimeMinutes = 35, Servings = 6,
                Difficulty = "Easy", Rating = 4.6,
                Ingredients = new List<string> { "6 apples", "½ cup brown sugar", "Cinnamon", "1 cup flour", "½ cup butter", "½ cup oats", "Vanilla custard" },
                Instructions = new List<string> { "Peel and slice apples.", "Toss apples with sugar and cinnamon.", "Place in baking dish.", "Rub butter into flour and oats for crumble.", "Sprinkle over apples.", "Bake at 180°C for 35 minutes.", "Serve warm with custard." },
                Nutrition = new NutritionInfo { Calories = 350, ProteinGrams = 4, CarbohydratesGrams = 55, FatGrams = 14, FiberGrams = 4, SugarGrams = 32, SodiumMilligrams = 100 },
                DietaryTags = new List<string> { "Vegetarian" }
            },
            new()
            {
                Id = 20, Name = "Spiced Roasted Chickpeas", Category = "Snacks",
                Description = "Crunchy oven-roasted chickpeas seasoned with paprika, cumin, and garlic powder.",
                ImageUrl = "roasted_chickpeas.png", PrepTimeMinutes = 5, CookTimeMinutes = 30, Servings = 4,
                Difficulty = "Easy", Rating = 4.0,
                Ingredients = new List<string> { "400g can chickpeas", "1 tbsp olive oil", "1 tsp paprika", "½ tsp cumin", "½ tsp garlic powder", "Salt" },
                Instructions = new List<string> { "Drain, rinse, and dry chickpeas thoroughly.", "Toss with olive oil and spices.", "Spread on baking sheet.", "Roast at 200°C for 25-30 minutes, shaking halfway.", "Cool completely before serving." },
                Nutrition = new NutritionInfo { Calories = 150, ProteinGrams = 7, CarbohydratesGrams = 18, FatGrams = 5, FiberGrams = 5, SugarGrams = 1, SodiumMilligrams = 250 },
                DietaryTags = new List<string> { "Vegan", "Gluten-Free", "High Fiber" }
            },
            new()
            {
                Id = 21, Name = "Beef Tacos", Category = "Dinner",
                Description = "Mexican-style beef tacos with fresh salsa, guacamole, and lime crema.",
                ImageUrl = "tacos.png", PrepTimeMinutes = 20, CookTimeMinutes = 15, Servings = 4,
                Difficulty = "Medium", Rating = 4.6,
                Ingredients = new List<string> { "500g ground beef", "Taco seasoning", "8 corn tortillas", "Tomatoes", "Onion", "Avocado", "Lime", "Sour cream", "Cilantro" },
                Instructions = new List<string> { "Brown beef with taco seasoning.", "Dice tomatoes and onion for salsa.", "Mash avocado with lime and salt for guacamole.", "Warm tortillas.", "Assemble tacos with beef, salsa, guacamole, and crema." },
                Nutrition = new NutritionInfo { Calories = 480, ProteinGrams = 30, CarbohydratesGrams = 35, FatGrams = 24, FiberGrams = 6, SugarGrams = 3, SodiumMilligrams = 650 },
                DietaryTags = new List<string> { "Gluten-Free Option", "High Protein" }
            }
        };

        foreach (var cat in _categories)
        {
            cat.RecipeCount = _recipes.Count(r =>
                r.Category.Equals(cat.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
