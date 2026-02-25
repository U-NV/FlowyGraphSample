using FlowyGraph;
using UnityEngine;

public class FlowyGraphSampleManager : MonoBehaviour
{
    [SerializeField] private FlowyGraphAsset introGraphAsset;
    private FlowyGraphRuntime introGraphRuntime;
    private void Start()
    {
        var saveLoad = FindObjectOfType<SaveLoadManager>();
        if (saveLoad != null)
        {
            saveLoad.LoadOrReset();
        }
        else
        {
            Debug.LogWarning("[FlowyGraphSampleManager] 未找到 SaveLoadManager，已重置黑板数据。");
            FlowyGraphBlackboard.ImportValuesFromJson("{}", true);
        }



        introGraphRuntime = new FlowyGraphRuntime(introGraphAsset);
        introGraphRuntime.OnGraphOver += OnIntroGraphOver;
        introGraphRuntime.Start();
    }

    private void OnIntroGraphOver(FlowyGraphRuntime runtime)
    {
        introGraphRuntime.OnGraphOver -= OnIntroGraphOver;
        introGraphRuntime = null;
    }
}