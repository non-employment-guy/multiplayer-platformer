using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play()
    {
        SceneManager.LoadScene("Offline");
    }

    public void PlayOnline()
    {
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        Application.Quit();
    }
}