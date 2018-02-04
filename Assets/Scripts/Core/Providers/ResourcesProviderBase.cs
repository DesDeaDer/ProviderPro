using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Providers
{

    /// <summary>
    /// The resource provider class from the "Resources" folder
    /// </summary>
    /// <typeparam name="TKey">The type of key that must be inherited from enum with the value "None = 0"</typeparam>
    /// <typeparam name="TValue">The type of the value that must be inherited from UnityEngine.Object</typeparam>
    public abstract class ResourcesProviderBase<TKey, TValue> : ScriptableObject, IProvider<TKey, TValue>
    where TKey : struct
    where TValue : Object
    {

        #region Data
#pragma warning disable 0649
        [SerializeField] private string[] _values;
#pragma warning restore 0649
        #endregion

        public TValue this[TKey key]
        {
            get
            {
                var index = (int)(object)key;
                --index; //key with value 0 is "None"

                return Resources.Load<TValue>(_values[index]);
            }
        }
    }
}
