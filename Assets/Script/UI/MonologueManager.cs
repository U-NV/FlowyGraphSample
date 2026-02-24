using UnityEngine;
using TMPro;
using FlowyGraph;
using System;
using UnityEngine.UI;
using U0UGames.Localization;
using U0UGames.Localization.UI;

public class MonologueManager : MonoBehaviour
{
    [SerializeField] private GameObject monologuePanel;
    [SerializeField] private Button bgButton;

    [SerializeField] private LocalizeText characterNameText;
    [SerializeField] private GameObject characterNameRoot;


    [SerializeField] private LocalizeText contentText;

    [SerializeField] private float closeDelay = 0.02f;


    private Action onNextCallback;

    private void Awake(){
        bgButton.onClick.AddListener(OnClickNext);
        isCloseing = false;
        monologuePanel.SetActive(false);
    }

    private void OnEnable()
    {
        FlowyGraphBlackboard.RegisterClass(this);
    }

    private void OnDisable()
    {
        FlowyGraphBlackboard.UnregisterClass(this);
    }

    private float closeTimer = 0.0f;
    private bool isCloseing = false;
    public void ShowMonologue(CharacterData character, LocalizeString content, Action onNext)
    {
        closeTimer = 0.0f;
        isCloseing = false;

        if(character!=null){
            characterNameText.text = character.nameLocalizeData.LocalizeString;
            characterNameText.color = character.nameColor;
        }
        characterNameRoot.SetActive(character!=null);

        onNextCallback = onNext;
        monologuePanel.SetActive(true);
        contentText.text = content;
    }

    public void OnClickNext()
    {
        if (onNextCallback != null)
        {
            var callback = onNextCallback;
            onNextCallback = null;
            callback.Invoke();
        }
    }

    public void StartClosing()
    {
        closeTimer = 0.0f;
        isCloseing = true;
    }

    private void Update(){
        if(isCloseing){
            closeTimer += Time.deltaTime;
            if(closeTimer >= closeDelay){
                HideMonologue();
            }
        }
    }

    public void HideMonologue()
    {
        monologuePanel.SetActive(false);
    }
}
