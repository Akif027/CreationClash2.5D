using System.Collections.Generic;
using UnityEngine;
namespace ShapeRecognitionPlugin
{
    public static class SmoothShape
    {
        // Smooths points to reduce noise, especially around intersections
        public static Vector2[] SmoothPoints(Vector2[] points, int smoothingWindow = 3)
        {
            List<Vector2> smoothedPoints = new List<Vector2>();

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 sum = Vector2.zero;
                int count = 0;

                for (int j = i - smoothingWindow; j <= i + smoothingWindow; j++)
                {
                    if (j >= 0 && j < points.Length)
                    {
                        sum += points[j];
                        count++;
                    }
                }

                smoothedPoints.Add(sum / count);
            }

            return smoothedPoints.ToArray();
        }

        // Removes redundant points around intersections by applying a minimum distance filter
        public static Vector2[] RemoveRedundantIntersectionPoints(Vector2[] points, float minDistance = 0.1f)
        {
            List<Vector2> filteredPoints = new List<Vector2>();
            filteredPoints.Add(points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                if (Vector2.Distance(points[i], filteredPoints[^1]) > minDistance)
                {
                    filteredPoints.Add(points[i]);
                }
            }

            return filteredPoints.ToArray();
        }

    }

}