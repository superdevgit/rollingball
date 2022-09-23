using UnityEngine;
using System.Collections;

public class csShowAllEffect : MonoBehaviour
{
    public string[] EffectName;
    public Transform[] Effect;
    //public GUIText Text1;
    public int i = 0;

    public void Init()
    {
        Instantiate(Effect[i], transform.position + new Vector3(0, 4.5f, 0), Quaternion.identity);
    }

    void Update ()
    {
        //Text1.text = i + 1 + ":" + EffectName[i];

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (i <= 0)
                i = 51;

            else
                i--;

           Instantiate(Effect[i], transform.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (i < 51)
                i++;

            else
                i = 0;

            Instantiate(Effect[i], transform.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.C))
        { 
            Instantiate(Effect[i], transform.position, Quaternion.identity);
        }
    }
}
