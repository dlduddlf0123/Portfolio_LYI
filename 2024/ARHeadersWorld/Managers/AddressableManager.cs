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

    enum DataType
    {
        NONE = 0,
        INVENTORY_ITEM,
        AUDIO_CLIP,
        TEXT_ASSET,
        ROOM_PREFAB,
        STAGE,
        ENEMY,
        ENEMY_STATUS,
        PERK,
    }

    /// <summary>
    /// Addressable 에셋 로드, Dictionary 형식으로 저장, 데이터 홀더
    /// </summary>
    public class AddressableManager : MonoBehaviour
    {
        //public Dictionary<string, InventoryItem> dic_inventoryItem = new Dictionary<string, InventoryItem>();
        //public Dictionary<string, AudioClip> dic_audioClip = new Dictionary<string, AudioClip>();
        //public Dictionary<string, TextAsset> dic_textAsset = new Dictionary<string, TextAsset>();

        //public Dictionary<string, GameObject> dic_roomPrefab = new Dictionary<string, GameObject>();
        //public Dictionary<string, Enemy> dic_enemy = new Dictionary<string, Enemy>();
        //public Dictionary<string, TextAsset> dic_jsonEnemyStatus = new Dictionary<string, TextAsset>();

        //public Dictionary<string, TextAsset> dic_jsonStageData = new Dictionary<string, TextAsset>();
        //public Dictionary<string, Perk> dic_perk = new Dictionary<string, Perk>();

        //public Dictionary<string, Sprite> dic_sprite = new Dictionary<string, Sprite>(); //스프라이트 저장용, UI 등에 이용

        //public bool isLoadComplete = false;
        //bool isLogActive = false; //디버그용 변수

        //private void Start()
        //{
        //    if (GameManager.Instance.statGame == SceneStatus.MAIN ||
        //        GameManager.Instance.statGame == SceneStatus.PRACTICE &&
        //        !isLoadComplete)
        //    {
        //        StartCoroutine(LoadBurbirdAddressableAssets());
        //    }
        //}

        //private IEnumerator LoadBurbirdAddressableAssets()
        //{
        //    StartCoroutine(LoadAddressableAssetsLabel("AudioClip", DataType.AUDIO_CLIP));

        //    StartCoroutine(LoadAddressableAssetsLabel("TextAsset", DataType.TEXT_ASSET));
        //    StartCoroutine(LoadAddressableAssetsLabel("Stage", DataType.STAGE));
        //    StartCoroutine(LoadAddressableAssetsLabel("EnemyStatus", DataType.ENEMY_STATUS));
        //    StartCoroutine(LoadAddressableAssetsLabel("Perk", DataType.PERK));
        //    StartCoroutine(LoadAddressableAssetsLabel("Enemy", DataType.ENEMY));

        //    yield return StartCoroutine(LoadAddressableAssetsLabel("InventoryItem", DataType.INVENTORY_ITEM));

        //    OnAddressableLoadComplete();

        //    if (GameManager.Instance.statGame == SceneStatus.MAIN)
        //    {
        //        GameManager.Instance.OnMainScene();
        //    }
        //    if (GameManager.Instance.statGame == SceneStatus.PRACTICE)
        //    {
        //        GameManager.Instance.OnPracticeStart();
        //    }
        //    StaticManager.UI.MessageUI.PopupMessage("Addressable Load Complete");
        //}

        //public void AddressableLoadForBackend(AfterBackendLoadFunc afterBackendLoadFunc)
        //{
        //    string className = GetType().Name;
        //    string funcName = MethodBase.GetCurrentMethod()?.Name;

        //    //Addressable 가져오기
        //    StartCoroutine(LoadBurbirdAddressableAssetsBackend((isSuccess, errorInfo) => {
        //        afterBackendLoadFunc.Invoke(isSuccess, className, funcName, errorInfo);
        //    }));
        //}

        //public delegate void AfterGetFunc(bool isSuccess, string errorInfo);


        //private IEnumerator LoadBurbirdAddressableAssetsBackend(AfterGetFunc afterLoadingFunc)
        //{
        //    bool isSuccess = false;
        //    string errorInfo = string.Empty;
        //    try
        //    {
        //        //yield return StartCoroutine(LoadDropItemAddressableAssets());

        //        StartCoroutine(LoadAddressableAssetsLabel("AudioClip", DataType.AUDIO_CLIP));

        //        StartCoroutine(LoadAddressableAssetsLabel("TextAsset", DataType.TEXT_ASSET));
        //        StartCoroutine(LoadAddressableAssetsLabel("Stage", DataType.STAGE));
        //        StartCoroutine(LoadAddressableAssetsLabel("EnemyStatus", DataType.ENEMY_STATUS));
        //        StartCoroutine(LoadAddressableAssetsLabel("Perk", DataType.PERK));
        //        StartCoroutine(LoadAddressableAssetsLabel("Enemy", DataType.ENEMY));

        //        isSuccess = true;
        //    }
        //    catch (Exception e)
        //    {
        //        errorInfo = e.ToString();
        //    }
        //    finally
        //    {
        //        afterLoadingFunc.Invoke(isSuccess, errorInfo);
        //    }

        //    yield return StartCoroutine(LoadAddressableAssetsLabel("InventoryItem", DataType.INVENTORY_ITEM));

        //    OnAddressableLoadComplete();
        //}

        //public void OnAddressableLoadComplete()
        //{
        //    LogDebug("Addressable Load Complete()");
        //    isLoadComplete = true;

        //    GameManager.Instance.dataMgr.DataManagerInit();
        //    GameManager.Instance.invenMgr.LoadInventoryDatasAfterAddressable();
        //}


        ///// <summary>
        ///// 방 정보 불러오기
        ///// </summary>
        ///// <param name="stageNum"></param>
        //public IEnumerator LoadRoomAssets(int stageNum)
        //{
        //    LogDebug("LoadRoomAssets()");
        //    dic_roomPrefab.Clear();
        //    yield return StartCoroutine(LoadRoomPrefabAddressableAssets(stageNum));
        //}

        //IEnumerator LoadRoomPrefabAddressableAssets(int stageNum)
        //{
        //    yield return StartCoroutine(LoadAddressableAssetsLabel("Stage" + stageNum, DataType.ROOM_PREFAB));
        //}
        //public Dictionary<string, GameObject> GetRoomData()
        //{
        //    return dic_roomPrefab;
        //}

        //#region Load Addressable Asset
        //private IEnumerator LoadAddressableAssetsLabel(string label, DataType dataType)
        //{
        //    //1. Label 기준 어드레서블 에셋 위치 불러오기
        //    //1. find all the locations with label

        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: 1");
        //    AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
        //    Addressables.LoadResourceLocationsAsync(label);

        //    //2. 위치 불러올 때 까지 대기
        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: 2");
        //    if (!locationsHandle.IsDone)
        //    {
        //        yield return locationsHandle;
        //    }

        //    //3. 오브젝트를 불러올 핸들 저장용 리스트 선언
        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: 3");
        //    List<AsyncOperationHandle> handleList = new List<AsyncOperationHandle>();

        //    //4. 불러온 로케이션 결과에서 각 로케이션에 대한 처리
        //    foreach (IResourceLocation location in locationsHandle.Result)
        //    {
        //        AddHandleList(handleList, location, dataType);
        //    }

        //    //7. 한번에 대기하기 위한 그룹 오퍼레이션 관리 생성, 완료될 때 까지 대기
        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: 7");
        //    AsyncOperationHandle<IList<AsyncOperationHandle>> dropGroupOp = Addressables.ResourceManager.CreateGenericGroupOperation(handleList);

        //    if (!dropGroupOp.IsDone)
        //        yield return dropGroupOp;

        //    //8. 불러온 에셋들을 Dictionary에 저장했으니 메모리에서 어드레서블을 해제한다

        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: 8");
        //    Addressables.Release(locationsHandle);


        //    LogDebug(dataType.ToString() + "LoadAddressableAsset: Done");
        //    //9. 로그로 데이터 확인
        //    CheckDataLog(dataType);
        //}

        //private void LogDebug(string message)
        //{
        //    if (true)
        //    {
        //        Debug.Log(message);
        //    }
        //}

        //void CheckDataLog(DataType dataType)
        //{
        //    if (isLogActive == false)
        //    {
        //        return;
        //    }
        //    switch (dataType)
        //    {
        //        case DataType.INVENTORY_ITEM:
        //            foreach (var item in dic_inventoryItem)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.AUDIO_CLIP:
        //            foreach (var item in dic_audioClip)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.TEXT_ASSET:
        //            foreach (var item in dic_textAsset)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.STAGE:
        //            foreach (var item in dic_jsonStageData)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.ENEMY:
        //            foreach (var item in dic_enemy)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.ENEMY_STATUS:
        //            foreach (var item in dic_jsonEnemyStatus)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        case DataType.PERK:
        //            foreach (var item in dic_perk)
        //            {
        //                Debug.Log(item.Key + " - " + item.Value.name);
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //void AddHandleList(List<AsyncOperationHandle> handleList, IResourceLocation location, DataType dataType)
        //{
        //    switch (dataType)
        //    {
        //        case DataType.INVENTORY_ITEM:
        //            handleList.Add(InventoryItemHandle(location));
        //            break;
        //        case DataType.AUDIO_CLIP:
        //            //if (AudioClipChecker(location.ToString()))
        //            //{
        //            //    Debug.Log("Location Data not figure this Type: " + location);
        //            //    return;
        //            //}
        //            handleList.Add(AudioClipHandle(location));
        //            break;
        //        case DataType.TEXT_ASSET:
        //            handleList.Add(TextAssetHandle(location));
        //            break;
        //        case DataType.ROOM_PREFAB:
        //            handleList.Add(RoomPrefabHandle(location));
        //            break;
        //        case DataType.STAGE:
        //            handleList.Add(JsonStageDataHandle(location));
        //            break;
        //        case DataType.ENEMY:
        //            handleList.Add(EnemyHandle(location));
        //            break;
        //        case DataType.ENEMY_STATUS:
        //            handleList.Add(JsonEnemyStatusHandle(location));
        //            break;
        //        case DataType.PERK:
        //            handleList.Add(PerkHandle(location));
        //            break;
        //        default:
        //            Debug.Log("Please check DataType: " + dataType.ToString());
        //            break;
        //    }
        //}
        //bool AudioClipChecker(string s)
        //{
        //    if (s.EndsWith(".wav") || s.EndsWith(".mp4") || s.EndsWith(".mp3"))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //private AsyncOperationHandle<InventoryItem> InventoryItemHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<InventoryItem> loadAssetHandle
        //        = Addressables.LoadAssetAsync<InventoryItem>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_inventoryItem.ContainsKey(obj.Result.name))
        //          {
        //              dic_inventoryItem.Add(obj.Result.ItemName, obj.Result);
        //          }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<AudioClip> AudioClipHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<AudioClip> loadAssetHandle
        //        = Addressables.LoadAssetAsync<AudioClip>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_audioClip.ContainsKey(obj.Result.name))
        //          { dic_audioClip.Add(obj.Result.name, obj.Result); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<TextAsset> TextAssetHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<TextAsset> loadAssetHandle
        //        = Addressables.LoadAssetAsync<TextAsset>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_textAsset.ContainsKey(obj.Result.name))
        //          { dic_textAsset.Add(obj.Result.name, obj.Result); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<GameObject> RoomPrefabHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<GameObject> loadAssetHandle
        //        = Addressables.LoadAssetAsync<GameObject>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_roomPrefab.ContainsKey(obj.Result.name))
        //          { dic_roomPrefab.Add(obj.Result.name, obj.Result); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<TextAsset> JsonStageDataHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<TextAsset> loadAssetHandle
        //        = Addressables.LoadAssetAsync<TextAsset>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_jsonStageData.ContainsKey(obj.Result.name))
        //          { dic_jsonStageData.Add(obj.Result.name, obj.Result); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<GameObject> EnemyHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<GameObject> loadAssetHandle
        //        = Addressables.LoadAssetAsync<GameObject>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_enemy.ContainsKey(obj.Result.name))
        //          { dic_enemy.Add(obj.Result.name, obj.Result.GetComponent<Enemy>()); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<TextAsset> JsonEnemyStatusHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<TextAsset> loadAssetHandle
        //        = Addressables.LoadAssetAsync<TextAsset>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_jsonEnemyStatus.ContainsKey(obj.Result.name))
        //          { dic_jsonEnemyStatus.Add(obj.Result.name, obj.Result); }
        //      };

        //    return loadAssetHandle;
        //}
        //private AsyncOperationHandle<GameObject> PerkHandle(IResourceLocation location)
        //{
        //    //5. 각 파일 위치에 따른 오브젝트 로딩 시작
        //    AsyncOperationHandle<GameObject> loadAssetHandle
        //        = Addressables.LoadAssetAsync<GameObject>(location);

        //    //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
        //    loadAssetHandle.Completed +=
        //      obj =>
        //      {
        //          if (!dic_perk.ContainsKey(obj.Result.name))
        //          {
        //              dic_perk.Add(obj.Result.name, obj.Result.GetComponent<Perk>());
        //          }
        //      };

        //    return loadAssetHandle;
        //}

        //#endregion
    }
