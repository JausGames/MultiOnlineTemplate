using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    NetworkObject netObj;
    Rigidbody2D body;
    const float SPEED = 0.5f;

    public NetworkVariableVector2 Velocity = new NetworkVariableVector2(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public override void NetworkStart()
    {
        //if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
        //        out var networkedClient))
        Velocity.Value = Vector3.zero;
        body = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        var dashForce = 0f;
        //swall dash while slow
        if (body.velocity.magnitude < 0.1f) { dashForce = 0.8f; }
        var angle = Vector2.Angle(Velocity.Value, body.velocity);
        Debug.Log("PlayerController, FixedUpdate : force " + dashForce);
        Debug.Log("PlayerController, FixedUpdate : angle " + angle);
        //check Acceleration != zero && Acceleration.Speed angle < 67.5
        if (Velocity.Value != Vector2.zero && (angle < 67.5f || body.velocity.magnitude < 0.3f)) body.velocity += Velocity.Value + Velocity.Value.normalized * dashForce;
        else body.velocity -= 0.3f * body.velocity;
    }
    public void Move(Vector2 direction)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ChangeVelocity(direction);
        }
        else
        {
            SubmitPositionRequestServerRpc(direction);
        }
    }
    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        ChangeVelocity(direction);
    }

    void ChangeVelocity(Vector2 direction)
    {
        Debug.Log("PlayerController, ChangeVelocity : direction " + direction);
        Debug.Log("PlayerController, ChangeVelocity : magnitude " + body.velocity.magnitude);

        //check max speed && check controller dead zone
        if (body.velocity.magnitude < 0.8f && direction.magnitude > 0.2f) Velocity.Value = direction * SPEED;
        else Velocity.Value = Vector2.zero;
    }
}
