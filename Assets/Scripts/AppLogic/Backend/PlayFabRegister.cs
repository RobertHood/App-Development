using System;
using System.Collections;
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

    public GameObject errorMessage;

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
            ShowErrorMessage("username and password are required.");
            Debug.LogWarning("Register: username and password are required.");
            return;
        }

        if (password != confirm)
        {
            ShowErrorMessage("passwords do not match.");
            Debug.LogWarning("Register: passwords do not match.");
            return;
        }

        if (password.Length < 6)
        {
            ShowErrorMessage("password must be at least 6 characters.");
            Debug.LogWarning("Register: password must be at least 6 characters.");
            return;
        }

        if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
        {
            ShowErrorMessage("email is invalid");
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
        ShowErrorMessage("Register successfully. Go to login Panel to log in");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        isProcessing = false;
        Debug.LogError("Register failed: " + error.GenerateErrorReport());
    }


    void OnDestroy()
    {
        if (registerButton != null)
            registerButton.onClick.RemoveListener(RegisterNewUser);
    }

        private IEnumerator FadeOutNotification()
    {
        CanvasGroup cg = errorMessage.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = errorMessage.AddComponent<CanvasGroup>();
        }
        cg.alpha = 1f;
        yield return new WaitForSeconds(2f);
        float duration = 1f; 
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }
        cg.alpha = 0f;
        errorMessage.SetActive(false);
    }

    private void ShowErrorMessage(String message){
        errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
        errorMessage.SetActive(true);
        StartCoroutine(FadeOutNotification());
    }
}