using Core.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ResourcesProviderBase<,>), true)]
public partial class ResourcesDictionaryDrawer : Editor
{
    [SerializeField] private Texture _addItemImage;
    [SerializeField] private Texture _dublicateItemImage;
    [SerializeField] private Texture _removeItemImage;
    [SerializeField] private Texture _viewItemImage;

    private const string PATH_PROPERTY_DATA = "_values";
    private const int ITEM_HEIGHT = 20;
    private const int BUTTON_HEIGHT = ITEM_HEIGHT + 1;
    private const int BUTTON_WIDTH = ITEM_HEIGHT + 1;

    private const string PATH_FOLDER_RESOURCES = "Resources/";

    private readonly Color COLOR_BUTTON_ACTIVE = new Color(0.80f, 1.00f, 0.80f);
    private readonly Color COLOR_BUTTON_DEACTIVE = new Color(0.70f, 0.67f, 0.67f);

    static private readonly GUILayoutOption _buttonWidthOption = GUILayout.Width(BUTTON_WIDTH);
    static private readonly GUILayoutOption _buttonHeightOption = GUILayout.Height(BUTTON_HEIGHT);
    static private readonly GUILayoutOption _itemHeightOption = GUILayout.Height(ITEM_HEIGHT);

    public bool IsChanged { get; private set; }
    public bool IsItemsValid { get; private set; }

    private void SetChanged()
    {
        IsChanged = true;
    }

    private void SetUnchanged()
    {
        IsChanged = false;
    }

    private Type _keyType;
    private Type _valueType;
    private SerializedProperty _itemsProperty;
    private IList<Item> _items;

    protected bool IsInitialized { get; private set; }
    public Color Color { get; private set; }

    private void OnEnable()
    {
        IsInitialized = TryGetInitialize(target.GetType(), out _itemsProperty, out _items, out _keyType, out _valueType);
    }

    private void OnDisable()
    {
        IsInitialized = false;
    }

    public override void OnInspectorGUI()
    {
        if (IsInitialized)
        {
            DrawProccesing(_itemsProperty, _items, _keyType, _valueType);
        }
    }

    private bool TryGetInitialize(Type targetType, out SerializedProperty arrayProperty, out IList<Item> items, out Type keyType, out Type valueType)
    {
        var arrayPropertyResult = serializedObject.FindProperty(PATH_PROPERTY_DATA);
        if (arrayPropertyResult != null)
        {
            Type keyTypeResult;
            Type valueTypeResult;
            if (TryGetKeyValueType(target.GetType(), out keyTypeResult, out valueTypeResult))
            {
                var keys = keyTypeResult.GetEnumNames();
                var itemsResult = new List<Item>(arrayPropertyResult.arraySize);

                for (int i = 0; i < arrayPropertyResult.arraySize; i++)
                {
                    var itemProperty = arrayPropertyResult.GetArrayElementAtIndex(i);
                    var itemValue = GetObjectRef(valueTypeResult, itemProperty);
                    var itemKey = GetKeyFromIndex(keys, i);

                    itemsResult.Add(new Item(itemKey, itemValue));
                }

                items = itemsResult;
                arrayProperty = arrayPropertyResult;
                keyType = keyTypeResult;
                valueType = valueTypeResult;

                return true;
            }
        }

        items = null;
        arrayProperty = null;
        keyType = null;
        valueType = null;

        return false;
    }

