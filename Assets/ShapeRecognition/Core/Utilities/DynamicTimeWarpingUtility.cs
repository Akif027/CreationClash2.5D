using UnityEngine;

public static class DynamicTimeWarpingUtility
{
    public static float Calculate(Vector2[] seq1, Vector2[] seq2)
    {
        int n = seq1.Length;
        int m = seq2.Length;
        float[,] dtw = new float[n, m];

        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                dtw[i, j] = Mathf.Infinity;

        dtw[0, 0] = Vector2.Distance(seq1[0], seq2[0]);

        for (int i = 1; i < n; i++)
        {
            for (int j = 1; j < m; j++)
            {
                float cost = Vector2.Distance(seq1[i], seq2[j]);
                dtw[i, j] = cost + Mathf.Min(dtw[i - 1, j], dtw[i, j - 1], dtw[i - 1, j - 1]);
            }
        }
        return dtw[n - 1, m - 1];
    }
}
