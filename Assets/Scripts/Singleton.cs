using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<T>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            return instance;
        }
    }
}
