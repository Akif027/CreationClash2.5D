using System.Numerics;
using UnityEngine;

public static class FourierTransform
{
    public static Complex[] GenerateDescriptor(UnityEngine.Vector2[] points)
    {
        int n = points.Length;
        Complex[] descriptor = new Complex[n];
        Complex[] complexPoints = new Complex[n];

        for (int i = 0; i < n; i++)
            complexPoints[i] = new Complex(points[i].x, points[i].y);

        for (int k = 0; k < n; k++)
        {
            Complex sum = new Complex(0, 0);
            for (int t = 0; t < n; t++)
            {
                float angle = -2 * Mathf.PI * k * t / n;
                sum += complexPoints[t] * new Complex(Mathf.Cos(angle), Mathf.Sin(angle));
            }
            descriptor[k] = sum / n;
        }

        for (int i = 1; i < descriptor.Length; i++)
            descriptor[i] /= descriptor[1].Magnitude;

        descriptor[0] = new Complex(0, 0);
        return descriptor;
    }

    public static float CompareDescriptors(Complex[] desc1, Complex[] desc2)
    {
        float sum = 0;
        int count = Mathf.Min(desc1.Length, desc2.Length);
        for (int i = 1; i < count; i++)
            sum += Mathf.Abs((float)(desc1[i].Magnitude - desc2[i].Magnitude));
        return sum / count;
    }
}
