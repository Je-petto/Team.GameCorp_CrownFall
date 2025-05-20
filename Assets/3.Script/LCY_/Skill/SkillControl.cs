using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillControl : MonoBehaviour
{
    // ----- temp
    [SerializeField] private float targetHp;

    // ----- temp

    public SkillData data;
    public GameObject prefab;

    private bool cool_;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        prefab = Instantiate(data.prefab);
        prefab.SetActive(false);
    }

    private void Update()
    {
        SkillClick();
    }

    void SkillClick()
    {
        if (Input.GetMouseButton(1) && !cool_)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                cool_ = true;
                prefab.transform.position = hit.point;
                prefab.SetActive(true);
                StartCoroutine(Duration_Co());
                StartCoroutine(CoolDown_Co());
            }
        }
    }


    #region Coroutine
    IEnumerator Duration_Co()
    {
        yield return new WaitForSeconds(data.duration);
        prefab.SetActive(false);
    }

    IEnumerator CoolDown_Co()
    {
        yield return new WaitForSeconds(data.cool);
        cool_ = false;
    }

    IEnumerator DotDamage_Co()
    {
        for (int i = 0; i < data.dot;)
        {
            targetHp -= data.damage;
            yield return new WaitForSeconds(data.dot / data.dot);
            i++;
        }
        //state = STATE.NONE;
    }
    #endregion
}