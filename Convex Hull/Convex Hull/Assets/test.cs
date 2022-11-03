using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector2[] points;
    public float pointA, pointB, pointC;

    void Start()
    {
        pointA = (points[2].x - points[4].x) * (points[0].y - points[4].y) - (points[2].y - points[4].y) * (points[0].x - points[4].x);
        //pointB = (points[3].x - points[0].x) * (points[1].y - points[0].y) - (points[3].y - points[0].y) * (points[1].x - points[0].x);
        pointC = (points[1].x - points[4].x) * (points[0].y - points[4].y) - (points[1].y - points[4].y) * (points[0].x - points[4].x);
    }
}
