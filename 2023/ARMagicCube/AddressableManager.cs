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
using System.Linq;

using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.AddressableAssets.ResourceLocators;

enum DataType
    {
        NONE = 0,
        MAGIC_CUBE,
        IMAGE_LIBRARY,
    }

/// <summary>
/// Addressable 에셋 로드, Dictionary 형식으로 저장, 데이터 홀더
/// </summary>
public class AddressableManager : MonoBehaviour
{
    public Dictionary<string, List<GameObject>> dic__allMagicCube = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, XRReferenceImageLibrary> dic_allLibrary = new Dictionary<string, XRReferenceImageLibrary>();


    public Dictionary<LibraryType, long> dic_downloadSize = new Dictionary<LibraryType, long>();
    public List<LibraryType> list_loadedLibrary = new List<LibraryType>();

    public bool isLoadComplete = false;
    bool isLogActive = false; //디버그용 변수

    private void Start()
    {
        //if (GameManager.Instance.statGame == SceneStatus.MAIN ||
        //    GameManager.Instance.statGame == SceneStatus.PRACTICE &&
        //    !isLoadComplete)
        //{
        //    StartCoroutine(LoadBurbirdAddressableAssets());
        //}

       //GameManager.Instance.ui_librarySelect.ChangeAddressableStatus(false);
        //StartCoroutine(LoadMagicCubeAssets());
    }

    /// <summary>
    /// 3/18/2024-LYI
    /// 다운로드 된 에셋 호출 시 실행
    /// 에셋에서 프리팹 생성
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    /// <returns></returns>
   public IEnumerator LoadMagicCubeAssets(LibraryType type, UnityAction action = null)
    {
        yield return StartCoroutine(LoadAddressableAssetsMagicCube(type.ToString(), DataType.MAGIC_CUBE));

        list_loadedLibrary.Add(type);
        if (action != null)
        {
            action.Invoke();
        }
    }


    public void OnAddressableLoadComplete()
    {
        LogDebug("Addressable Load Complete()");
        isLoadComplete = true;

        GameManager.Instance.libraryHeader.Init();
      //  GameManager.Instance.ui_librarySelect.ChangeAddressableStatus(true);

    }


    #region Load Addressable Asset

    /// <summary>
    /// 3/14/2024-LYI
    /// 각 라벨 에셋 존재 여부 체크
    /// 이후 해당 버튼 상태 변경
    /// 다운로드상태를 캐싱할 것이 아니라 에셋이 보유되어있는지 체크하도록 변경
    /// 내부 데이터 로컬 파일 위치를 찾지 못해 결국 다운로드 사이즈로 대체
    /// </summary>
    /// <param name="button"></param>
    public async void CheckLabelStatus(UI_EpisodeButton button)
    {
        string label = button.libraryType.ToString();

        long downloadSize = 0;

        //Dictionary에 이미 있으면 돌려보내기
        if (dic_downloadSize.ContainsKey(button.libraryType))
        {
            downloadSize = dic_downloadSize[button.libraryType];
            Debug.Log(label + " 이미 용량이 계산되었습니다: " + downloadSize);
            return;
        }
        
        
        AsyncOperationHandle<long> operationHandle = Addressables.GetDownloadSizeAsync(label);

        await operationHandle.Task;

        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            downloadSize = operationHandle.Result;
            dic_downloadSize.Add(button.libraryType, downloadSize);


            //이미 다운로드 완료된 상태
            if (downloadSize == 0)
            {
                Debug.Log(label + " 로컬 파일이 있습니다.");
                button.ChangeButtonStatus(ButtonStatus.OPEN);
            }
            else
            {
                Debug.Log(label + " 용량 계산 완료되었습니다.");
                Debug.Log(label.ToString() + "DownloadSize: " + (int)(downloadSize / 1048576) + " MB");
                button.ChangeButtonStatus(ButtonStatus.LOCK);
            }
        }
        else if (operationHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log(label + " 용량 계산 실패했습니다: " + operationHandle.OperationException.Message);
            button.ChangeButtonStatus(ButtonStatus.LOCK);
        }


        Addressables.Release(operationHandle);

        isLoadComplete = true;

