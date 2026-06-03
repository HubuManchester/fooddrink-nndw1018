namespace FoodieApp.Helpers;

/// <summary>
/// Walks the visual tree and scales all Label/Entry/Button font sizes.
/// Tracks original sizes internally so repeated scaling doesn't compound.
/// </summary>
public static class FontScaleHelper
{
    private static readonly Dictionary<int, double> _originals = new();
    private static double _lastScale = 1.0;

    public static void ApplyScale(Element root, double scale)
    {
        if (root == null) return;
        WalkTree(root, scale, _lastScale);
        _lastScale = scale;
    }

    private static void WalkTree(Element element, double newScale, double oldScale)
    {
        if (element is Label label)
        {
            Apply(element, label, newScale, oldScale, (l, s) => l.FontSize = s);
        }
        else if (element is Entry entry)
        {
            Apply(element, entry, newScale, oldScale, (e, s) => e.FontSize = s);
        }
        else if (element is Button button)
        {
            Apply(element, button, newScale, oldScale, (b, s) => b.FontSize = s);
        }

        if (element is IVisualTreeElement visual)
        {
            foreach (var child in visual.GetVisualChildren())
            {
                if (child is Element childEl)
                    WalkTree(childEl, newScale, oldScale);
            }
        }
    }

    private static void Apply<T>(Element element, T target, double newScale, double oldScale, Action<T, double> setter)
        where T : VisualElement
    {
        int id = element.GetHashCode();
        double currentSize = target switch
        {
            Label l => l.FontSize,
            Entry e => e.FontSize,
            Button b => b.FontSize,
            _ => 14.0
        };

        // Recover original: current = original * oldScale
        if (!_originals.ContainsKey(id))
            _originals[id] = currentSize;

        double original = _originals[id];
        double newSize = Math.Clamp(Math.Round(original * newScale, 1), 6, 80);
        setter(target, newSize);
    }
}
