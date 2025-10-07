using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableTester : MonoBehaviour
{
    Dictionary<string, GameObject> _preloadedObjects
       = new Dictionary<string, GameObject>();

    private IEnumerator PreloadHazards()
    {
        //find all the locations with label "SpaceHazards"
        var loadResourceLocationsHandle
            = Addressables.LoadResourceLocationsAsync("SpaceHazards", typeof(GameObject));

        if (!loadResourceLocationsHandle.IsDone)
            yield return loadResourceLocationsHandle;

        //start each location loading
        List<AsyncOperationHandle> opList = new List<AsyncOperationHandle>();

        foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
        {
            AsyncOperationHandle<GameObject> loadAssetHandle
                = Addressables.LoadAssetAsync<GameObject>(location);
            loadAssetHandle.Completed +=
                obj => { _preloadedObjects.Add(location.PrimaryKey, obj.Result); };
            opList.Add(loadAssetHandle);
        }

        //create a GroupOperation to wait on all the above loads at once. 
        var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);

        if (!groupOp.IsDone)
            yield return groupOp;

        Addressables.Release(loadResourceLocationsHandle);

        //take a gander at our results.
        foreach (var item in _preloadedObjects)
        {
            Debug.Log(item.Key + " - " + item.Value.name);
        }
    }
}