        //다운로드 완료되면 경고창 출력
        GameManager.Instance.ui_warning.Open();
    }



    /// <summary>
    /// 3/14/2024-LYI
    /// Download size 체크 함수
    /// </summary>
    public void CheckDownloadSize(LibraryType type, UnityAction<int> action = null)
    {
        long downloadSize = 0;
        int MBsize = 0;

        //Dictionary에 이미 있으면 돌려보내기
        if (dic_downloadSize.ContainsKey(type))
        {
            downloadSize = dic_downloadSize[type];
            //이미 다운로드가 완료된 경우
            if (downloadSize == 0)
            {
                return;
            }

            MBsize = (int)(downloadSize / 1048576);

            if (action != null)
            {
                action.Invoke(MBsize);
            }
            return;
        }

        string label = type.ToString();

        Addressables.GetDownloadSizeAsync(label).Completed +=
           (AsyncOperationHandle<long> operationHandle) =>
           {
               if (operationHandle.Status == AsyncOperationStatus.Succeeded)
               {
                   downloadSize = operationHandle.Result;
                   MBsize = (int)(downloadSize / 1048576);
                   dic_downloadSize.Add(type, downloadSize);

                   Debug.Log(label + " 용량 계산 완료되었습니다.");
                   Debug.Log(label.ToString() + "DownloadSize: " + downloadSize);

                   if (action != null)
                   {
                       action.Invoke(MBsize);
                   }

                   Addressables.Release(operationHandle);
               }
               else if (operationHandle.Status == AsyncOperationStatus.Failed)
               {
                   Debug.Log(label + " 용량 계산 실패했습니다: " + operationHandle.OperationException.Message);
               }
           };
    }


    /// <summary>
    /// 3/14/2024-LYI
    /// 다운로드 진행
    /// EpisodeButton에서 호출하나 Addressable을 사용하므로 매니저에 만듦
    /// </summary>
    /// <param name="button">이 함수를 실행시킨 버튼</param>
    public void BundleDownLoad(UI_EpisodeButton button = null)
    {
        string label = button.libraryType.ToString();

        long downloadSize = 0;

        button.ChangeButtonStatus(ButtonStatus.LOADING);

        //Dictionary에 이미 있으면 스킵
        if (dic_downloadSize.ContainsKey(button.libraryType))
        {
            downloadSize = dic_downloadSize[button.libraryType];
            StartCoroutine(DownloadProcess(button, downloadSize));
        }
        else
        {
            //없으면 로딩 상태로 전환 후 계산 시작
            Addressables.GetDownloadSizeAsync(label).Completed +=
                (AsyncOperationHandle<long> operationHandle) =>
                {
                    if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        downloadSize = operationHandle.Result;
                        dic_downloadSize.Add(button.libraryType, downloadSize);

                        StartCoroutine(DownloadProcess(button, downloadSize));

                        Addressables.Release(operationHandle);

                        Debug.Log(label + " 용량 계산 완료되었습니다.");
                        Debug.Log(label.ToString() + "DownloadSize: " + downloadSize);
                    }
                    else if (operationHandle.Status == AsyncOperationStatus.Failed)
                    {
                        Debug.Log(label + " 용량 계산 실패했습니다: " + operationHandle.OperationException.Message);
                        Addressables.Release(operationHandle);
                    }
                };
        }
    }

    /// <summary>
    /// 3/18/2024-LYI
    /// 에셋 다운로드 진행
    /// 다운로드 중 percent 표시
    /// </summary>
    /// <param name="button"></param>
    /// <param name="downloadSize"></param>
    /// <returns></returns>
    public IEnumerator DownloadProcess(UI_EpisodeButton button, long downloadSize)
    {
        string label = button.libraryType.ToString();

        // 다운로드 크기가 0보다 큰 경우, 다운로드가 필요함
        if (downloadSize > 0)
        {
            float progress = 0f;
            // 다운로드 진행 상황 모니터링을 위한 핸들 생성
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label);
            button.ChangeButtonStatus(ButtonStatus.DOWNLOAD);

            //다운로드 도중인 경우
            while (!downloadHandle.IsDone)
            {
                //버튼에서 퍼센트 UI 표시
                progress = downloadHandle.GetDownloadStatus().Percent;
                button.ChangeDownloadPercent(progress);
                yield return null;
            }

            //완료 시 퍼센트 100%
            button.ChangeDownloadPercent(1);

            //완료 이후 처리
            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log(label + " 다운로드가 완료되었습니다.");
                button.ChangeButtonStatus(ButtonStatus.OPEN);
                dic_downloadSize[button.libraryType] = 0;
            }
            else if (downloadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.Log(label + " 다운로드가 실패했습니다: " + downloadHandle.OperationException.Message);
                button.ChangeButtonStatus(ButtonStatus.LOCK);
            }
            
            //핸들 메모리 해제
            Addressables.Release(downloadHandle);
        }
        else
        {
            // 다운로드 크기가 0이면 이미 다운로드된 상태이므로 패스
            Debug.Log(label + " 이미 다운로드된 상태입니다.");
            button.ChangeButtonStatus(ButtonStatus.OPEN);
            yield break;
        }

    }
    public IEnumerator DownloadProcess(LibraryType type, long downloadSize, UnityAction onDownloadComplete =null)
    {
        string label = type.ToString();

        // 다운로드 크기가 0보다 큰 경우, 다운로드가 필요함
        if (downloadSize > 0)
        {
            float progress = 0f;
            // 다운로드 진행 상황 모니터링을 위한 핸들 생성
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(label);
           // button.ChangeButtonStatus(ButtonStatus.DOWNLOAD);

            //다운로드 도중인 경우
            while (!downloadHandle.IsDone)
            {
                //버튼에서 퍼센트 UI 표시
                progress = downloadHandle.GetDownloadStatus().Percent;
               // button.ChangeDownloadPercent(progress);
                yield return null;
            }

            //완료 시 퍼센트 100%
           // button.ChangeDownloadPercent(1);

            //완료 이후 처리
            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log(label + " 다운로드가 완료되었습니다.");
              //  button.ChangeButtonStatus(ButtonStatus.OPEN);
                dic_downloadSize[type] = 0;
            }
            else if (downloadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.Log(label + " 다운로드가 실패했습니다: " + downloadHandle.OperationException.Message);
               // button.ChangeButtonStatus(ButtonStatus.LOCK);
            }

            //핸들 메모리 해제
            Addressables.Release(downloadHandle);
        }
        else
        {
            // 다운로드 크기가 0이면 이미 다운로드된 상태이므로 패스
            Debug.Log(label + " 이미 다운로드된 상태입니다.");
           // button.ChangeButtonStatus(ButtonStatus.OPEN);
            yield break;
        }

    }

    /// <summary>
    /// 3/18/2024-LYI
    /// Label 기반 프리팹 생성 진행
    /// Dictionary에 저장
    /// </summary>
    /// <param name="label"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    private IEnumerator LoadAddressableAssetsMagicCube(string label, DataType dataType)
    {
        // 1. Load all the locations with the specified label
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 1");
        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label);

        // 2. Wait until the locations are loaded
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 2");
        yield return locationsHandle;

        // 3. Declare a list to store the loading handles
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 3");
        List<AsyncOperationHandle<GameObject>> handleList = new List<AsyncOperationHandle<GameObject>>();

        // 4. Load objects for each location
        foreach (IResourceLocation location in locationsHandle.Result)
        {
            // 5. Load objects asynchronously
            AsyncOperationHandle<GameObject> loadAssetHandle = Addressables.LoadAssetAsync<GameObject>(location);

            // 6. Register the loaded object to the dictionary
            loadAssetHandle.Completed += loadOperationHandle =>
            {
                if (loadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject go = loadOperationHandle.Result;

                    string assetPath = location.PrimaryKey;
                    string[] pathParts = assetPath.Split('/');
                    string groupName = pathParts.Length >= 2 ? pathParts[2] : "Unknown Group";
                    //string groupName = location.ResourceType.ToString();

                    if (!dic__allMagicCube.ContainsKey(groupName))
                    {
                        dic__allMagicCube[groupName] = new List<GameObject>();
                    }

                    dic__allMagicCube[groupName].Add(go);
                    dic__allMagicCube[groupName].Sort((a, b) => a.name.CompareTo(b.name));
                }
            };

            handleList.Add(loadAssetHandle);
        }


        // 7. Wait for all objects to finish loading
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 7");
        yield return new WaitUntil(() => handleList.All(handle => handle.IsDone));

        // 8. Release the locations
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 8");
        Addressables.Release(locationsHandle);

        LogDebug(dataType.ToString() + "LoadAddressableAsset: Done");

        // 9. Log and check the loaded data
        CheckDataLog(dataType);
    }
    private IEnumerator LoadAddressableAssetsXRLibrary(string label, DataType dataType)
    {
        // 1. Load all the locations with the specified label
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 1");
        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(label);

        // 2. Wait until the locations are loaded
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 2");
        yield return locationsHandle;

        // 3. Declare a list to store the loading handles
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 3");
        List<AsyncOperationHandle<XRReferenceImageLibrary>> handleList = new List<AsyncOperationHandle<XRReferenceImageLibrary>>();

        // 4. Load objects for each location
        foreach (IResourceLocation location in locationsHandle.Result)
        {
            // 5. Load objects asynchronously
            AsyncOperationHandle<XRReferenceImageLibrary> loadAssetHandle = Addressables.LoadAssetAsync<XRReferenceImageLibrary>(location);

            // 6. Register the loaded object to the dictionary
            loadAssetHandle.Completed += loadOperationHandle =>
            {
                string assetPath = location.PrimaryKey;
                string[] pathParts = assetPath.Split('/');
                string groupName = pathParts.Length >= 2 ? pathParts[2] : "Unknown Group";

                if (!dic_allLibrary.ContainsKey(groupName))
                {
                    if (loadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        dic_allLibrary.Add(groupName, loadOperationHandle.Result);
                    }
                }
            };

            handleList.Add(loadAssetHandle);
        }


        // 7. Wait for all objects to finish loading
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 7");
        yield return new WaitUntil(() => handleList.All(handle => handle.IsDone));

        // 8. Release the locations
        LogDebug(dataType.ToString() + "LoadAddressableAsset: 8");
        Addressables.Release(locationsHandle);

        LogDebug(dataType.ToString() + "LoadAddressableAsset: Done");

        // 9. Log and check the loaded data
        CheckDataLog(dataType);
    }


    private void LogDebug(string message)
    {
        if (true)
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
            //case DataType.AUDIO_CLIP:
            //    foreach (var item in dic_audioClip)
            //    {
            //        Debug.Log(item.Key + " - " + item.Value.name);
            //    }
            default:
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

    
    #endregion
}