using UnityEngine;

public partial class ResourcesDictionaryDrawer
{
    static private class ItemDrawOptions
    {
        static private Color COLOR_ITEM_VALID = new Color(1.00f, 1.00f, 0.87f);
        static private Color COLOR_ITEM_ALL_ERROR = new Color(0.61f, 0.60f, 0.68f);
        static private Color COLOR_ITEM_HAS_ERROR = new Color(0.90f, 0.90f, 0.90f);

        static private Color COLOR_PART_VALID = new Color(1.00f, 1.00f, 1.00f);
        static private Color COLOR_PART_ERROR = new Color(1.00f, 0.20f, 0.25f);

        static public DrawOptionsResult GetResult(ValidatorResult result)
        {
            return new DrawOptionsResult()
            {
                backgroundItem = GetColorItemBackground(result.isItemValid, result.isItemAllError),
                BackgroundKey = GetColorPartBackground(result.isKeyValid, result.isItemValid, result.isItemAllError),
                BackgroundValue = GetColorPartBackground(result.isValueValid, result.isItemValid, result.isItemAllError)
            };
        }

        private static Color GetColorPartBackground(bool isPartValid, bool isItemValid, bool isItemAllError)
        {
            if (isItemAllError)
                return COLOR_ITEM_ALL_ERROR;
            if (isItemValid)
                return COLOR_ITEM_VALID;
            if (isPartValid)
                return COLOR_PART_VALID;

            return COLOR_PART_ERROR;
        }

        private static Color GetColorItemBackground(bool isItemValid, bool isItemAllError)
        {
            if (isItemAllError)
                return COLOR_ITEM_ALL_ERROR;
            if (isItemValid)
                return COLOR_ITEM_VALID;

            return COLOR_ITEM_HAS_ERROR;
        }
    }

}
