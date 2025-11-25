using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour
{
    // UI references — assign in Inspector
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    // removed statusText UI field — using console logging instead
    public TextMeshProUGUI usernameInfo;
    public TextMeshProUGUI uid;
    public GameObject loginPanel;
    private string lastUsername;
    private bool isProcessing = false;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            // replace with your real TitleId if needed
            PlayFabSettings.staticSettings.TitleId = "1FFB23";
            PlayFabSettings.TitleId = "1FFB23";
        }

        // If you prefer to wire the Button in code instead of the Inspector
        if (loginButton != null)
            loginButton.onClick.AddListener(LoginWithUsername);
    }

    // Call this from the Button OnClick or via code
    public void LoginWithUsername()
    {
        if (isProcessing) return;

        string username = usernameInput != null ? usernameInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Enter username and password.");
            return;
        }
        lastUsername = username;
        isProcessing = true;
        Debug.Log("Logging in...");
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        isProcessing = false;
        Debug.Log("Login successful. PlayFabId: " + result.PlayFabId);
        
        usernameInfo.text = lastUsername;
        uid.text = "UID: " + result.PlayFabId;
        loginPanel.SetActive(false);
        PlayFabManager.Instance.SetLogin(result); 
    }

    private void OnLoginFailure(PlayFabError error)
    {
        isProcessing = false;
        Debug.LogWarning("Login failed: " + error.GenerateErrorReport());
        Debug.LogWarning("Login error message: " + error.ErrorMessage);

        // optional: detect missing account and auto-register
        if (error.ErrorMessage != null && error.ErrorMessage.ToLower().Contains("account not found"))
        {
            Debug.Log("Account not found — creating account...");
            RegisterNewUser(usernameInput != null ? usernameInput.text.Trim() : "", passwordInput != null ? passwordInput.text : "");
        }
    }

    private void RegisterNewUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Cannot create account: missing fields.");
            return;
        }

        var req = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(req, OnRegisterSuccess, OnRegisterFailure);
        Debug.Log("Register request sent for username: " + username);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Account created — logging in... PlayFabId: " + result.PlayFabId);
        // login immediately after register
        LoginWithUsername();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("Register failed: " + error.GenerateErrorReport());
    }
}