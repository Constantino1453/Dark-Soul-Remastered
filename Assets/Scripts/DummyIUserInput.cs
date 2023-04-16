using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyIUserInput : IUserInput
{
    // Start is called before the first frame update
    IEnumerator Start()//协程？
    {
        while (true)
        {
            //Dup = 1.0f;
            //Dright = 0.0f;
            //Jright = 2.0f;
            //Jup = 0.0f;
            //run = true;
            //yield return new WaitForSeconds (3.0f);
            //Dup = 0.0f;
            //Dright = 0.0f;
            //Jright = 0.0f;
            //Jup = 0.0f;
            //yield return new WaitForSeconds(1.0f);
            rb = true;
            yield return 0;//“至少yield一次”
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDmagDvec(Dup, Dright);
    }
}
