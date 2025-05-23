using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectButton : MonoBehaviour
{
    public string characterCID;

    public void OnClick()
    {
        CharacterSelectManager.Instance.SelectCharacterByCID(characterCID);
    }
}