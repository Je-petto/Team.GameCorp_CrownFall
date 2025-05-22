using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private TowerControl control;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(gameObject.tag)) return;

        control = other.GetComponent<TowerControl>();
        if (control.protect)
        {
            control.state.shieldHealth -= 10;
        }
        else
        {
            control.state.health -= 10;
            control.isHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(gameObject.tag)) return;
        control.isHit = false;
        control = null;
    }
}