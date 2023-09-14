using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSignleton : MonoBehaviour
{
    public static GameObject Instance;
    private void Awake()
    {
        //This just makes sure there cannot be more than one event handler.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this.gameObject;
    }
}
