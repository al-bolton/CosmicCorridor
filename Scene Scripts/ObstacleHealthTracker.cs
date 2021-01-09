using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHealthTracker : MonoBehaviour
{
    [Tooltip("Obstacle health")] [SerializeField] int obstacleHealth = 3;
    [Tooltip("The scene corridor generator")] [SerializeField] GameObject sceneCorridorGenerator = null;
    CorridorGenerator corridorGenerator = null;


    // Start is called before the first frame update
    void Start()
    {
        sceneCorridorGenerator = GameObject.FindGameObjectWithTag("CorridorGen");

        corridorGenerator = sceneCorridorGenerator.GetComponent<CorridorGenerator>();
    }

    private void OnTriggerEnter(Collider collidedObject)
    {
        // Simple function to delete object when it hits the box of deletion
        if (collidedObject.name == "Player Bullet(Clone)")
        {
            obstacleHealth--;

            if (obstacleHealth == 0)
            {
                Destroy(gameObject);

                corridorGenerator.obstacleListTracker[((int)transform.position.z - 1) / 2][((int)transform.position.x - 1) / 2] = 0;
            }
        }

    }
}
