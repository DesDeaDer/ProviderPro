using UnityEngine;
using Demo.Test;

public class Load1 : MonoBehaviour
{

    #region Data
#pragma warning disable 0649

    [SerializeField] private TestProvider _prefabsProvider;

#pragma warning restore 0649
    #endregion

    public TestProvider Prefabs
    {
        get
        {
            return _prefabsProvider;
        }
    }

    void Start()
    {
        var objectID = TestProviderID.Cube1;
        var objectRef = Prefabs[objectID];

        Instantiate(objectRef);
    }

}
