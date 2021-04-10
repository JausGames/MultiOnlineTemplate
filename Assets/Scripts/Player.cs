using MLAPI;
using UnityEngine;

public class Player : NetworkBehaviour
{
    PlayerController controller;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }



}
