using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEditor.PackageManager;

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
    public GameObject errorMessage;
    private string lastUsername;
    private bool isProcessing = false;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            PlayFabSettings.staticSettings.TitleId = "1FFB23";
            PlayFabSettings.TitleId = "1FFB23";
        }


        if (loginButton != null)
            loginButton.onClick.AddListener(LoginWithUsername);

        if (PlayFabManager.Instance != null && PlayFabManager.Instance.IsLoggedIn)
        {
            loginPanel.SetActive(false);
            usernameInfo.text = PlayFabManager.Instance.Username;
            uid.text = "UID: " + PlayFabManager.Instance.PlayFabId;
        }

        if (PlayFabManager.Instance == null)
        {
            usernameInfo.text = "Guest";
            uid.text = "UID: guest"; 
        }
    }


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
        var request = new LoginWithPlayFabRequest 
        { 
            Username = username, 
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        isProcessing = false;
        Debug.Log("Login successful. PlayFabId: " + result.PlayFabId);
        
        string displayName = result.InfoResultPayload?.PlayerProfile?.DisplayName ?? lastUsername;
        usernameInfo.text = displayName;
        uid.text = "UID: " + result.PlayFabId;
        loginPanel.SetActive(false);
        ShowErrorMessage("Login Successful");
        PlayFabManager.Instance.SetLogin(result, lastUsername); 
    }

    private void OnLoginFailure(PlayFabError error)
    {
        isProcessing = false;
        Debug.LogWarning("Login failed: " + error.GenerateErrorReport());
        Debug.LogWarning("Login error message: " + error.ErrorMessage);
        
        ShowErrorMessage(error.ErrorMessage);
        
        
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

    private IEnumerator FadeOutNotification()
    {
        CanvasGroup cg = errorMessage.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = errorMessage.AddComponent<CanvasGroup>();
        }
        cg.alpha = 1f;
        yield return new WaitForSeconds(1f); 
        float duration = 0.5f;
        float time = 0f;
        while (time < duration && errorMessage != null && errorMessage.activeSelf)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }
        if (errorMessage != null)
        {
            cg.alpha = 0f;
            errorMessage.SetActive(false);
        }
    }

    private void ShowErrorMessage(String message){
        errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
        errorMessage.SetActive(true);
        StartCoroutine(FadeOutNotification());
    }
}