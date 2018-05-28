using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject ButtonsPanel;

    public GameObject ConnectPanel;

    public Text AddressText;

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
        NetworkManager.singleton.StartHost();
    }

    public void PlayOnline()
    {
        ConnectPanel.SetActive(true);
        ButtonsPanel.SetActive(false);
    }

    public void Back()
    {
        ConnectPanel.SetActive(false);
        ButtonsPanel.SetActive(true);
    }

    public void ConfirmAddress()
    {
        var address = AddressText.text;
        if (string.IsNullOrEmpty(address))
            address = "localhost";
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
    }

    public void Exit()
    {
        Application.Quit();
    }
}