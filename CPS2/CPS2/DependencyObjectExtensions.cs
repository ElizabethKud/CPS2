using System.Windows;
using System.Windows.Media;

namespace CPS2;

public static class DependencyObjectExtensions
{
    public static IEnumerable<T> GetSelfAndAncestors<T>(this DependencyObject obj) where T : DependencyObject
    {
        while (obj != null)
        {
            if (obj is T t) yield return t;
            obj = VisualTreeHelper.GetParent(obj);
        }
    }
}