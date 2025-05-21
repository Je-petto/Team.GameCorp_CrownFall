using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectButton : MonoBehaviour
{
    public int characterID;

    public void OnClick()
    {
        CharacterSelectManager.Instance.SelectCharacter(characterID);
    }
}