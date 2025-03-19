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
    
    public static T Clone<T>(this T source) where T : new()
    {
        var clone = new T();
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.CanWrite)
                property.SetValue(clone, property.GetValue(source));
        }
        return clone;
    }
}