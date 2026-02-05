using TMPro;
using UnityEngine;
public class CharacterVisual : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform messageBubbleAnchor;

    [SerializeField] private GameObject visualRoot;

    [SerializeField] private bool isVisible = true;
    [SerializeField] private InteractableObject interactableObject;

    public CharacterData CharacterData => characterData;
    public Transform MessageBubbleAnchor => messageBubbleAnchor;
    public bool IsVisible => isVisible;
    public string SaveId => characterData != null ? characterData.name : name;

    private void Awake(){
        if (nameText != null && characterData != null)
        {
            nameText.text = characterData.name;
        }
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        visualRoot.SetActive(visible);
        if (interactableObject != null)
        {
            // 角色可见性变化时不要中断正在运行的流程图
            interactableObject.SetEnabled(visible, false);
        }
    }
}