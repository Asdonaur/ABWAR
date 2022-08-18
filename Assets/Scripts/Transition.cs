using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public string scene;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(transicion());
    }

    IEnumerator transicion()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(scene);
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
