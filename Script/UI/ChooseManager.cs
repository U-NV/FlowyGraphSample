using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using FlowyGraph;

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

    public void ShowChoices(List<string> choices, System.Action<int> onSelected)
    {
        choosePanel.SetActive(true);
        ClearChoices();

        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            ChooseButton btn = Instantiate(buttonPrefab, buttonContainer);
            btn.Init(choices[i], () => {
                onSelected?.Invoke(index);
                HideChoices();
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
