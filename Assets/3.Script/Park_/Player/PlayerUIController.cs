using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
{
    [SerializeField] private Image teamColorHpbar;
    [SerializeField] private PlayerController_Net player;

    void Start()
    {
        // player.OnChangedHp += OnChangeCurrentHpBar;
    }


    public void SetUI()
    {
        player.OnChangeHp += OnChangeCurrentHpBar;


        RPCSetTeamColor();
    }

    [ClientRpc]
    public void RPCSetTeamColor()
    {
        teamColorHpbar.color = player.teamCode == 0 ? Color.red : Color.blue;
    }

    [ClientRpc]
    public void OnChangeCurrentHpBar(float percent)
    {
        teamColorHpbar.fillAmount = percent;
    }
}