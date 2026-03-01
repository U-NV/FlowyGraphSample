using System.Collections.Generic;
using System.Linq;
using Oneiromancer.TMP.Effects;
using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Tags
{
    /// Component that can process custom tags in TMP_Text, given SO settings for each tag
    [ExecuteAlways]
    public class TagParser : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TagEffectsConfig effectsConfig;
        private CustomTagPreprocessor _currentPreprocessor;
        public IReadOnlyCollection<TagInfo> TagInfos => _currentPreprocessor.TagInfos;
        private bool _inPreviewMode;

        private void Awake()
        {
            SetParser();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !_inPreviewMode) return;
#endif
            UpdateTextMesh();
        }

        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }

        public void SetTargetText(TMP_Text text)
        {
            if (text == null) throw new System.ArgumentNullException(nameof(text), "Text shouldn't be null");
            _text = text;
        }

        private List<(TagInfo, BaseTextEffectSo)> _tagProcessorLookup = new ();

        private void UpdateTextMesh()
        {
            if(_currentPreprocessor == null)return;
            if(!effectsConfig || effectsConfig.TagEffects == null || effectsConfig.TagEffects.Count == 0)return;
            if(_currentPreprocessor.TagInfos == null || _currentPreprocessor.TagInfos.Count == 0)return;
            
            // 收集需要顶点动画的处理器
            _tagProcessorLookup.Clear();
            foreach (var tagInfo in _currentPreprocessor.TagInfos)
            {
                foreach (var processor in effectsConfig.TagEffects)
                {
                    if (!tagInfo.IsTagEqual(processor.Tag)) continue;
                    _tagProcessorLookup.Add((tagInfo,processor));
                }
            }

            // 存在文字特效则进行 TMP mesh更新
            if (_tagProcessorLookup.Count > 0)
            {
                _text.ForceMeshUpdate();
                foreach (var tagGroup in _tagProcessorLookup)
                {
                    var tagInfo = tagGroup.Item1;
                    var processor = tagGroup.Item2;
                    processor.ProcessEffect(_text, tagInfo.StartIndex, tagInfo.LastIndex);
                }
                _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        private void SetParser()
        {
            if (!effectsConfig || effectsConfig.TagEffects == null || effectsConfig.TagEffects.Count == 0)
            {
                _currentPreprocessor = null;
                return;
            }
            var possibleTags = effectsConfig.TagEffects.Select(x => x.Tag).ToList();
            _currentPreprocessor = new CustomTagPreprocessor(possibleTags);
            _text.textPreprocessor = _currentPreprocessor;
            _text.ForceMeshUpdate();
        }
  
        public void AddCustomTag(string tagName)
        {
            _currentPreprocessor.AddCustomTagName(tagName);
        }
        

#if UNITY_EDITOR
        [ContextMenu("Start Preview")]
        public void Preview()
        {
            if (Application.isPlaying) return;
            SetParser();
            _inPreviewMode = true;
        }

        [ContextMenu("Stop Preview")]
        public void ResetParser()
        {
            if (Application.isPlaying) return;
            _currentPreprocessor = null;
            _text.textPreprocessor = null;
            _text.ForceMeshUpdate();
            _inPreviewMode = false;
        }
#endif
    }
}