using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
    public string PlayFabId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLogin(LoginResult result)
    {
        PlayFabId = result.PlayFabId;
        Debug.Log("Saved login session: " + PlayFabId);
    }
}
