using UnityEngine;

public class QuestionMarkFollower : MonoBehaviour
{
    [SerializeField] private Transform enemy;   // The enemy this is above
    [SerializeField] private Transform player;  // The player to match Y-rotation

    [SerializeField] private float heightOffset = 2f; // How high above the enemy

    void LateUpdate()
    {
        if (enemy == null || player == null) return;

        // Stay above enemy
        Vector3 pos = enemy.position;
        pos.y += heightOffset;
        transform.position = pos;

        // Rotate flat (X=-90), match player's Y rotation, flip on Z so bottom faces player
        float playerY = player.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(-90f, playerY, 180f);
    }
}
