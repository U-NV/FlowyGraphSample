using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text text;
    internal void Init(string v, Action value)
    {
        text.text = v;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            value?.Invoke();
        });
    }
}
