using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using System.IO;
using System.Text;

using System.Reflection;
using System;

using MoreMountains.InventoryEngine;

namespace AroundEffect
{

    enum DataType
    {
        NONE = 0,
        INVENTORY_ITEM,
        AUDIO_CLIP,
        TEXT_ASSET,
    }

    /// <summary>
    /// Addressable 에셋 로드, Dictionary 형식으로 저장, 데이터 홀더
    /// </summary>
    public class AddressableManager : MonoBehaviour
    {
        public Dictionary<string, InventoryItem> dic_inventoryItem = new Dictionary<string, InventoryItem>();
        public Dictionary<string, AudioClip> dic_audioClip = new Dictionary<string, AudioClip>();
        public Dictionary<string, TextAsset> dic_textAsset = new Dictionary<string, TextAsset>();

        public Dictionary<string, GameObject> dic_stagePrefab = new Dictionary<string, GameObject>();

        public Dictionary<string, TextAsset> dic_jsonStageData = new Dictionary<string, TextAsset>();

        //6/13/2024-LYI
        //???? ?????? ???????? ????
        public Dictionary<string, IResourceLocation> dic_assetLocation = new Dictionary<string, IResourceLocation>(); //???? ???? ????
        public List<int> list_loadedStageNum = new List<int>(); //???? ???? ?? ???????? ????, ???????? ???????? ??????


        public bool isLoadComplete = false;

        [Header("Debug")]
        public bool isStartDebug = false;
        public int debugStageNum = 1001;
        public bool isLogActive = false; //???????? ????

        public int assetsToLoad = 0;
        public int loadCompleteCount = 0;

        private void Start()
        {
            if (!isLoadComplete)
            {
                //GameManager.Instance.ChangeGameStat(GameStatus.LOADING);
                //StartCoroutine(LoadAddressableAssets());
            }
        }

        private IEnumerator LoadAddressableAssets()
        {
            assetsToLoad = 1;
            StartCoroutine(LoadAddressableAssetsLabel(Constants.Label.LABEL_AUDIO_CLIP, DataType.AUDIO_CLIP));
            StartCoroutine(LoadAddressableAssetsLabel(Constants.Label.LABEL_ITEM, DataType.INVENTORY_ITEM));


            //StartCoroutine(LoadAddressableAssetsLabel("Stage", DataType.STAGE));

            //StartCoroutine(LoadAddressableAssetsLabel("TextAsset", DataType.TEXT_ASSET));

            //wait until load complete
            while (loadCompleteCount < assetsToLoad)
            {

                yield return null;
            }


            LogDebug("Addressable Load Complete()");
            GameManager.Instance.OnAddreessableLoadComplete();
        }

