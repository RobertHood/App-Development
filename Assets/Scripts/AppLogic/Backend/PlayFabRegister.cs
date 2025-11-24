using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabRegister : MonoBehaviour
{
    // Assign these in the Inspector
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField emailInput;
    public Button registerButton;

    private bool isProcessing = false;

    void Start()
    {
        // Ensure PlayFab TitleId is set (replace with your real TitleId if needed)
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "1FFB23";
            PlayFabSettings.TitleId = "1FFB23";
        }

        if (registerButton != null)
            registerButton.onClick.AddListener(RegisterNewUser);
    }

    public void RegisterNewUser()
    {
        if (isProcessing) return;

        string username = usernameInput != null ? usernameInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";
        string confirm = confirmPasswordInput != null ? confirmPasswordInput.text : "";
        string email = emailInput != null ? emailInput.text.Trim() : "";

        // Basic validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Register: username and password are required.");
            return;
        }

        if (password != confirm)
        {
            Debug.LogWarning("Register: passwords do not match.");
            return;
        }

        if (password.Length < 6)
        {
            Debug.LogWarning("Register: password must be at least 6 characters.");
            return;
        }

        if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
        {
            Debug.LogWarning("Register: email looks invalid.");
            return;
        }

        isProcessing = true;
        Debug.Log("Register: sending request for username: " + username);

        var req = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            Email = string.IsNullOrEmpty(email) ? null : email,
            RequireBothUsernameAndEmail = false,
            DisplayName = username
        };

        PlayFabClientAPI.RegisterPlayFabUser(req, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        isProcessing = false;
        Debug.Log("Register: success. PlayFabId: " + result.PlayFabId);
        // Auto login after successful register
        AutoLoginAfterRegister();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        isProcessing = false;
        Debug.LogError("Register failed: " + error.GenerateErrorReport());
    }

    private void AutoLoginAfterRegister()
    {
        string username = usernameInput != null ? usernameInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("AutoLogin: missing credentials.");
            return;
        }

        Debug.Log("AutoLogin: attempting login for " + username);
        var loginReq = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(loginReq, OnAutoLoginSuccess, OnAutoLoginFailure);
    }

    private void OnAutoLoginSuccess(LoginResult result)
    {
        Debug.Log("AutoLogin success. PlayFabId: " + result.PlayFabId);
        // TODO: proceed to next scene / hide register UI
    }

    private void OnAutoLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("AutoLogin failed: " + error.GenerateErrorReport());
    }

    void OnDestroy()
    {
        if (registerButton != null)
            registerButton.onClick.RemoveListener(RegisterNewUser);
    }
}