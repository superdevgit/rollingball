﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPath : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
