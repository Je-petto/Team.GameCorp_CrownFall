using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
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
        yield return new WaitUntil(() => player != null && player.data != null);

        this.teamCode = player.teamCode;
        teamColorHpbar.color = teamCode == 0 ? Color.red : Color.blue;

        player.OnChangedHp += OnChangeCurrentHpBar;
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