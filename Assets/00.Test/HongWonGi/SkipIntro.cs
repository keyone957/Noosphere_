using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipIntro : MonoBehaviour
{
   

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            SceneManager.LoadScene("PrologueMap_real");
        }
    }
}
