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


namespace AroundEffect
{

    enum DataType
    {
        NONE = 0,
        AUDIO_CLIP
    }

    /// <summary>
    /// Addressable 에셋 로드, Dictionary 형식으로 저장, 데이터 홀더
    /// </summary>
    public class AddressableManager : MonoBehaviour
    {
        public Dictionary<string, AudioClip> dic_audioClip = new();

        public bool isLoadComplete = false;

        [Header("Debug")]
        public bool isStartDebug = false;
        public bool isLogActive = false; //디버그용 변수

        public int assetsToLoad = 0;
        public int loadCompleteCount = 0;



        private void Start()
        {
            if (!isLoadComplete)
            {
                StartCoroutine(LoadAddressableAssets());
            }
        }

        private IEnumerator LoadAddressableAssets()
        {
            assetsToLoad = 1;
            StartCoroutine(LoadAddressableAssetsLabel(Constants.Label.LABEL_AUDIO_CLIP, DataType.AUDIO_CLIP));


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


        #region Load Addressable Asset

        private IEnumerator LoadAddressableAssetsLabel(string label, DataType dataType)
        {
            //1. Label 기준 어드레서블 에셋 위치 불러오기
            //1. find all the locations with label

            LogDebug(dataType.ToString() + "LoadAddressableAsset: 1");
            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
            Addressables.LoadResourceLocationsAsync(label);

            //2. 위치 불러올 때 까지 대기
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 2");
            if (!locationsHandle.IsDone)
            {
                yield return locationsHandle;
            }

            //3. 오브젝트를 불러올 핸들 저장용 리스트 선언
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 3");
            List<AsyncOperationHandle> handleList = new List<AsyncOperationHandle>();

            //4. 불러온 로케이션 결과에서 각 로케이션에 대한 처리
            foreach (IResourceLocation location in locationsHandle.Result)
            {
                AddHandleList(handleList, location, dataType);
            }

            //7. 한번에 대기하기 위한 그룹 오퍼레이션 관리 생성, 완료될 때 까지 대기
            LogDebug(dataType.ToString() + "LoadAddressableAsset: 7");
            AsyncOperationHandle<IList<AsyncOperationHandle>> dropGroupOp = Addressables.ResourceManager.CreateGenericGroupOperation(handleList);

            if (!dropGroupOp.IsDone)
                yield return dropGroupOp;

            //8. 불러온 에셋들을 Dictionary에 저장했으니 메모리에서 어드레서블을 해제한다

            LogDebug(dataType.ToString() + "LoadAddressableAsset: 8");
            Addressables.Release(locationsHandle);


            LogDebug(dataType.ToString() + "LoadAddressableAsset: Done");
            //9. 로그로 데이터 확인
            CheckDataLog(dataType);

            loadCompleteCount++;
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
                //case DataType.TEXT_ASSET:
                //    handleList.Add(TextAssetHandle(location));
                //    break;
                //case DataType.STAGE:
                //    handleList.Add(StagePrefabHandle(location));
                //    break;
                default:
                    Debug.Log("Please check DataType: " + dataType.ToString());
                    break;
            }
        }

        private AsyncOperationHandle<AudioClip> AudioClipHandle(IResourceLocation location)
        {
            //5. 각 파일 위치에 따른 오브젝트 로딩 시작
            AsyncOperationHandle<AudioClip> loadAssetHandle
                = Addressables.LoadAssetAsync<AudioClip>(location);

            //6. 오브젝트 로딩 완료 시 Dictionary에 해당 파일 등록, 로드했다는 핸들 정보 또한 리스트에 저장
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
}