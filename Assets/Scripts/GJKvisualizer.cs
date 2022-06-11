using UnityEngine;
using clCollision;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GJKvisualizer : MonoBehaviour
{
    [Tooltip("0 = infinte")]
    public float MaxNumberOfIterations; // Change to time?

    [Tooltip("Enables extra debug lines. Use only a few colliders in scene to avoid clutter." +
        "Red lines point to the simplex points at termination, from origin. Blue lines show the support search directions, " +
        "starting from the collider's transform's position.")]
    public bool DrawDebugLines = false;

    private int iter = 0;
    private GJK.Output GJKoutput;

    private UnityEngine.Vector3 point1 = new UnityEngine.Vector3();
    private UnityEngine.Vector3 point2 = new UnityEngine.Vector3();

    private CL_Collider[] cl_CollidersInScene;

    private void Start()
    {
        cl_CollidersInScene = FindObjectsOfType<CL_Collider>();
    }

    private void LateUpdate()
    {
        GJK.DRAWLINES = DrawDebugLines;

        if ((iter > MaxNumberOfIterations) && (MaxNumberOfIterations != 0.0f))
            return;

        // reset color
        for (int i = 0; i < cl_CollidersInScene.Length; i++)
            cl_CollidersInScene[i].gameObject.GetComponent<Renderer>().material.color = Color.green;

        TestAndDraw();

        iter++;

    }

    /// <summary>
    /// test all colliders in cl_CollidersInScene with one another only once and draw lines between closest points
    /// </summary>
    private void TestAndDraw()
    {
        for (int i = 0; i < cl_CollidersInScene.Length; i++) // forwards
            for (int j = cl_CollidersInScene.Length-1; j > 0; j--) // backwards
            {
                if (j <= i) continue;

                GJKoutput = GJK.GJK_intersect(cl_CollidersInScene[i], cl_CollidersInScene[j]);

                if (GJKoutput.Intersection == true)
                {
                    cl_CollidersInScene[i].gameObject.GetComponent<Renderer>().material.color = Color.red;
                    cl_CollidersInScene[j].gameObject.GetComponent<Renderer>().material.color = Color.red;
                } else if (GJKoutput.Intersection == false)
                {
                    point1 = GJKoutput.Point1.ToUnityVec();
                    point2 = GJKoutput.Point2.ToUnityVec();
                    Debug.DrawLine(point1, point2, Color.white);
                }
            }
    }

}
