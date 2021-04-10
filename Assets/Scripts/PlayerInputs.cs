
using MLAPI;
using System;
using UnityEngine;

public class PlayerInputs : NetworkBehaviour
{
    PlayerController controller;
    public void Start()
    {
        controller = GetComponent<PlayerController>();
        Debug.Log("PlayerInputs, Start : isLocalPlayer ? " + IsLocalPlayer);
        if (!IsLocalPlayer) return;
        Debug.Log("PlayerInputs, Start : StickMove = " + InputManager.Controls.Player.StickMove);
        InputManager.Controls.Player.StickMove.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        InputManager.Controls.Player.StickMove.canceled += _ => SetMovement(Vector2.zero);
    }

    private void SetMovement(Vector2 direction)
    {
        Debug.Log("PlayerInputs, SetMovement : direction " + direction);
        controller.Move(direction);
    }
}
