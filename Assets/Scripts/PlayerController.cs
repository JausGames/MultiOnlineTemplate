using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    NetworkObject netObj;
    Rigidbody2D body;

    [SerializeField] float SPEED = 0.5f;
    [SerializeField] float MAX_SPEED_RETURN = 0.5f;
    [SerializeField] float MAX_SPEED_TO_DASH = 0.1f;
    [SerializeField] float RUN_DASH = 1f;
    [SerializeField] float MAX_SPEED = 3f;
    [SerializeField] float MAX_ANGLE = 45f;

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
        if (body.velocity.magnitude < MAX_SPEED_TO_DASH) dashForce = RUN_DASH;

        var angle = Vector2.Angle(Velocity.Value, body.velocity);
        Debug.Log("PlayerController, FixedUpdate : force " + dashForce);
        Debug.Log("PlayerController, FixedUpdate : angle " + angle);
        //check Acceleration != zero && Acceleration.Speed angle < 67.5 
        if (Velocity.Value != Vector2.zero 
            && (angle < MAX_ANGLE || body.velocity.magnitude < MAX_SPEED_RETURN)
            && body.velocity.magnitude < MAX_SPEED) body.velocity += Velocity.Value + Velocity.Value.normalized * dashForce;
        
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

        //check controller dead zone  
        if (direction.magnitude > 0.2f) Velocity.Value = direction * SPEED;
        else Velocity.Value = Vector2.zero;
    }
}
