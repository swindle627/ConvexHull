using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConvexHull : MonoBehaviour
{
    public bool bruteForce, quickHull;
    // Min and max value that x and y can be. Set in Unity Editor
    public float min_X, max_X, min_Y, max_Y;
    // Visual for the points
    public GameObject pointVisual;
    // Sets the amount of points
    public int pointCount;
    // Array of points (x, y) on a 2d plane
    public Vector2[] points;
    // Used to draw the lines
    public LineRenderer line;
    // List of points that are a part of the perimeter
    private List<Vector2> perimeterPoints;
    // List of d values (d = (x - x1)(y2 - y1) - (y - y1)(x2 - x1))
    private List<float> dVals;
    // Start is called before the first frame update
    void Start()
    {
        points = new Vector2[pointCount]; 
        // Initializes array at start by setting points to random values
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x = Random.Range(min_X, max_X);
            points[i].y = Random.Range(min_Y, max_Y);
        }

        // Creates visual for points
        foreach(Vector2 point in points)
        {
            Instantiate(pointVisual, point, Quaternion.identity);
        }

        dVals = new List<float>();
        perimeterPoints = new List<Vector2>();

        if(bruteForce)
        {
            BruteForce();
        }

        if(quickHull)
        {
            Quickhull();
        }
    }
    // Calculates the perimeter via brute force
    public void BruteForce()
    {
        // line is calculated between every possible pair of points (points[i] and points[j])
        // if other points arent all on the same side of the line then the line isnt a part of the perimeter
        // if the line is a part of the perimeter points[i] and points[j] are added to a list
        for(int i = 0; i < points.Length; i++)
        {
            for (int j = 0; j < points.Length; j++)
            {
                bool allNeg = true;
                bool allPos = true;
                if(i != j)
                {
                    for(int k = 0; k < points.Length; k++)
                    {
                        if(k != i && k != j)
                        {
                            // calculates which side of the line points[k] is on
                            dVals.Add((points[k].x - points[i].x) * (points[j].y - points[i].y) - (points[k].y - points[i].y) * (points[j].x - points[i].x));
                        }
                    }

                    // if any d values are positive then they are not all negative
                    // if any d values are negative then they are not all positive
                    // d values that equal zero have no effect
                    foreach (float d in dVals)
                    {
                        if (d > 0)
                        {
                            allNeg = false;
                        }
                        else if (d < 0)
                        {
                            allPos = false;
                        }
                    }

                    // if either allNeg or allPos is true that means all points al on the same side of the line
                    // points[i] and points[j] are added to perimeterPoints
                    if (allNeg == true || allPos == true)
                    {
                        perimeterPoints.Add(points[i]);
                        perimeterPoints.Add(points[j]);
                    }

                    dVals.Clear();
                }
            }
        }

        DrawLines();
    }
    // Calculates the perimeter via quickhull
    private void Quickhull()
    {
        // will hold the indexes of the furthest left and right points
        int leftmost = 0, rightmost = 0;

        // Gets the furthest left and right points
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].x < points[leftmost].x)
            {
                leftmost = i;
            }
            else if (points[i].x > points[rightmost].x)
            {
                rightmost = i;
            }
        }

        UpperHull(leftmost, rightmost);
        LowerHull(leftmost, rightmost);
        DrawLines();
    }
    // Calculates upper hull
    private void UpperHull(int pointIndex1, int pointIndex2)
    {
        int pMax = 0;
        float maxD = 0;

        // will calculate d value of all points
        // the index of the highest value will be stored in pMax
        for(int i = 0; i < points.Length; i++)
        {
            float val = (points[i].x - points[pointIndex1].x) * (points[pointIndex2].y - points[pointIndex1].y) - (points[i].y - points[pointIndex1].y) * (points[pointIndex2].x - points[pointIndex1].x);

            if (val > maxD)
            {
                maxD = val;
                pMax = i;
            }
        }

        // if all d values are less than zero these points are in the perimeter and the recursive loop ends
        if(maxD == 0)
        {
            perimeterPoints.Add(points[pointIndex1]);
            perimeterPoints.Add(points[pointIndex2]);
            return;
        }

        UpperHull(pointIndex1, pMax);
        UpperHull(pMax, pointIndex2);
    }
    // Calculates lower hull
    private void LowerHull(int pointIndex1, int pointIndex2)
    {
        int pMin = 0;
        float minD = 0;

        // will calculate d value of all points
        // the index of the lowest value will be stored in pMin
        for (int i = 0; i < points.Length; i++)
        {
            float val = (points[i].x - points[pointIndex1].x) * (points[pointIndex2].y - points[pointIndex1].y) - (points[i].y - points[pointIndex1].y) * (points[pointIndex2].x - points[pointIndex1].x);

            if (val < minD)
            {
                minD = val;
                pMin = i;
            }
        }

        // if all d values are less than zero these points are in the perimeter and the recursive loop ends
        if (minD == 0)
        {
            perimeterPoints.Add(points[pointIndex1]);
            perimeterPoints.Add(points[pointIndex2]);
            return;
        }

        UpperHull(pointIndex1, pMin);
        UpperHull(pMin, pointIndex2);
    }
    // Draws a line between every two points in perimeterPoints to make the perimeter
    private void DrawLines()
    {
        int lineCount = perimeterPoints.Count / 2;
        
        for(int i = 0; i < lineCount; i++)
        {
            Instantiate(line);
        }

        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

        int position = 0, lineNum = 0;

        for(int i = 0; i < perimeterPoints.Count; i++)
        {
            lines[lineNum].GetComponent<LineRenderer>().SetPosition(position, new Vector3(perimeterPoints[i].x, perimeterPoints[i].y, 0));
            position++;

            if(position > 1)
            {
                position = 0;
                lineNum++;
            }
        }
    }
}
