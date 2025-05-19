using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
{
    public int teamIndex = 1;
    [SerializeField] private Image teamColorImage;

    void Start()
    {
        if (teamIndex == 1)
            teamColorImage.color = Color.red;
        else
            teamColorImage.color = Color.blue;
    }

    public void SetTeamColor(int index)
    {
        teamIndex = index;
    }
}