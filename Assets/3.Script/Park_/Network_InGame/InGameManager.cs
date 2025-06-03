using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class InGameManager : NetworkBehaviour
{
    public static InGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        Instance = this;
    }

    [SyncVar]
    public bool isGameOver;

    private void Start()
    {
        isGameOver = false;
    }
    
    

}