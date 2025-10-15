using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ResourceManager : Singleton<ResourceManager>
{
    // Area 이미지 목록
    private Dictionary<Area, Sprite> dayLightAreaImageList = new Dictionary<Area, Sprite>();
    private Dictionary<Area, Sprite> nightAreaImageList = new Dictionary<Area, Sprite>();
    
    // Addressable 핸들 저장 (메모리 해제용)
    private List<AsyncOperationHandle> loadedHandles = new List<AsyncOperationHandle>();
    
    // 로딩 상태 관리
    private bool isAreaImagesLoaded = false;
    
    // 이벤트
    public event Action OnAreaImagesLoaded;
    
    #region Public Properties
    public bool IsAreaImagesLoaded => isAreaImagesLoaded;
    public Dictionary<Area, Sprite> DayLightAreaImageList => dayLightAreaImageList;
    public Dictionary<Area, Sprite> NightAreaImageList => nightAreaImageList;
    #endregion
    
    #region Area Images Management
    /// <summary>
    /// 모든 Area 이미지들을 비동기로 로드합니다.
    /// </summary>
    public async Task LoadAreaImagesAsync()
    {
        if (isAreaImagesLoaded)
        {
            Debug.Log("Area 이미지들이 이미 로드되어 있습니다.");
            return;
        }
        
        Debug.Log("ResourceManager: Area 이미지 로딩 시작...");
        
        try
        {
            await LoadAllAreaImages();
            isAreaImagesLoaded = true;
            OnAreaImagesLoaded?.Invoke();
            
            Debug.Log($"ResourceManager: Area 이미지 로딩 완료 - 낮: {dayLightAreaImageList.Count}개, 밤: {nightAreaImageList.Count}개");
        }
        catch (Exception e)
        {
            Debug.LogError($"ResourceManager: Area 이미지 로딩 실패 - {e.Message}");
        }
    }
    
    /// <summary>
    /// 특정 Area의 낮/밤 이미지를 가져옵니다.
    /// </summary>
    public Sprite GetAreaImage(Area area, bool isDayLight)
    {
        if (!isAreaImagesLoaded)
        {
            Debug.LogWarning("Area 이미지들이 아직 로드되지 않았습니다.");
            return null;
        }
        
        if (isDayLight)
        {
            return dayLightAreaImageList.TryGetValue(area, out Sprite sprite) ? sprite : null;
        }
        else
        {
            return nightAreaImageList.TryGetValue(area, out Sprite sprite) ? sprite : null;
        }
    }
    
    /// <summary>
    /// Area 이미지가 로드되었는지 확인합니다.
    /// </summary>
    public bool HasAreaImage(Area area, bool isDayLight)
    {
        if (!isAreaImagesLoaded) return false;
        
        if (isDayLight)
            return dayLightAreaImageList.ContainsKey(area);
        else
            return nightAreaImageList.ContainsKey(area);
    }
    #endregion
    
    #region Generic Asset Loading
    /// <summary>
    /// 제네릭 에셋을 비동기로 로드합니다.
    /// </summary>
    public async Task<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            loadedHandles.Add(handle);
            
            var asset = await handle.Task;
            Debug.Log($"ResourceManager: 에셋 로드 완료 - {address}");
            return asset;
        }
        catch (Exception e)
        {
            Debug.LogError($"ResourceManager: 에셋 로드 실패 - {address} - {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 라벨로 에셋들을 일괄 로드합니다.
    /// </summary>
    public async Task<List<T>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
    {
        try
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            loadedHandles.Add(handle);
            
            var assets = await handle.Task;
            Debug.Log($"ResourceManager: 라벨 '{label}'로 {assets.Count}개 에셋 로드 완료");
            return new List<T>(assets);
        }
        catch (Exception e)
        {
            Debug.LogError($"ResourceManager: 라벨 '{label}' 에셋 로드 실패 - {e.Message}");
            return new List<T>();
        }
    }
    #endregion
    
    #region Private Methods
    private async Task LoadAllAreaImages()
    {
        var loadTasks = new List<Task>();
        
        foreach (Area areaType in System.Enum.GetValues(typeof(Area)))
        {
            if (areaType == Area.AreaMaxCount) continue;
            
            string areaName = areaType.ToString();
            
            // 낮 이미지 로드 태스크
            var dayTask = LoadAreaImageAsync(areaType, areaName, true);
            loadTasks.Add(dayTask);
            
            // 밤 이미지 로드 태스크
            var nightTask = LoadAreaImageAsync(areaType, areaName, false);
            loadTasks.Add(nightTask);
        }
        
        // 모든 이미지 로딩 완료 대기
        await Task.WhenAll(loadTasks);
    }
    
    private async Task LoadAreaImageAsync(Area areaType, string areaName, bool isDayLight)
    {
        string address = isDayLight 
            ? $"Image/Area/Day/{areaName}_Day"
            : $"Image/Area/Night/{areaName}_Night";
        
        var sprite = await LoadAssetAsync<Sprite>(address);
        
        if (sprite != null)
        {
            if (isDayLight)
                dayLightAreaImageList[areaType] = sprite;
            else
                nightAreaImageList[areaType] = sprite;
                
            Debug.Log($"ResourceManager: {(isDayLight ? "낮" : "밤")} 이미지 로드 완료 - {areaName}");
        }
        else
        {
            Debug.LogWarning($"ResourceManager: {(isDayLight ? "낮" : "밤")} 이미지 로드 실패 - {areaName}");
        }
    }
    #endregion
    
    #region Memory Management
    /// <summary>
    /// 특정 핸들을 해제합니다.
    /// </summary>
    public void ReleaseHandle(AsyncOperationHandle handle)
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
            loadedHandles.Remove(handle);
        }
    }
    
    /// <summary>
    /// 모든 로드된 에셋을 해제합니다.
    /// </summary>
    public void ReleaseAllAssets()
    {
        Debug.Log("ResourceManager: 모든 에셋 해제 시작...");
        
        foreach (var handle in loadedHandles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        
        loadedHandles.Clear();
        dayLightAreaImageList.Clear();
        nightAreaImageList.Clear();
        isAreaImagesLoaded = false;
        
        Debug.Log("ResourceManager: 모든 에셋 해제 완료");
    }
    
    /// <summary>
    /// Area 이미지만 해제합니다.
    /// </summary>
    public void ReleaseAreaImages()
    {
        dayLightAreaImageList.Clear();
        nightAreaImageList.Clear();
        isAreaImagesLoaded = false;
        Debug.Log("ResourceManager: Area 이미지 해제 완료");
    }
    #endregion
    
    #region Unity Lifecycle
    private void OnDestroy()
    {
        ReleaseAllAssets();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 앱이 일시정지될 때 메모리 정리
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
    #endregion
}