        /// <summary>
        /// 6/13/2024-LYI
        /// ?? ???? ?? ???? ???????? ????
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        IEnumerator CacheResourceLocations(string label)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var location in handle.Result)
                {
                    string name = ExtractObjectName(location);
                    // ?????? ???????? ????
                    dic_assetLocation.Add(name, location);
                }
                Debug.Log("Resource locations cached successfully.");
            }
            else
            {
                Debug.LogError("Failed to load resource locations with label: " + label);
            }
        }
        private string ExtractObjectName(IResourceLocation location)
        {
            string path = location.PrimaryKey;
            string objectName = Path.GetFileNameWithoutExtension(path);
            return objectName;
        }




        #region Load Addressable Asset
        private IEnumerator LoadAddressableAssetsLabel(string label, DataType dataType)
        {
            //1. Label ???? ?????????? ???? ???? ????????
            //1. find all the locations with label

            LogDebug(dataType.ToString() + "LoadAddressableAsset: 1");
            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
            Addressables.LoadResourceLocationsAsync(label);

            //2. ???? ?????? ?? ???? ????
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 2");
            if (!locationsHandle.IsDone)
            {
                yield return locationsHandle;
            }

            //3. ?????????? ?????? ???? ?????? ?????? ????
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 3");
            List<AsyncOperationHandle> handleList = new List<AsyncOperationHandle>();

            //4. ?????? ???????? ???????? ?? ?????????? ???? ????
            foreach (IResourceLocation location in locationsHandle.Result)
            {
                AddHandleList(handleList, location, dataType);
            }

            //7. ?????? ???????? ???? ???? ?????????? ???? ????, ?????? ?? ???? ????
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 7");
            AsyncOperationHandle<IList<AsyncOperationHandle>> dropGroupOp = Addressables.ResourceManager.CreateGenericGroupOperation(handleList);

            if (!dropGroupOp.IsDone)
                yield return dropGroupOp;

            //8. ?????? ???????? Dictionary?? ?????????? ?????????? ???????????? ????????

            LogDebug(dataType.ToString() + "LoadAddressableAsset: 8");
            Addressables.Release(locationsHandle);


            LogDebug(dataType.ToString() + "LoadAddressableAsset: Done");
            //9. ?????? ?????? ????
            CheckDataLog(dataType);

            loadCompleteCount++;
        }

        private void LogDebug(string message)
        {
            if (isLogActive)
            {
                Debug.Log(message);
            }
        }

        void CheckDataLog(DataType dataType)
        {
            if (isLogActive == false)
            {
                return;
            }
            switch (dataType)
            {
                case DataType.AUDIO_CLIP:
                    foreach (var item in dic_audioClip)
                    {
                        Debug.Log(item.Key + " - " + item.Value.name);
                    }
                    break;
                case DataType.TEXT_ASSET:
                    foreach (var item in dic_textAsset)
                    {
                        Debug.Log(item.Key + " - " + item.Value.name);
                    }
                    break;
                case DataType.INVENTORY_ITEM:
                    foreach (var item in dic_inventoryItem)
                    {
                        Debug.Log(item.Key + " - " + item.Value.name);
                    }
                    break;
                default:
                    break;
            }
        }

        void AddHandleList(List<AsyncOperationHandle> handleList, IResourceLocation location, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.AUDIO_CLIP:
                    //if (AudioClipChecker(location.ToString()))
                    //{
                    //    Debug.Log("Location Data not figure this Type: " + location);
                    //    return;
                    //}
                    handleList.Add(AudioClipHandle(location));
                    break;
                case DataType.TEXT_ASSET:
                    handleList.Add(TextAssetHandle(location));
                    break;
                case DataType.INVENTORY_ITEM:
                    handleList.Add(InventoryItemHandle(location));
                    break;
                default:
                    Debug.Log("Please check DataType: " + dataType.ToString());
                    break;
            }
        }
        bool AudioClipChecker(string s)
        {
            if (s.EndsWith(".wav") || s.EndsWith(".mp4") || s.EndsWith(".mp3"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private AsyncOperationHandle<AudioClip> AudioClipHandle(IResourceLocation location)
        {
            //5. ?? ???? ?????? ???? ???????? ???? ????
            AsyncOperationHandle<AudioClip> loadAssetHandle
                = Addressables.LoadAssetAsync<AudioClip>(location);

            //6. ???????? ???? ???? ?? Dictionary?? ???? ???? ????, ?????????? ???? ???? ???? ???????? ????
            loadAssetHandle.Completed +=
              obj =>
              {
                  if (obj.Result != null)
                  {
                      if (!dic_audioClip.ContainsKey(obj.Result.name))
                      { dic_audioClip.Add(obj.Result.name, obj.Result); }
                  }
              };

            return loadAssetHandle;
        }
        private AsyncOperationHandle<TextAsset> TextAssetHandle(IResourceLocation location)
        {
            //5. ?? ???? ?????? ???? ???????? ???? ????
            AsyncOperationHandle<TextAsset> loadAssetHandle
                = Addressables.LoadAssetAsync<TextAsset>(location);

            //6. ???????? ???? ???? ?? Dictionary?? ???? ???? ????, ?????????? ???? ???? ???? ???????? ????
            loadAssetHandle.Completed +=
              obj =>
              {
                  if (obj.Result != null)
                  {
                      if (!dic_textAsset.ContainsKey(obj.Result.name))
                      { dic_textAsset.Add(obj.Result.name, obj.Result); }
                  }
              };

            return loadAssetHandle;
        }
        private AsyncOperationHandle<InventoryItem> InventoryItemHandle(IResourceLocation location)
        {
            //5. 각 파일 위치에 따른 오브젝트 로딩 시작
            AsyncOperationHandle<InventoryItem> loadAssetHandle
                = Addressables.LoadAssetAsync<InventoryItem>(location);

            //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
            loadAssetHandle.Completed +=
              obj =>
              {
                  if (obj.Result != null)
                  {
                      if (!dic_inventoryItem.ContainsKey(obj.Result.name))
                      { dic_inventoryItem.Add(obj.Result.ItemName, obj.Result); }
                  }
              };

            return loadAssetHandle;
        }
        #endregion
    }
}