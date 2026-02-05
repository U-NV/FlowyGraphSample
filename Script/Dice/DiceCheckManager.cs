using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FlowyGraph;


[Serializable]
public struct DiceCheckRequest
{
    public int attrValue;
    public int difficulty;
    public string prompt;
}

public class DiceCheckManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text detailText;
    [SerializeField] private Button rollButton;

    [Header("Attributes")]
    [SerializeField] private int diceSides = 20;

    private DiceCheckRequest currentRequest;
    private Action<bool> onComplete;
    private bool hasResult;
    private bool isActive;
    private int lastRoll;
    private int lastTotal;
    private bool lastSuccess;

    private void Awake()
    {
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(OnActionButtonClick);
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetButtonVisible(false);
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    public void RequestCheck(DiceCheckRequest request, Action<bool> onResult)
    {
        if (onResult == null)
        {
            return;
        }

        if (isActive)
        {
            Debug.LogWarning("[DiceCheckManager] 已有鉴定进行中，改为快速结算。");
            ResolveImmediately(request, onResult);
            return;
        }

        if (panel == null || rollButton == null || detailText == null || titleText == null)
        {
            ResolveImmediately(request, onResult);
            return;
        }

        isActive = true;
        currentRequest = request;
        onComplete = onResult;
        hasResult = false;
        lastRoll = 0;
        lastTotal = 0;
        lastSuccess = false;

        panel.SetActive(true);
        hasResult = false;
        SetButtonLabel("掷骰");
        SetButtonVisible(true);
        UpdateHeaderText();
        detailText.text = "点击掷骰开始鉴定。";
    }

    private void UpdateHeaderText()
    {
        var attrName = currentRequest.attrValue;
        titleText.text = string.IsNullOrEmpty(currentRequest.prompt) ? $"{attrName}鉴定" : currentRequest.prompt;
    }

    private void Roll()
    {
        if (!isActive)
        {
            return;
        }

        SetButtonVisible(false);
        var attrValue = currentRequest.attrValue;
        lastRoll = UnityEngine.Random.Range(1, Mathf.Max(2, diceSides + 1));
        lastTotal = lastRoll + attrValue;
        lastSuccess = lastTotal >= currentRequest.difficulty;
        hasResult = true;

        detailText.text =
            $"掷骰:{lastRoll} 属性:{attrValue} 难度:{currentRequest.difficulty} 结果:{(lastSuccess ? "成功" : "失败")}";
        SetButtonLabel("确认");
        SetButtonVisible(true);
    }

    private void OnActionButtonClick()
    {
        if (!hasResult)
        {
            Roll();
            return;
        }

        Confirm();
    }

    private void Confirm()
    {
        if (!isActive)
        {
            return;
        }

        if (!hasResult)
        {
            Roll();
            return;
        }

        var result = lastSuccess;
        onComplete?.Invoke(result);

        Cleanup();
    }

    private void Cleanup()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetButtonVisible(false);

        isActive = false;
        onComplete = null;
    }

    private void ResolveImmediately(DiceCheckRequest request, Action<bool> onResult)
    {
        var attrValue = request.attrValue;
        var roll = UnityEngine.Random.Range(1, Mathf.Max(2, diceSides + 1));
        var total = roll + attrValue;
        var success = total >= request.difficulty;
        onResult.Invoke(success);
    }

    private void SetButtonVisible(bool visible)
    {
        if (rollButton != null)
        {
            rollButton.gameObject.SetActive(visible);
        }
    }

    private void SetButtonLabel(string text)
    {
        if (rollButton == null)
        {
            return;
        }

        var label = rollButton.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.text = text;
        }
    }



}
