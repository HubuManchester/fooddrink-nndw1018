# Foodie — Your Ultimate Food Companion

<p align="center">
  <img src="src/FoodieApp/Resources/Images/appicon.svg" alt="Foodie Logo" width="120" />
</p>

A cross-platform mobile app built with **.NET MAUI** that helps you discover recipes, scan food barcodes for nutrition info, plan weekly meals, track your food records, and find nearby restaurants.

---

## Features

### 🏠 Discover
- Browse recipes by food categories
- **Recipe of the Day** — a fresh pick every time you open the app
- **Shake to discover** — shake your device to get a random recipe
- Full-text search across recipe names, descriptions, and ingredients

### 📖 Recipes
- Complete recipe list with filtering and sorting
- Filter by **category**, **difficulty** (Easy / Medium / Hard), and **favourites**
- Sort by **rating**, **name**, **prep time**, or **calories**
- Tap any recipe to view detailed instructions, ingredients, and nutrition

### 📷 Barcode Scanner
- Scan food product barcodes with your camera (powered by ZXing.Net.Maui)
- Manual barcode entry as a fallback
- Instantly retrieve product nutrition information, ingredients, and allergens

### 📅 Meal Planner
- Plan your **weekly meals** — Breakfast, Lunch, Dinner, and Snacks
- Assign recipes to each meal slot
- **Daily nutrition totals** automatically calculated
- Text-to-Speech reads your daily nutrition summary aloud

### 📝 Food Records
- Create detailed food records with:
  - **Photo capture** using your device camera
  - **GPS location** with reverse geocoding (address lookup)
  - Full nutrition data entry with validation
- View, browse, and delete your saved records

### 🗺️ Restaurant Finder
- Find nearby restaurants using your GPS location
- See restaurant names, cuisines, ratings, distances, and open/closed status
- Tap to open any restaurant in your device's map app with driving directions

### ⚙️ Accessibility & Settings
- **Dark Mode** — reduces eye strain in low light
- **Font Scaling** — adjustable from 0.8× to 2.0×
- **Reduce Animations** — minimizes motion for vestibular comfort
- **High Contrast** — increased contrast ratios for better readability
- **Haptic Feedback** — tactile responses on supported devices
- **Text-to-Speech** — reads recipes, nutrition info, and accessibility details aloud
- Follows **WCAG 2.1** guidelines for mobile accessibility

---

## Tech Stack

| Technology | Purpose |
|---|---|
| [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) | Runtime & SDK |
| [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui) | Cross-platform UI framework |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | MVVM source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui) | UI components, animations, converters |
| [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) | Camera-based barcode scanning |
| `Microsoft.Extensions.Logging` | Debug logging |

### Target Platforms

| Platform | Minimum Version |
|---|---|
| Android | 5.0 (API 21) |
| iOS | 15.0 |
| macOS | 15.0 (Mac Catalyst) |
| Windows | 10.0.17763.0 |

---

## Architecture

The app follows **MVVM** (Model-View-ViewModel) with **dependency injection** via the built-in `MauiApp.CreateBuilder` container.

```
View (XAML Pages)
    ↕ data binding
ViewModel (MVVM Toolkit)
    ↕ service interfaces
Service Layer (implementations)
    ↕
Models (data classes)
```

### Key Patterns
- **CommunityToolkit.Mvvm** source generators for boilerplate-free MVVM:
  - `[ObservableProperty]` — generates `INotifyPropertyChanged` bindable properties
  - `[RelayCommand]` — generates `ICommand` implementations from methods
- **Singleton services** for stateful data (recipes, settings, food records)
- **Transient ViewModels** for detail/navigation pages that receive `[QueryProperty]` parameters
- **`BaseViewModel`** provides shared `IsBusy`, error handling, and `ExecuteAsync` helper

---

## Project Structure

