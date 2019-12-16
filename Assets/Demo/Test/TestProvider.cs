using Core.Providers;
using UnityEngine;

namespace Demo.Test
{
    [CreateAssetMenu(menuName = "Providers/Test")]
    public class TestProvider : ResourcesProviderBase<TestProviderID, GameObject> { }
}
