using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DieEnd : MonoBehaviour
{
    // Start is called before the first frame update
  public void Again()
    {
        SceneManager.LoadScene("Mobile");
    }

}
