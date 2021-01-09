using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletChamberTracking : MonoBehaviour
{
    [Tooltip("Player character game object")] [SerializeField] GameObject playerCharacter;
    PlayerController mainPlayerController = null;

    // Start is called before the first frame update
    void Start()
    {
        mainPlayerController = playerCharacter.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = mainPlayerController.bulletLoaded;
    }
}
