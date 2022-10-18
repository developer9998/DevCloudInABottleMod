using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOverTime : MonoBehaviour
{
    private float cooldownTime = 0;
    // jesus fucking christ what the actual fuck is this
    void Start()
    {
        cooldownTime = 0.75f;
        cooldownTime = Time.time + cooldownTime;
    }
    void Update()
    {
        //my (dev9998)'s special way of doing stupid cooldown stuff
        // okay fuck this

        if (Time.time > cooldownTime)
            Destroy(gameObject);
    }
}
