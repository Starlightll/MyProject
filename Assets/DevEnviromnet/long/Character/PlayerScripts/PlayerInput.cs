using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "Player/Input")]
public class PlayerInput : ScriptableObject
{
    public Vector2 MoveDirection => new Vector2(
        Input.GetAxis("Horizontal"),
        Input.GetAxis("Vertical")
    );

    public bool AttackPressed => Input.GetMouseButtonDown(0);
    public bool AttackReleased => Input.GetMouseButtonUp(0);
    public bool JumpPressed => Input.GetButtonDown("Jump");
    public bool JumpReleased => Input.GetButtonUp("Jump");
    
}
