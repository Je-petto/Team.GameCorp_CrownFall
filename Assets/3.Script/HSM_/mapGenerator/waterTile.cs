using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror.Examples.Benchmark;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class waterTile : MonoBehaviour
{
   [Tooltip("물에 들어왔을 때 곱해질 속도 배율")]
    public float slowMultiplier = 0.5f;
   [Tooltip("물에서 나왔을때 복구될 속도 배율")]
   public float normalMultiplier = 1f;

    void Awake()
    {
        // collider가 Trigger인지 보장
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        //rigidbody가 없으면 추가(Trigger가 동작하려면 rigidbody가 하나 있어야 함)
        if(GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            //if(pm != null)
                //pm.SetSpeedMultiplier(slowMultiplier);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            //if(pm != null)
                //pm.SetSpeedMultiplier(normalMultiplier);
        }
    }

}

