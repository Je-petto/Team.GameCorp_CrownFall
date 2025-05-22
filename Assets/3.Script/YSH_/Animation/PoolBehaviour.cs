using UnityEngine;

public abstract class PoolBehaviour : MonoBehaviour
{
    [HideInInspector] public PoolManager poolmanager;

    public void Despawn() => poolmanager.Despawn(this);
}
