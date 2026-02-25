using System.Collections;
using System.Collections.Generic;
using U0UGames.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "FlowyGraphSample/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    // public string name;
    public LocalizeData nameLocalizeData;
    public Color nameColor;
}
