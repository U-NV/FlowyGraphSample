using System.Collections;
using System.Collections.Generic;
using TMPro;
using U0UGames.Localization;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject btnPrefab;
    // Start is called before the first frame update
    void Awake()
    {
        var configs = LocalizationManager.Config;
        foreach (var languageCode in configs.languageDisplayDataList)
        {
            Debug.Log("languageCode: " + languageCode.languageCode);
            if(!configs.inGameLanguageCodeList.Contains(languageCode.languageCode))
            {
                Debug.Log("not in inGameLanguageCodeList: " + languageCode.languageCode);
                    continue;
            }

            var btn = Instantiate(btnPrefab, root.transform);
            btn.GetComponentInChildren<TMP_Text>().text = languageCode.displayName;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                LocalizationManager.SwitchLanguage(languageCode.languageCode);
            });
        }
    }

}
