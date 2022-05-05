using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CLCollision;

public class DLLtest : MonoBehaviour
{
    void Start()
    {
        GJK obj = new GJK();
        obj.AddValues(2, 3);
        print("2 + 3 = " + obj.c);
    }

    void Update()
    {
        print(GJK.GenerateRandom(0, 100));
    }
}
