using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour         //NetworkBehaviour
{
    public int teamCode = 0;
    [SerializeField] private Image teamColorHpbar;
    [SerializeField] private PlayerController player;

    void Start()
    {
        StartCoroutine(SetUI_Co());
        player.OnChangedHp += OnChangeCurrentHpBar;
    }

    IEnumerator SetUI_Co()
    {
        yield return new WaitUntil(() => player != null);
        this.teamCode = player.teamCode;
        if (teamCode == 0)
            teamColorHpbar.color = Color.red;
        else
            teamColorHpbar.color = Color.blue;
    }


    public void SetTeamColor(int index)
    {
        teamCode = index;
    }

    public void OnChangeCurrentHpBar(float percent)
    {
        teamColorHpbar.fillAmount = percent;
    }
}