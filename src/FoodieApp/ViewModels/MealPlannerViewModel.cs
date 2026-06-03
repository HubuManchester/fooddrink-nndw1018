namespace FoodieApp.ViewModels;

public partial class MealPlannerViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;

    public MealPlannerViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        Title = "Meal Planner";
    }

    [ObservableProperty]
    private ObservableCollection<MealPlan> _weeklyPlan = new();

    [ObservableProperty]
    private ObservableCollection<Recipe> _availableRecipes = new();

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private NutritionInfo _dailyNutritionTotal = new();

    [ObservableProperty]
    private string _selectedMealType = "Breakfast";

    public List<string> MealTypes { get; } = new() { "Breakfast", "Lunch", "Dinner", "Snack" };

    [RelayCommand]
    private async Task LoadMealPlanAsync()
    {
        await ExecuteAsync(async () =>
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            AvailableRecipes = new ObservableCollection<Recipe>(recipes);

            var startOfWeek = SelectedDate.AddDays(-(int)SelectedDate.DayOfWeek);
            var plan = new List<MealPlan>();
            for (int day = 0; day < 7; day++)
            {
                var date = startOfWeek.AddDays(day);
                plan.Add(new MealPlan { Id = day * 4 + 1, Date = date, Type = MealType.Breakfast, RecipeName = "Tap to add" });
                plan.Add(new MealPlan { Id = day * 4 + 2, Date = date, Type = MealType.Lunch, RecipeName = "Tap to add" });
                plan.Add(new MealPlan { Id = day * 4 + 3, Date = date, Type = MealType.Dinner, RecipeName = "Tap to add" });
                plan.Add(new MealPlan { Id = day * 4 + 4, Date = date, Type = MealType.Snack, RecipeName = "Tap to add" });
            }
            WeeklyPlan = new ObservableCollection<MealPlan>(plan);
        }, "Failed to load meal plan");
    }

    [RelayCommand]
    private async Task SpeakDailyNutritionAsync()
    {
        if (DailyNutritionTotal == null) return;

        try
        {
            var n = DailyNutritionTotal;
            var text = $"Daily nutrition summary. " +
                       $"Total calories: {n.Calories:F0}. " +
                       $"Protein: {n.ProteinGrams:F1} grams. " +
                       $"Carbohydrates: {n.CarbohydratesGrams:F1} grams. " +
                       $"Fat: {n.FatGrams:F1} grams. " +
                       $"Fiber: {n.FiberGrams:F1} grams. " +
                       $"Sugar: {n.SugarGrams:F1} grams. " +
                       $"Sodium: {n.SodiumMilligrams:F0} milligrams.";

            await Helpers.TextToSpeechHelper.SpeakAsync(text);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Text-to-Speech Error",
                $"Could not speak nutrition: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SelectMealSlotAsync(MealPlan mealPlan)
    {
        if (mealPlan == null) return;

        try
        {
            var recipeNames = AvailableRecipes.Select(r => r.Name).ToArray();
            if (recipeNames.Length == 0)
            {
                await Shell.Current.DisplayAlert("No Recipes", "Please load the meal plan first.", "OK");
                return;
            }

            string selectedName = await Shell.Current.DisplayActionSheet(
                $"Select recipe for {mealPlan.Type}",
                "Cancel",
                null,
                recipeNames);

            if (string.IsNullOrEmpty(selectedName) || selectedName == "Cancel") return;

            var recipe = AvailableRecipes.FirstOrDefault(r => r.Name == selectedName);
            if (recipe == null) return;

            var existingMeal = WeeklyPlan.FirstOrDefault(m =>
                m.Id == mealPlan.Id);

            if (existingMeal != null)
            {
                existingMeal.RecipeName = recipe.Name;
                existingMeal.RecipeId = recipe.Id;

                // Refresh the collection to update the UI
                var plan = WeeklyPlan.ToList();
                WeeklyPlan = new ObservableCollection<MealPlan>(plan);
            }

            CalculateDailyNutrition();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Meal plan error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddMealToPlanAsync(Recipe recipe)
    {
        if (recipe == null) return;

        try
        {
            string action = await Shell.Current.DisplayActionSheet(
                "Select meal type",
                "Cancel",
                null,
                "Breakfast", "Lunch", "Dinner", "Snack");

            if (string.IsNullOrEmpty(action) || action == "Cancel") return;

            var mealType = Enum.Parse<MealType>(action);
            var existingMeal = WeeklyPlan.FirstOrDefault(m =>
                m.Date.Date == SelectedDate.Date && m.Type == mealType);

            if (existingMeal != null)
            {
                existingMeal.RecipeName = recipe.Name;
                existingMeal.RecipeId = recipe.Id;
            }

            CalculateDailyNutrition();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Meal plan error: {ex.Message}");
        }
    }

    private void CalculateDailyNutrition()
    {
        try
        {
            var todayMeals = WeeklyPlan
                .Where(m => m.Date.Date == SelectedDate.Date && m.RecipeId > 0)
                .ToList();

            var recipesForToday = new List<Recipe>();
            foreach (var meal in todayMeals)
            {
                var recipe = AvailableRecipes.FirstOrDefault(r => r.Id == meal.RecipeId);
                if (recipe != null)
                {
                    recipesForToday.Add(recipe);
                }
            }

            var nutritionService = new NutritionService();
            DailyNutritionTotal = nutritionService.CalculateDailyTotal(recipesForToday);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Nutrition calculation error: {ex.Message}");
        }
    }
}