```
FoodieApp/
├── FoodieApp.sln                       # Solution file
└── src/
    ├── FoodieApp/                      # Main app project
    │   ├── App.xaml / App.xaml.cs       # Application entry & settings wiring
    │   ├── AppShell.xaml                # Shell-based tab navigation
    │   ├── MauiProgram.cs              # DI container & service registration
    │   │
    │   ├── Models/                     # Data classes
    │   │   ├── Recipe.cs               # Recipe with nutrition, ingredients, instructions
    │   │   ├── FoodCategory.cs         # Browse category (name, icon, count)
    │   │   ├── BarcodeProduct.cs       # Scanned product with allergens
    │   │   ├── MealPlan.cs             # Meal slot (date, type, recipe)
    │   │   ├── FoodRecord.cs           # User food record (photo, GPS, nutrition)
    │   │   ├── Restaurant.cs           # Nearby restaurant info
    │   │   ├── NutritionInfo.cs        # Calories, macros, fiber, sugar, sodium
    │   │   └── AppSettings.cs          # Theme, font, animation preferences
    │   │
    │   ├── ViewModels/                 # MVVM ViewModels
    │   │   ├── BaseViewModel.cs        # Shared logic (IsBusy, error handling)
    │   │   ├── MainViewModel.cs        # Discover page — categories, search, shake
    │   │   ├── RecipeListViewModel.cs  # Recipe list — filter, sort, favourites
    │   │   ├── RecipeDetailViewModel.cs # Recipe detail — TTS, share, servings
    │   │   ├── BarcodeScanViewModel.cs # Barcode scanning & product lookup
    │   │   ├── MealPlannerViewModel.cs # Weekly meal planner & nutrition totals
    │   │   ├── AddFoodRecordViewModel.cs # New food record — photo, GPS, validation
    │   │   ├── MyRecordsViewModel.cs   # Browse & delete saved records
    │   │   ├── RestaurantFinderViewModel.cs # GPS-based restaurant discovery
    │   │   └── SettingsViewModel.cs    # Accessibility & theme settings
    │   │
    │   ├── Views/                      # XAML Pages
    │   │   ├── MainPage.xaml
    │   │   ├── RecipeListPage.xaml
    │   │   ├── RecipeDetailPage.xaml
    │   │   ├── BarcodeScanPage.xaml
    │   │   ├── MealPlannerPage.xaml
    │   │   ├── AddFoodRecordPage.xaml
    │   │   ├── MyRecordsPage.xaml
    │   │   ├── RestaurantFinderPage.xaml
    │   │   └── SettingsPage.xaml
    │   │
    │   ├── Services/                   # Business logic & data access
    │   │   ├── IRecipeService.cs / RecipeService.cs
    │   │   ├── INutritionService.cs / NutritionService.cs
    │   │   ├── IBarcodeService.cs / BarcodeService.cs
    │   │   ├── ISettingsService.cs / SettingsService.cs
    │   │   └── IFoodRecordService.cs / FoodRecordService.cs
    │   │
    │   ├── Helpers/                    # Utility & platform helpers
    │   │   ├── Constants.cs            # App-wide constants & error messages
    │   │   ├── AccessibilityHelper.cs  # Semantic properties & font scaling
    │   │   ├── FontScaleHelper.cs      # Visual tree font scaling
    │   │   ├── HapticFeedbackHelper.cs # Haptic click & long-press wrappers
    │   │   ├── HardwareHelper.cs       # Camera & location permission checks
    │   │   └── TextToSpeechHelper.cs   # Cross-platform TTS with pause/resume
    │   │
    │   ├── Converters/                 # XAML value converters
    │   │   ├── BoolToColorConverter.cs
    │   │   ├── BoolToErrorColorConverter.cs
    │   │   ├── CountToBoolConverter.cs
    │   │   ├── DifficultyToColorConverter.cs
    │   │   ├── InverseBoolConverter.cs
    │   │   ├── OpenStatusConverter.cs
    │   │   ├── RatingToStarsConverter.cs
    │   │   └── ScanButtonTextConverter.cs
    │   │
    │   ├── Resources/                  # Fonts, images, styles, raw assets
    │   │   ├── Fonts/
    │   │   ├── Images/
    │   │   ├── Raw/
    │   │   └── Styles/
    │   │       ├── Colors.xaml          # App colour palette
    │   │       ├── Styles.xaml          # Global control styles
    │   │       └── AccessibilityStyles.xaml
    │   │
    │   └── Platforms/                  # Platform-specific code
    │       ├── Android/
    │       ├── iOS/
    │       └── Windows/
    │
    └── FoodieApp.Analyzers/            # Custom Roslyn analyzers (dev tools)
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 (v17.8+) with the **.NET MAUI** workload, or the `dotnet workload install maui` command
- **Android**: Android SDK (installed via Visual Studio or standalone)
- **iOS / macOS**: Xcode 15+ (macOS only)
- **Windows**: Windows 10 SDK (10.0.19041+)

### Restore & Build

```bash
# Restore dependencies
dotnet restore

# Build for all target platforms
dotnet build

# Build for a specific platform
dotnet build -f net8.0-android
dotnet build -f net8.0-ios
dotnet build -f net8.0-windows10.0.19041.0
```

### Run

```bash
# Run on Windows
dotnet run -f net8.0-windows10.0.19041.0

# Run on Android emulator / device
dotnet run -f net8.0-android

# Run on iOS simulator (macOS only)
dotnet run -f net8.0-ios
```

Or open `FoodieApp.sln` in Visual Studio, select your target platform, and press **F5**.

---

## Key Design Decisions

- **No external API dependencies** — all recipe, category, and product data is served from in-memory services, making the app fully functional offline
- **Reverse geocoding** uses free services (BigDataCloud, Nominatim OSM) with no API key required
- **Shell navigation** with `TabBar` for top-level pages and `[QueryProperty]` for detail navigation
- **Source-generated MVVM** via CommunityToolkit.Mvvm to keep ViewModels clean and testable
- **Platform-adaptive TTS** — custom Windows `MediaPlayer`-based implementation that supports pause/resume, alongside MAUI's built-in `TextToSpeech` for Android/iOS

---

## Accessibility (WCAG 2.1)

Foodie is built with accessibility in mind:

- **Screen reader support** — all interactive elements have `SemanticProperties.Description` and `SemanticProperties.Hint`
- **Scalable text** — font size adjustable from 80% to 200%
- **High contrast mode** — increased contrast ratios for low-vision users
- **Reduced motion** — disables animations for users with vestibular disorders
- **Text-to-Speech** — reads recipes, nutrition info, and UI guidance aloud with pause/resume control
- **Dark mode** — reduces eye strain and glare

---

## License

This project is provided for educational and demonstration purposes.

---

## Acknowledgements

- [.NET MAUI](https://github.com/dotnet/maui)
- [CommunityToolkit](https://github.com/CommunityToolkit)
- [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui)
- [BigDataCloud Reverse Geocoding](https://www.bigdatacloud.com/)
- [OpenStreetMap Nominatim](https://nominatim.org/)
