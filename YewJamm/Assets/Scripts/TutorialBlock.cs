using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock : MonoBehaviour
{
    public Vector3 speedZero;
    
    void Start()
    {
        GetComponent<Rigidbody>().velocity = speedZero;
    }
}
