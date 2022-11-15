using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    public partial class AssetBundleManager : MonoBehaviour
    {
        public enum LoadMode
        {
            None,
            FromServerUrl,
            FromLocalPath,
        }

        public enum LoadState
        {
            None,
            LoadManifest,
            LoadAssetBundles,
            Done,
        }

        [Serializable]
        public struct AssetBundleSetting
        {
            public string overrideServerUrl;
            public string overrideLocalFolderPath;
            public LoadMode overrideLoadMode;
            public string platformFolderName;
        }

        [SerializeField]
        public struct AssetBundleInfo
        {
            public string url;
            public Hash128 hash;
            public bool cached;
        }

        public static AssetBundleManager Singleton { get; private set; }

        [SerializeField]
        protected string serverUrl = "http://localhost/AssetBundles";
        [SerializeField]
        protected string localFolderPath = "AssetBundles/";
        [SerializeField]
        protected string initSceneName = "init";
        [SerializeField]
        protected float delayBeforeLoadInitScene = 0.5f;
        [SerializeField]
        protected LoadMode loadMode = LoadMode.FromServerUrl;
        [SerializeField]
        protected LoadMode editorLoadMode = LoadMode.FromLocalPath;
        [SerializeField]
        protected bool loadAssetBundleOnStart = true;
        [SerializeField]
        protected AssetBundleSetting androidSetting = new AssetBundleSetting()
        {
            platformFolderName = "Android",
        };
        [SerializeField]
        protected AssetBundleSetting iosSetting = new AssetBundleSetting()
        {
            platformFolderName = "iOS",
        };
        [SerializeField]
        protected AssetBundleSetting windowsSetting = new AssetBundleSetting()
        {
            platformFolderName = "StandaloneWindows64",
        };
        [SerializeField]
        protected AssetBundleSetting osxSetting = new AssetBundleSetting()
        {
            platformFolderName = "StandaloneOSXIntel64",
        };
        [SerializeField]
        protected AssetBundleSetting linuxSetting = new AssetBundleSetting()
        {
            platformFolderName = "StandaloneLinux64",
        };
        [SerializeField]
        protected AssetBundleSetting serverSetting = new AssetBundleSetting()
        {
            platformFolderName = "StandaloneLinux64",
            overrideLoadMode = LoadMode.FromLocalPath,
        };
        public UnityEvent onManifestLoaded = new UnityEvent();
        public UnityEvent onManifestLoadedFail = new UnityEvent();
        public UnityEvent onAssetBundlesLoaded = new UnityEvent();
        public UnityEvent onAssetBundlesLoadedFail = new UnityEvent();
        [SerializeField]
        protected bool clearCache = false;

        public LoadMode CurrentLoadMode
        {
            get
            {
                LoadMode currentLoadMode = loadMode;
                if (Application.isEditor)
                    currentLoadMode = editorLoadMode;
                else if (CurrentSetting.overrideLoadMode != LoadMode.None)
                    currentLoadMode = CurrentSetting.overrideLoadMode;
                return currentLoadMode;
            }
        }
        public LoadState CurrentLoadState { get; protected set; } = LoadState.None;
        public int LoadingAssetBundlesCount { get; protected set; } = 0;
        public int LoadingAssetBundlesFromCacheCount { get; protected set; } = 0;
        public int LoadedAssetBundlesCount { get; protected set; } = 0;
        public int LoadedAssetBundlesFromCacheCount { get; protected set; } = 0;
        public long LoadingAssetBundleFileSize { get; protected set; } = 0;
        public double LoadingSpeedPerSeconds { get; protected set; } = 0;
        public double LoadingRemainingSeconds { get; protected set; } = 0;
        public float TotalLoadProgress { get { return LoadedAssetBundlesCount == LoadingAssetBundlesCount ? 1f : ((float)LoadedAssetBundlesCount / (float)LoadingAssetBundlesCount); } }
        public string LoadingAssetBundleFileName { get; protected set; }
        public string LoadingAssetBundleFromCacheFileName { get; protected set; }
        public AssetBundleSetting CurrentSetting { get; protected set; }
        public string ServerUrl { get { return !string.IsNullOrEmpty(CurrentSetting.overrideServerUrl) ? CurrentSetting.overrideServerUrl : serverUrl; } }
        public string LocalFolderPath { get { return !string.IsNullOrEmpty(CurrentSetting.overrideLocalFolderPath) ? CurrentSetting.overrideLocalFolderPath : localFolderPath; } }
        public Dictionary<string, AssetBundle> Dependencies { get; private set; } = new Dictionary<string, AssetBundle>();
        public UnityWebRequest CurrentWebRequest { get; protected set; }

        private bool tempErrorOccuring = false;
        private AssetBundle tempAssetBundle = null;
        private readonly Dictionary<string, AssetBundleInfo> loadingAssetBundles = new Dictionary<string, AssetBundleInfo>();
        private float downloadProgressFromLastOneSeconds = 0f;
        private float sumDeltaProgress = 0f;
        private float oneSecondsCounter = 0f;
        private float secondsCount = 0;

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                CurrentSetting = serverSetting;
            }
            else
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        CurrentSetting = androidSetting;
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        CurrentSetting = iosSetting;
                        break;
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                        CurrentSetting = windowsSetting;
                        break;
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                        CurrentSetting = osxSetting;
                        break;
                    case RuntimePlatform.LinuxEditor:
                    case RuntimePlatform.LinuxPlayer:
                        CurrentSetting = linuxSetting;
                        break;
                }
            }
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (clearCache)
                Caching.ClearCache();
