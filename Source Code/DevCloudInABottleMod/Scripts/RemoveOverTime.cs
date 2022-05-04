using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOverTime : MonoBehaviour
{
    private static int cooldownTime = 0;
    void Start()
    {
        cooldownTime = 750;
    }
    void Update()
    {
        //my (dev9998)'s special way of doing stupid cooldown stuff
        if (cooldownTime == 0)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            cooldownTime--;
        }
    }
}
