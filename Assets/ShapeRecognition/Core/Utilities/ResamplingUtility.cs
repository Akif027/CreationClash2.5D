using System.Collections.Generic;
using UnityEngine;

namespace ShapeRecognitionPlugin
{
    public static class ResamplingUtility
    {
        /// <summary>
        /// Resamples a list of points to a specified target count by distributing points evenly along the shape.
        /// </summary>
        /// <param name="points">The original array of points to be resampled.</param>
        /// <param name="targetCount">The desired number of points in the resampled array.</param>
        /// <returns>An array of Vector2 points resampled to the target count.</returns>
        public static Vector2[] ResamplePoints(Vector2[] points, int targetCount)
        {
            List<Vector2> resampledPoints = new List<Vector2> { points[0] };
            float totalLength = 0f;

            // Calculate the total path length of the shape
            for (int i = 1; i < points.Length; i++)
                totalLength += Vector2.Distance(points[i - 1], points[i]);

            float segmentLength = totalLength / (targetCount - 1); // Length between each resampled point
            float accumulatedDistance = 0f;

            for (int i = 1; i < points.Length; i++)
            {
                float distance = Vector2.Distance(points[i - 1], points[i]);
                accumulatedDistance += distance;

                // Add new resampled point whenever the accumulated distance reaches the segment length
                while (accumulatedDistance >= segmentLength && resampledPoints.Count < targetCount)
                {
                    float t = (segmentLength - (accumulatedDistance - distance)) / distance;
                    Vector2 point = Vector2.Lerp(points[i - 1], points[i], t);
                    resampledPoints.Add(point);
                    accumulatedDistance -= segmentLength;
                }
            }

            // Ensure the last point is added
            if (resampledPoints.Count < targetCount)
                resampledPoints.Add(points[^1]);

            return resampledPoints.ToArray();
        }
    }
}
