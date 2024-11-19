using UnityEngine;
using System.Collections.Generic;

namespace ShapeRecognitionPlugin
{
    public static class ShapeUtils
    {
        public static Vector2[] CenterPoints(Vector2[] points)
        {
            Vector2 centroid = Vector2.zero;
            foreach (var point in points) centroid += point;
            centroid /= points.Length;

            Vector2[] centeredPoints = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                centeredPoints[i] = points[i] - centroid;
            }
            return centeredPoints;
        }

        public static Vector2[] NormalizeRotation(Vector2[] points)
        {
            float angle = Mathf.Atan2(points[1].y - points[0].y, points[1].x - points[0].x);
            Quaternion rotation = Quaternion.Euler(0, 0, -angle * Mathf.Rad2Deg);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = rotation * points[i];
            }

            return points;
        }

        public static Vector2[] SmoothPoints(Vector2[] points, int smoothingWindow = 3)
        {
            List<Vector2> smoothedPoints = new List<Vector2>();

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 sum = Vector2.zero;
                int count = 0;

                // Apply a smoothing window around each point
                for (int j = i - smoothingWindow; j <= i + smoothingWindow; j++)
                {
                    int index = (j + points.Length) % points.Length; // Wrap around the array for edge points
                    sum += points[index];
                    count++;
                }

                smoothedPoints.Add(sum / count); // Average the points in the window
            }

            return smoothedPoints.ToArray();
        }

        public static Vector2[] CenterAndNormalize(Vector2[] points)
        {
            // Calculate the center point
            Vector2 center = Vector2.zero;
            foreach (var point in points)
            {
                center += point;
            }
            center /= points.Length;

            // Center the points
            for (int i = 0; i < points.Length; i++)
            {
                points[i] -= center;
            }

            // Normalize scale
            float maxDistance = 0;
            foreach (var point in points)
            {
                maxDistance = Mathf.Max(maxDistance, point.magnitude);
            }
            if (maxDistance > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] /= maxDistance;
                }
            }

            return points;
        }

    }



}
