using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using FlowyGraph;
using U0UGames.Localization;
public class ChooseManager : MonoBehaviour
{
    [SerializeField] private GameObject choosePanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private ChooseButton buttonPrefab;

    private List<ChooseButton> activeButtons = new List<ChooseButton>();

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
        choosePanel.SetActive(false);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void ShowChoices(List<LocalizeString> choices, System.Action<int> onSelected)
    {
        choosePanel.SetActive(true);
        ClearChoices();

        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            ChooseButton btn = Instantiate(buttonPrefab, buttonContainer);
            btn.Init(choices[i], () => {
                // 先清理旧选项，再触发回调（因为回调可能同步触发新的ShowChoices）
                HideChoices();
                onSelected?.Invoke(index);
            });
            activeButtons.Add(btn);
        }
    }

    public void HideChoices()
    {
        choosePanel.SetActive(false);
        ClearChoices();
    }

    private void ClearChoices()
    {
        foreach (var btn in activeButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        activeButtons.Clear();
    }
}
