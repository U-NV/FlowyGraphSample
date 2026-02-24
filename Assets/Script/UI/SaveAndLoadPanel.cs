using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadPanel : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private SaveLoadManager saveLoadManager;

    private void Awake()
    {
        if (saveLoadManager == null)
        {
            saveLoadManager = FindObjectOfType<SaveLoadManager>();
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(HandleSave);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(HandleLoad);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(HandleReset);
        }
    }

    private void OnDestroy()
    {
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(HandleSave);
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveListener(HandleLoad);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(HandleReset);
        }
    }

    private void HandleSave()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.SaveGame();
        }
    }

    private void HandleLoad()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.LoadGame();
        }
    }

    private void HandleReset()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.ResetGame();
        }
    }
}
