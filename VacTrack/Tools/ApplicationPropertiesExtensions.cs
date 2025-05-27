using System.Collections;

namespace VacTrack.Tools
{
    public static class ApplicationPropertiesExtensions
    {
        public static bool TryGet<T>(this IDictionary properties, string key, out T value)
        {
            if (properties.Contains(key) && properties[key] is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default!;
            return false;
        }
    }
}
