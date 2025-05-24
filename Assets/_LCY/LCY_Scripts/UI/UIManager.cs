using UnityEngine;
using UnityEngine.UI;

using CustomInspector;
using TMPro;

public class UIManager : MonoBehaviour
{
    [ReadOnly] public PlayerController playerController;

    public TextMeshProUGUI atk;
    public Slider hpSlider;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        hpSlider.maxValue = playerController.data.hp;
        hpSlider.value = playerController.data.hp;
        atk.text = playerController.data.name.ToString();
    }

    private void Update()
    {
        hpSlider.value = playerController.currentHp;
    }
}