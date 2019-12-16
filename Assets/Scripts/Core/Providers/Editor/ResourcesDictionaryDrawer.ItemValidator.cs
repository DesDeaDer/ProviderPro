using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public partial class ResourcesDictionaryDrawer
{
    static private class ItemValidator
    {

        static public ValidatorResult GetResult(IEnumerable<Item> items, Item item, int index)
        {
            bool isKeyValid = IsValidKey(items.Select(x => x.key), item.key, index);
            bool isValueValid = IsValueValid(item.value);

            return new ValidatorResult
            {
                isKeyValid = isKeyValid,
                isKeyHasError = !isValueValid,
                isValueValid = isValueValid,
                isValueHasError = isValueValid,
                isItemValid = isKeyValid && isValueValid,
                isItemHasError = !isKeyValid || !isValueValid,
                isItemAllError = !isKeyValid && !isValueValid
            };
        }

        static private bool IsValueValid(Object value) => value != null;

        static private bool IsValidKey(IEnumerable<string> keys, string key, int index)
        {
            if (!IsValidKey(key))
            {
                return false;
            }

            var currentIndex = 0;
            foreach (var current in keys)
            {
                if (currentIndex < index)
                {
                    if (current == key)
                    {
                        return false;
                    }
                }
                ++currentIndex;
            }

            return true;
        }

        private static bool IsValidKey(string key)
        {
            if
            (
                string.IsNullOrEmpty(key)
                ||
                (
                    !char.IsLetter(key[0])
                    && key[0] != '_'
                )
                ||
                (
                    key == Item.KEY_DEFAULT
                    || key == Item.KEY_ERROR
                    || key == Item.KEY_NONE
                )
            )
            {
                return false;
            }

            foreach (var ch in key)
            {
                if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    return false;
                }
            }

            return true;
        }
    }

}
