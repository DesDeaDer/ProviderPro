using UnityEngine;
using Demo.Test;

namespace MyNamespace
{
    public class Demo : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TestProviderID _resourceID;
        [SerializeField] private TestProvider _prefabsProvider;

#pragma warning restore 0649
        #endregion

        public TestProvider Prefabs => _prefabsProvider;
        public TestProviderID ResourceID => _resourceID;

        void Start()
        {
            if (ResourceID != TestProviderID.None)
            {
                Instantiate(Prefabs[ResourceID]);
            }
        }
    }
}
