using Object = UnityEngine.Object;

public partial class ResourcesDictionaryDrawer
{
    private class Item
    {
        public const string KEY_DEFAULT = "";
        public const string KEY_NONE = "None";
        public const string KEY_ERROR = "Error";

        public string key;
        public Object value;

        public Item(Item item)
        {
            key = item.key;
            value = item.value;
        }

        public Item(string key = KEY_DEFAULT, Object value = null)
        {
            this.key = key;
            this.value = value;
        }

        public void Set(Item item)
        {
            key = item.key;
            value = item.value;
        }
    }

}