#endif
            if (loadAssetBundleOnStart)
                LoadAssetBundle();
        }

        private void Update()
        {
            if (CurrentLoadState != LoadState.LoadAssetBundles)
                return;
            if (string.IsNullOrEmpty(LoadingAssetBundleFileName))
                return;
            if (CurrentWebRequest == null || CurrentWebRequest.isDone)
                return;
            oneSecondsCounter += Time.deltaTime;
            if (oneSecondsCounter >= 1f)
            {
                secondsCount += oneSecondsCounter;
                oneSecondsCounter = 0f;
                float deltaProgress = CurrentWebRequest.downloadProgress - downloadProgressFromLastOneSeconds;
                sumDeltaProgress += deltaProgress;
                LoadingSpeedPerSeconds = (sumDeltaProgress / secondsCount) * LoadingAssetBundleFileSize;
                LoadingRemainingSeconds = (1 - CurrentWebRequest.downloadProgress) / (sumDeltaProgress / secondsCount);
                downloadProgressFromLastOneSeconds = CurrentWebRequest.downloadProgress;
            }
        }

        public void LoadAssetBundle()
        {
            switch (CurrentLoadMode)
            {
                case LoadMode.FromServerUrl:
                    LoadAssetBundleFromServer();
                    break;
                case LoadMode.FromLocalPath:
                    LoadAssetBundleFromLocalFolder();
                    break;
            }
        }

        public void LoadAssetBundleFromServer()
        {
            StartCoroutine(LoadAssetBundlesFromUrlRoutine(ServerUrl));
        }

        public void LoadAssetBundleFromLocalFolder()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    StartCoroutine(LoadAssetBundlesFromUrlRoutine($"file:///{Path.GetFullPath(".")}/{LocalFolderPath}"));
                    break;
                default:
                    StartCoroutine(LoadAssetBundlesFromUrlRoutine($"{Path.GetFullPath(".")}/{LocalFolderPath}"));
                    break;
            }
        }

        private bool IsWebRequestLoadedFail(string key, UnityEvent evt)
        {
            if (WebRequestIsError(CurrentWebRequest))
            {
                OnAssetBundleLoadedFail(key, evt);
                return true;
            }
            return false;
        }

        public bool WebRequestIsError(UnityWebRequest unityWebRequest)
        {
#if UNITY_2020_2_OR_NEWER
            UnityWebRequest.Result result = unityWebRequest.result;
            return (result == UnityWebRequest.Result.ConnectionError)
                || (result == UnityWebRequest.Result.DataProcessingError)
                || (result == UnityWebRequest.Result.ProtocolError);
#else
            return unityWebRequest.isHttpError || unityWebRequest.isNetworkError;
#endif
        }

        private void OnAssetBundleLoadedFail(string key, UnityEvent evt, string error = "")
        {
            tempErrorOccuring = true;
            evt.Invoke();
            CurrentLoadState = LoadState.None;
            string logError = error;
            if (!string.IsNullOrEmpty(CurrentWebRequest.error))
                logError = CurrentWebRequest.error;
            Debug.LogError($"[AssetBundleManager] Load {key} from {CurrentWebRequest.url}, error: {logError}");
        }

        private IEnumerator GetAssetBundleSize(string url, string loadKey)
        {
            UnityWebRequest webRequest = UnityWebRequest.Head(url);
            yield return webRequest.SendWebRequest();
            string contentLength = webRequest.GetResponseHeader("Content-Length");
            if (WebRequestIsError(webRequest))
                LoadingAssetBundleFileSize = -1;
            else
                LoadingAssetBundleFileSize = Convert.ToInt64(contentLength);
            Debug.Log($"[AssetBundleManager] Get Asset Bundle Size {loadKey} Content-Length: {LoadingAssetBundleFileSize}");
        }

        private IEnumerator LoadAssetBundleFromUrlRoutine(string url, string loadKey, UnityEvent successEvt, UnityEvent errorEvt, Hash128? hash)
        {
            Debug.Log($"[AssetBundleManager] Load {loadKey}");
            // Get asset bundle size
            yield return StartCoroutine(GetAssetBundleSize(url, loadKey));
            // Create request to get asset bundle from cache or download from server
            if (hash.HasValue)
                CurrentWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash.Value);
            else
                CurrentWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            downloadProgressFromLastOneSeconds = 0f;
            sumDeltaProgress = 0f;
            oneSecondsCounter = 0f;
            secondsCount = 0;
            LoadingSpeedPerSeconds = 0f;
            LoadingRemainingSeconds = float.PositiveInfinity;
            // Send request
            yield return CurrentWebRequest.SendWebRequest();
            // Error handling
            if (IsWebRequestLoadedFail(loadKey, errorEvt))
                yield break;
            tempAssetBundle = DownloadHandlerAssetBundle.GetContent(CurrentWebRequest);
            if (tempAssetBundle == null)
            {
                OnAssetBundleLoadedFail(loadKey, errorEvt, "No Asset Bundle");
                yield break;
            }
            // Done
            successEvt.Invoke();
            Debug.Log($"[AssetBundleManager] Load {loadKey} done");
        }

        private IEnumerator LoadAssetBundlesFromUrlRoutine(string url)
        {
            Dependencies.Clear();
            string downloadingUrl;
            Hash128 downloadHash;
            bool isCached;
            tempErrorOccuring = false;
            tempAssetBundle = null;
            // Load platform's manifest to get all asset bundles
            CurrentLoadState = LoadState.LoadManifest;
            downloadingUrl = new Uri(url).Append(CurrentSetting.platformFolderName, CurrentSetting.platformFolderName).AbsoluteUri;
            yield return StartCoroutine(LoadAssetBundleFromUrlRoutine(downloadingUrl, "manifest", onManifestLoaded, onManifestLoadedFail, null));
            if (tempErrorOccuring)
                yield break;
            // Read platform's manifest to get all asset bundles info
            AssetBundleManifest manifest = tempAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] assetBundles = manifest.GetAllAssetBundles();
            LoadingAssetBundlesCount = 0;
            LoadingAssetBundlesFromCacheCount = 0;
            foreach (string assetBundle in assetBundles)
            {
                string[] dependencies = manifest.GetAllDependencies(assetBundle);
                foreach (string dependency in dependencies)
                {
                    downloadingUrl = new Uri(url).Append(CurrentSetting.platformFolderName, dependency).AbsoluteUri;
                    downloadHash = manifest.GetAssetBundleHash(dependency);
                    if (!loadingAssetBundles.ContainsKey(dependency))
                    {
                        isCached = Caching.IsVersionCached(downloadingUrl, downloadHash);
                        loadingAssetBundles.Add(dependency, new AssetBundleInfo()
                        {
                            url = downloadingUrl,
                            hash = downloadHash,
                            cached = isCached,
                        });
                        if (!isCached)
                            LoadingAssetBundlesCount++;
                        else
                            LoadingAssetBundlesFromCacheCount++;
                    }
                }
                downloadingUrl = new Uri(url).Append(CurrentSetting.platformFolderName, assetBundle).AbsoluteUri;
                downloadHash = manifest.GetAssetBundleHash(assetBundle);
                if (!loadingAssetBundles.ContainsKey(assetBundle))
                {
                    isCached = Caching.IsVersionCached(downloadingUrl, downloadHash);
                    loadingAssetBundles.Add(assetBundle, new AssetBundleInfo()
                    {
                        url = downloadingUrl,
                        hash = downloadHash,
                        cached = isCached,
                    });
                    if (!isCached)
                        LoadingAssetBundlesCount++;
                    else
                        LoadingAssetBundlesFromCacheCount++;
                }
            }
            // Load all asset bundles
            LoadedAssetBundlesCount = 0;
            LoadedAssetBundlesFromCacheCount = 0;
            CurrentLoadState = LoadState.LoadAssetBundles;
            foreach (KeyValuePair<string, AssetBundleInfo> loadingAssetBundle in loadingAssetBundles)
            {
                LoadingAssetBundleFileName = string.Empty;
                LoadingAssetBundleFromCacheFileName = string.Empty;
                isCached = loadingAssetBundle.Value.cached;
                if (!isCached)
                    LoadingAssetBundleFileName = loadingAssetBundle.Key;
                else
                    LoadingAssetBundleFromCacheFileName = loadingAssetBundle.Key;
                yield return StartCoroutine(LoadAssetBundleFromUrlRoutine(loadingAssetBundle.Value.url, $"dependency: {loadingAssetBundle.Key}", onAssetBundlesLoaded, onAssetBundlesLoadedFail, loadingAssetBundle.Value.hash));
                if (tempErrorOccuring)
                    yield break;
                Dependencies[loadingAssetBundle.Key] = tempAssetBundle;
                if (!isCached)
                    LoadedAssetBundlesCount++;
                else
                    LoadedAssetBundlesFromCacheCount++;
            }
            // All asset bundles loaded, load init scene
            CurrentLoadState = LoadState.Done;
            yield return new WaitForSecondsRealtime(delayBeforeLoadInitScene);
            SceneManager.LoadScene(initSceneName);
        }
    }
}
