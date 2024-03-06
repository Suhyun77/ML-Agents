using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        TestMethod();
    }

    int num = 0;

    private void TestMethod()
    {
        while (true)
        {
            Debug.Log("1");
            for (int i=0; i<100; i++)
            {
                num++;

                if (i == 10)
                    break;
            }
            Debug.Log("2");
            break;
        }
        Debug.Log("3");
    }
}
