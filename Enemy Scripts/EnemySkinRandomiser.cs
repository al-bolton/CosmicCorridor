using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkinRandomiser : MonoBehaviour
{
    SkinnedMeshRenderer[] characterSkins;

    // Start is called before the first frame update
    void Start()
    {
        // Get an array of skins available on the enemy character
        characterSkins = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Randomly select and enable one of them
        int randomSelector = Random.Range(0, characterSkins.Length - 1);

        characterSkins[randomSelector].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
