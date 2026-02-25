using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FlowyGraph;
using U0UGames.Localization;
using U0UGames.Localization.UI;
using System.Collections.Generic;


[Serializable]
public struct DiceCheckRequest
{
    public int attrValue;
    public int difficulty;
    public LocalizeString prompt;
}

public class DiceCheckManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private LocalizeText titleText;
    [SerializeField] private LocalizeTextWithArg detailText;
    [SerializeField] private LocalizeData detailTextData = new LocalizeData("UI.Dice.DetailText",
                "难度: <b>{0}</b>    属性加成: <b>+{1}</b>\n <size=80%>需要 D{2} + {3} ≥ {4}</size>");
    [SerializeField] private LocalizeData rollAnimData = new LocalizeData("UI.Dice.RollAnim",
                "<size=150%><b>[ {0} ]</b></size>\n D{1}: {0} + 属性: {2} = {3}");
    [SerializeField] private LocalizeData rollResultData = new LocalizeData("UI.Dice.RollResult",
                "<size=150%><b>[ {0} ]</b></size>\n D{1}: <b>{0}</b> + 属性: <b>{2}</b> = <b>{3}</b>  (难度 {4})\n <size=120%><color={5}><b>— {6} —</b></color></size>");
    [SerializeField] private LocalizeData successString = new ("UI.Common.Success", "成功");
    [SerializeField] private LocalizeData failureString = new ("UI.Common.Failure", "失败");
    [SerializeField] private LocalizeData rollButtonString = new ("UI.Dice.RollButton", "掷骰");
    [SerializeField] private LocalizeData confirmButtonString = new ("UI.Dice.ConfirmButton", "确认");
    
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

        detailText.text = detailTextData.LocalizeString;
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
        SetButtonLabel(rollButtonString.LocalizeString);
        SetButtonVisible(true);

        UpdateHeaderText();
        UpdatePreRollInfo();
    }

    /// <summary>
    /// 更新标题文字
    /// </summary>
    private void UpdateHeaderText()
    {
        titleText.text = currentRequest.prompt;
    }

    /// <summary>
    /// 掷骰前：显示难度和属性信息
    /// </summary>
    private void UpdatePreRollInfo()
    {
        detailText.text = detailTextData.LocalizeString;
        detailText.SetArgs(new List<Func<object>>()
        {
            () => currentRequest.difficulty,
            () => currentRequest.attrValue,
            () => diceSides,
            () => currentRequest.attrValue,
            () => currentRequest.difficulty
        });
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

                
            detailText.text = rollAnimData.LocalizeString;
            detailText.SetArgs(new List<Func<object>>()
            {
                () => fakeRoll,
                () => diceSides,
                () => attrValue,
                () =>fakeTotal
            });

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        // 动画结束，显示最终结果
        hasResult = true;
        isAnimating = false;

        detailText.text = rollResultData.LocalizeString;
        detailText.SetArgs(new List<Func<object>>()
        {
            () => lastRoll,
            () => diceSides,
            () => attrValue,
            () =>lastTotal,
            () => currentRequest.difficulty,
            () => {return lastSuccess ? "#4CAF50" : "#F44336";},
            () => {
                return lastSuccess ? successString.LocalizeString.Value : failureString.LocalizeString.Value;
            }
        });
        
        SetButtonLabel(confirmButtonString.LocalizeString);
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

    private void SetButtonLabel(LocalizeString text)
    {
        if (rollButton == null)
        {
            return;
        }

        var label = rollButton.GetComponentInChildren<LocalizeText>();
        if (label != null)
        {
            label.text = text;
        }
    }
}
