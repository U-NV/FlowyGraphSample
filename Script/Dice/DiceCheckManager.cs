using System;
using System.Collections;
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

    [Header("Roll Animation")]
    [Tooltip("数字跳动动画总时长（秒）")]
    [SerializeField] private float animDuration = 1.5f;
    [Tooltip("动画开始时的跳动间隔（秒），越小越快")]
    [SerializeField] private float animStartInterval = 0.03f;
    [Tooltip("动画结束时的跳动间隔（秒），越大越慢")]
    [SerializeField] private float animEndInterval = 0.15f;

    private DiceCheckRequest currentRequest;
    private Action<bool> onComplete;
    private bool hasResult;
    private bool isActive;
    private bool isAnimating;
    private int lastRoll;
    private int lastTotal;
    private bool lastSuccess;
    private Coroutine rollCoroutine;

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
        isAnimating = false;
        lastRoll = 0;
        lastTotal = 0;
        lastSuccess = false;

        panel.SetActive(true);
        SetButtonLabel("掷骰");
        SetButtonVisible(true);
        UpdateHeaderText();
        UpdatePreRollInfo();
    }

    /// <summary>
    /// 更新标题文字
    /// </summary>
    private void UpdateHeaderText()
    {
        titleText.text = string.IsNullOrEmpty(currentRequest.prompt)
            ? "属性鉴定"
            : currentRequest.prompt;
    }

    /// <summary>
    /// 掷骰前：显示难度和属性信息
    /// </summary>
    private void UpdatePreRollInfo()
    {
        detailText.text =
            $"难度: <b>{currentRequest.difficulty}</b>    属性加成: <b>+{currentRequest.attrValue}</b>\n" +
            $"<size=80%>需要 D{diceSides} + {currentRequest.attrValue} ≥ {currentRequest.difficulty}</size>";
    }

    /// <summary>
    /// 开始掷骰：先算出结果，然后播放跳动动画
    /// </summary>
    private void Roll()
    {
        if (!isActive || isAnimating)
        {
            return;
        }

        SetButtonVisible(false);

        // 预先计算最终结果
        var attrValue = currentRequest.attrValue;
        lastRoll = UnityEngine.Random.Range(1, Mathf.Max(2, diceSides + 1));
        lastTotal = lastRoll + attrValue;
        lastSuccess = lastTotal >= currentRequest.difficulty;

        // 启动跳动动画
        rollCoroutine = StartCoroutine(RollAnimationCoroutine());
    }

    /// <summary>
    /// 数字跳动动画协程：数字快速变化，逐渐减速，最后定格在真实结果上
    /// </summary>
    private IEnumerator RollAnimationCoroutine()
    {
        isAnimating = true;

        var elapsed = 0f;
        var attrValue = currentRequest.attrValue;
        var maxSide = Mathf.Max(2, diceSides + 1);

        while (elapsed < animDuration)
        {
            var t = elapsed / animDuration;
            // Ease-in 曲线：间隔随时间逐渐变大，视觉上数字跳动越来越慢
            var interval = Mathf.Lerp(animStartInterval, animEndInterval, t * t);

            var fakeRoll = UnityEngine.Random.Range(1, maxSide);
            var fakeTotal = fakeRoll + attrValue;

            detailText.text =
                $"<size=150%><b>[ {fakeRoll} ]</b></size>\n" +
                $"D{diceSides}: {fakeRoll} + 属性: {attrValue} = {fakeTotal}";

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // 动画结束，显示最终结果
        hasResult = true;
        isAnimating = false;

        var resultColor = lastSuccess ? "#4CAF50" : "#F44336";
        var resultLabel = lastSuccess ? "成功" : "失败";

        detailText.text =
            $"<size=150%><b>[ {lastRoll} ]</b></size>\n" +
            $"D{diceSides}: <b>{lastRoll}</b> + 属性: <b>{attrValue}</b> = <b>{lastTotal}</b>  (难度 {currentRequest.difficulty})\n" +
            $"<size=120%><color={resultColor}><b>— {resultLabel} —</b></color></size>";

        SetButtonLabel("确认");
        SetButtonVisible(true);
        rollCoroutine = null;
    }

    private void OnActionButtonClick()
    {
        // 动画播放中忽略点击
        if (isAnimating)
        {
            return;
        }

        if (!hasResult)
        {
            Roll();
            return;
        }

        Confirm();
    }

    private void Confirm()
    {
        if (!isActive || !hasResult)
        {
            return;
        }

        var result = lastSuccess;
        onComplete?.Invoke(result);

        Cleanup();
    }

    private void Cleanup()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetButtonVisible(false);

        isActive = false;
        isAnimating = false;
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
