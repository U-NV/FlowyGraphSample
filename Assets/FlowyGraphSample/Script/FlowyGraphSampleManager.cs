using System.Collections;
using FlowyGraph;
using U0UGames.Localization;
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

        if(LocalizationManager.IsDataLoaded)
        {
            StartIntroGraph();
        }
        else
        {
            LocalizationManager.OnDataLoadOver += OnLocalizationLoadOver;
        }
    }

    private void OnLocalizationLoadOver()
    {
        LocalizationManager.OnDataLoadOver -= OnLocalizationLoadOver;
        StartIntroGraph();
    }

    private void StartIntroGraph()
    {
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