    static private bool TryGetKeyValueType(Type type, out Type keyType, out Type valueType)
    {
        keyType = null;
        valueType = null;

        try
        {
            var typeComliteGeneric = type;
            var typeTarget = typeof(ResourcesProviderBase<,>);
            var typePeek = type;

            do
            {
                typeComliteGeneric = typePeek;
                typePeek = typeComliteGeneric.BaseType;
            }
            while (typePeek == null || typePeek.IsGenericType && typePeek.GetGenericTypeDefinition() == typeTarget);

            var types = typeComliteGeneric.GetGenericArguments();

            keyType = types[0];
            valueType = types[1];

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    static private Object GetObjectRef(Type valueType, SerializedProperty itemProperty)
    {
        return Resources.Load(itemProperty.stringValue, valueType);
    }

    static private string GetKeyFromIndex(string[] keys, int i)
    {
        if (i + 1 < keys.Length)
        {
            return keys[i + 1];
        }

        return Item.KEY_ERROR;
    }

    private void DrawProccesing(SerializedProperty itemsProperty, IList<Item> items, Type keyType, Type valueType)
    {
        GUILayout.BeginVertical();

        var topPanelResult = DrawPanelTop(items, IsChanged, IsChanged && IsItemsValid);
        ProccesingPanelItems(items, valueType, _itemHeightOption);

        GUILayout.EndVertical();

        switch (topPanelResult)
        {
            case TopPanelResult.AddOneLastItem:
                items.Add(new Item());
                SetChanged();
                break;
            case TopPanelResult.SaveAll:
                Save(itemsProperty, items, keyType, serializedObject.targetObject);
                serializedObject.ApplyModifiedProperties();
                SetUnchanged();
                break;
        }
    }

    private void ProccesingPanelItems(IList<Item> items, Type valueType, params GUILayoutOption[] options)
    {
        var isItemsValid = true;

        var index = 0;

        ValidatorResult validatorResult;
        DrawOptionsResult drawOptionsResult;
        ItemToolsResult itemToolsResult;
        Item itemResult;
        foreach (var item in items)
        {
            validatorResult = ItemValidator.GetResult(items, item, index);
            drawOptionsResult = ItemDrawOptions.GetResult(validatorResult);

            //Draw item
            GUILayout.BeginHorizontal(options);

            itemResult = DrawItem(item, valueType, index, drawOptionsResult);
            itemToolsResult = DrawItemTools(items, index, _buttonWidthOption, _buttonHeightOption);

            GUILayout.EndHorizontal();

            //Collect validate
            if
            (
                item.key != itemResult.key
                || item.value != itemResult.value
            )
            {
                SetChanged();
                item.Set(itemResult);
                if (isItemsValid) //[OPTIMIZE] if false, all isItemsValid = false for any results from forward in the list
                {
                    isItemsValid = ItemValidator.GetResult(items, itemResult, index).isItemValid;
                }
            }
            else if (isItemsValid) //[OPTIMIZE] if false, all isItemsValid = false for any results from forward in the list
            {
                isItemsValid = validatorResult.isItemValid;
            }

            //process result from tools
            switch (itemToolsResult)
            {
                case ItemToolsResult.RemoveItem:
                    DeleteItem(items, index);
                    SetChanged();
                    return;
                case ItemToolsResult.DublicateItem:
                    DublicateItemAtIndex(items, index);
                    SetChanged();
                    return;
                case ItemToolsResult.ViewItem:
                    throw new NotImplementedException();
                    //return;
            }

            ++index;
        }

        IsItemsValid = isItemsValid;
    }

    private TopPanelResult DrawPanelTop(ICollection<Item> items, bool isVisibleSaveButton, bool isCanSave)
    {
        var result = TopPanelResult.None;
        var colorBackground = GUI.backgroundColor;

        GUILayout.BeginHorizontal();

        GUILayout.Label("Count: " + items.Count);

        GUILayout.FlexibleSpace();

        if (isVisibleSaveButton)
        {
            GUI.backgroundColor = GetColorButtonBackground(isCanSave);
            if (GUILayout.Button("Save", _buttonHeightOption))
            {
                if (isCanSave)
                {
                    result = TopPanelResult.SaveAll;
                }
            }
            GUI.backgroundColor = colorBackground;
        }

        if (GUILayout.Button(_addItemImage, GUI.skin.label, _buttonWidthOption, _buttonHeightOption))
        {
            result = TopPanelResult.AddOneLastItem;
        }

        GUILayout.EndHorizontal();

        return result;
    }

    private Item DrawItem(Item item, Type valueType, int index, DrawOptionsResult options)
    {
        var backgroundColor = GUI.backgroundColor;

        GUI.backgroundColor = options.backgroundItem;
        GUILayout.BeginHorizontal(GUI.skin.button, _itemHeightOption, _itemHeightOption);

        GUILayout.Label(index.ToString());

        GUI.backgroundColor = options.BackgroundKey;
        var key = EditorGUILayout.TextField(item.key);

        GUI.backgroundColor = options.BackgroundValue;
        var value = EditorGUILayout.ObjectField(item.value, valueType, false);

        GUILayout.EndHorizontal();
        GUI.backgroundColor = backgroundColor;

        return new Item(key, value);
    }

    private ItemToolsResult DrawItemTools(IList<Item> items, int index, params GUILayoutOption[] options)
    {
        var result = ItemToolsResult.None;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(_removeItemImage, GUI.skin.label, options))
        {
            result = ItemToolsResult.RemoveItem;
        }
        if (GUILayout.Button(_dublicateItemImage, GUI.skin.label, options))
        {
            result = ItemToolsResult.DublicateItem;
        }
        if (GUILayout.Button(_viewItemImage, GUI.skin.label, options))
        {
            result = ItemToolsResult.ViewItem;
        }

        GUILayout.EndHorizontal();

        return result;
    }

    static private void Save(SerializedProperty itemsProperty, IList<Item> items, Type keyType, Object target)
    {
        SaveKeys(items, keyType, target);
        SaveValues(itemsProperty, items);
    }

    static private void SaveKeys(IList<Item> items, Type keyType, Object target)
    {
        var code = EnumGenerator.GetCode(keyType.Namespace, keyType.Name, items.Select(x => x.key));
        var path = AssetDatabase.GetAssetPath(target);
        var pathID = Path.Combine(Path.GetDirectoryName(path), keyType.Name + ".cs");

        File.WriteAllText(pathID, code);

        AssetDatabase.Refresh(ImportAssetOptions.Default);
    }

    static private void SaveValues(SerializedProperty itemsProperty, IList<Item> items)
    {
        itemsProperty.arraySize = items.Count;

        int i = 0;
        foreach (var item in items)
        {
            itemsProperty.GetArrayElementAtIndex(i).stringValue = GetObjectPath(item.value);

            ++i;
        }
    }

    static private string GetObjectPath(Object obj)
    {
        if (obj != null)
        {
            var path = AssetDatabase.GetAssetPath(obj);

            var lengthExtension = Path.GetExtension(path).Length;
            var indexStart = path.IndexOf(PATH_FOLDER_RESOURCES);

            if (indexStart >= 0)
            {
                indexStart += PATH_FOLDER_RESOURCES.Length;

                path = path.Substring(indexStart, path.Length - indexStart - lengthExtension);

                return path;
            }
        }

        return string.Empty;
    }

    private void DublicateItemAtIndex(IList<Item> items, int index)
    {
        items.Insert(index, new Item(items[index]));
        SetChanged();
    }

    private void DeleteItem(IList<Item> items, int index)
    {
        items.RemoveAt(index);
        SetChanged();
    }

    private Color GetColorButtonBackground(bool isActive)
    {
        if (isActive)
        {
            return COLOR_BUTTON_ACTIVE;
        }

        return COLOR_BUTTON_DEACTIVE;
    }

}
