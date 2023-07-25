using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSignleton : MonoBehaviour
{
    public static GameObject Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this.gameObject;
    }
}
