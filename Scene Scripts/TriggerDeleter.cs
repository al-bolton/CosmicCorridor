using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeleter : MonoBehaviour
{
    private void OnTriggerEnter(Collider collidedObject)
    {
        // Simple function to delete object when it hits the box of deletion
        if (collidedObject.name == "Box of Deletion")
        {
            Destroy(gameObject);
        }
        
    }
}
