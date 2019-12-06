using System;
using System.Collections;

public class Class1
{
    public static int[] ArrayDiff(int[] a, int[] b)
    {
        // Your brilliant solution goes here
        // It's possible to pass random tests in about a second ;)
        Chequeo:
        for (int i = 0; i < b.Length; i++)
        {
            for (int j = 0; j < a.Length; j++)
            {
                if (a[i] == b[i])
                {
                    a[i] = null;
                    goto Chequeo;
                }
                a.Where
            }
            
        }

    }
}
