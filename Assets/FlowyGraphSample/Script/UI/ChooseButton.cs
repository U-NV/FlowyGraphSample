using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using U0UGames.Localization.UI;
using U0UGames.Localization;
public class ChooseButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private LocalizeText text;
    internal void Init(LocalizeString v, Action value)
    {
        text.text = v;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            value?.Invoke();
        });
    }
}
