using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransfromExtension
{
    //슬롯을 이름으로 찾는다 (대소문자 상관 없음)
    public static Transform FindSlot(this Transform root, params string[] slotnames)
    {
        List<Transform> children = root.GetComponentsInChildren<Transform>().ToList();        

        foreach( var slot in slotnames)
        {
        
            foreach( Transform t in children )
                if(t.name.ToLower().Contains(slot.ToLower()))
                    return t;        
        }
        Debug.Log($"못 찾음 : {slotnames.ToList()}");
        return null;
    }
}
