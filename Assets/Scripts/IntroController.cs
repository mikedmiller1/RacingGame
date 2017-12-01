using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            var selector = GetComponentInChildren<Dropdown>();
            SceneManager.LoadScene(selector.options[selector.value].text);
        }
	}
}