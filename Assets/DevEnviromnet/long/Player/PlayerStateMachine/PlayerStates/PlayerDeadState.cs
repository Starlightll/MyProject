using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadState : IPlayerState
{
    private PlayerController player;

    private float deadTimer = 0.0f;
    private float timeBeforeRestart = 3.0f;

    public PlayerDeadState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void Enter()
    {
        Debug.Log("Entering Dead State");
        player._anim.SetBool("IsDead", true);
        player._rb.linearVelocity = Vector2.zero;
        deadTimer = 0.0f;
    }

    public void Execute()
    {
        deadTimer += Time.deltaTime;
        Debug.Log("Executing Dead State");
        // Check if the animation is finished
        player._rb.linearVelocity = new Vector2(0, player._rb.linearVelocity.y);
        if (player._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && deadTimer >= timeBeforeRestart)
        {
            //Restart the level
            // player._stats.ResetStats();
            // player.transform.position = player._spawnPoint;
            // player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);

            //Restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Exit()
    {
        // Debug.Log("Exiting Dead State");
    }
}
