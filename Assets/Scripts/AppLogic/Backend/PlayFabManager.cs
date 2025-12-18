using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
    public string PlayFabId;
    public string Username;
    public GameObject userInfoObject;
    public TextMeshProUGUI usernameInfo;
    public TextMeshProUGUI uid;
    public GameObject loginButton;
    public GameObject logoutButton;
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            PlayFabSettings.staticSettings.TitleId = "1FFB23";
            PlayFabSettings.TitleId = "1FFB23";
        }

        if (PlayFabManager.Instance != null && PlayFabManager.Instance.IsLoggedIn)
        {
            loginButton.SetActive(false);
            logoutButton.SetActive(true);
            usernameInfo.text = PlayFabManager.Instance.Username;
            uid.text = "UID: " + PlayFabManager.Instance.PlayFabId;
        }

        if (PlayFabManager.Instance == null)
        {
            loginButton.SetActive(true);
            logoutButton.SetActive(false);
            usernameInfo.text = "Guest";
            uid.text = "UID: guest"; 
        }
    }
    public void SetLogin(LoginResult result, string username)
    {
        PlayFabId = result.PlayFabId;
        Username = username;
        Debug.Log("Saved login session: " + PlayFabId);
    }

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayFabId = null;
        Username = null;
        Debug.Log("Logged out");
    }
}
