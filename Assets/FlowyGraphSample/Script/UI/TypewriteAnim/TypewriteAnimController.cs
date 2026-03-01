using System;
using System.Collections;
using System.Collections.Generic;
using Oneiromancer.TMP.Tags;
using TMPro;
using U0UGames.Localization;
using UnityEngine;

namespace SubSystems.AllScenes.UI
{
    [RequireComponent(typeof(TMP_Text),typeof(TagParser))]
    public class TypewriteAnimController : MonoBehaviour
    {
        public const string WaitTagName = "wait";
        public static readonly List<string> NeedTagList = new List<string>()
        {
            WaitTagName
        };

        [SerializeField] private TypewriteAnimConfig config;
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private TagParser tagParser;
        private bool _isPlaying = false;
        public bool IsPlaying => _isPlaying;

        // private float _punctuationWaitTimeMulti = 4;
        // private int _visibleCharCount;
        private event Action OnAnimationFinished;
        private event Action OnCharVisible;
        private Coroutine _animCoroutine;
        private float _animTimePreChar;
        private int _visibleCharCount;

        private void OnValidate()
        {
            textComponent ??= GetComponent<TMP_Text>();
            tagParser ??= GetComponent<TagParser>();
        }

        private void Start()
        {
            tagParser.AddCustomTag(WaitTagName);
        }

        private float GetWaitTime(TagInfo tagInfo)
        {
            if (tagInfo == null) return 0;
            if (tagInfo.Tag != WaitTagName) return 0;
            var args = tagInfo.TagArgs;
            if (args == null || args.Count == 0) return 0;
            var firstArg = args[0];
            if(float.TryParse(firstArg, out float result)) 
                return result;
            return 0;
        }
        
        private Dictionary<int,float> _waitTimeLookup = new Dictionary<int,float>();
        
        protected IEnumerator TypewriteAnim(
            string text, 
            float waitTimeOffset)
        {
            _isPlaying = true;
            _visibleCharCount = textComponent.textInfo.characterCount;

            // 得到基准文字动画时长
            var defaultPreChar = config.DefaultWaitTimePreChar / LocalizationManager.CurrLanguageTextAnimSpeed;
            var timePreCharWithOffset = defaultPreChar;
            // 添加timeOffset
            if (waitTimeOffset > 0)
            {
                timePreCharWithOffset = (_visibleCharCount * defaultPreChar + waitTimeOffset) / _visibleCharCount;
            }
            _animTimePreChar = Mathf.Min(timePreCharWithOffset, defaultPreChar);
            
            var tagInfos = tagParser.TagInfos;
            _waitTimeLookup.Clear();
            foreach (var tagInfo in tagInfos)
            {
                if (tagInfo.Tag == WaitTagName)
                {
                    float waitTime = GetWaitTime(tagInfo);
                    if (waitTime > 0)
                    {
                        _waitTimeLookup[tagInfo.StartIndex] = waitTime;
                    }
                }
            }
            
            int endValue = _visibleCharCount;
            textComponent.maxVisibleCharacters = 0;
            var visibleCharNum = 0;
            while (visibleCharNum <= endValue)
            {
                // float charWaitTime = _animTimePreChar;
                int targetCharNum = 1;
                var timeScale = Time.timeScale;
                var frameTime = Time.unscaledDeltaTime;
                var realTimePreChar = _animTimePreChar / Time.timeScale;
                if (timeScale > 0 && realTimePreChar < frameTime)
                {
                    targetCharNum = Mathf.FloorToInt(frameTime/realTimePreChar);
                }
                targetCharNum = Mathf.Max(1, targetCharNum);
                for (int i = 0; i < targetCharNum; i++)
                {
                    int visibleCharIndex = visibleCharNum - 1;
                    bool isPunctuation = false;
                    if (visibleCharIndex >= 0 && visibleCharIndex<text.Length)
                    {
                        var visibleChar = text[visibleCharIndex];
                        if (config.Punctuations.Contains(visibleChar))
                        {
                            isPunctuation = true;
                        }
                    }

                    if (isPunctuation)
                    {
                        yield return new WaitForSeconds(config.PunctuationWaitTime);
                    }
                
                    if (_waitTimeLookup.TryGetValue(visibleCharIndex, out var waitTime))
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                    if (!isPunctuation)
                    {
                        OnCharVisible?.Invoke();
                    }
                    visibleCharNum++;
                    if (visibleCharNum > endValue)
                    {
                        break;
                    }
                    textComponent.maxVisibleCharacters = visibleCharNum;
                }
                
                yield return new WaitForSecondsRealtime(_animTimePreChar);
            }
            textComponent.maxVisibleCharacters = endValue;
            _isPlaying = false;

            OnAnimationFinished?.Invoke();
        }

        public void Complete()
        {
            if (_animCoroutine != null)
            {
                StopCoroutine(_animCoroutine);
            }
            _isPlaying = false;
            textComponent.maxVisibleCharacters = _visibleCharCount;
            OnAnimationFinished?.Invoke();
        }
        
        public void ShowText(string text,
            Action onFinish = null,
            Action onCharVisible = null,
            float waitTimeOffset = 0)
        {
            gameObject.SetActive(true);

            // 设置回调
            OnAnimationFinished = onFinish;
            OnCharVisible = onCharVisible;

            textComponent.gameObject.SetActive(true);
            
            // 初始化字符
            textComponent.text = text;
            textComponent.ForceMeshUpdate();
            
            
            // 播放文字动画
            if (_animCoroutine != null)
            {
                StopCoroutine(_animCoroutine);
            }
            _animCoroutine = StartCoroutine(TypewriteAnim( text, waitTimeOffset));
        }
    }
}
