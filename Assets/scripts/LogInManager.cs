using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class LogInManager : MonoBehaviour
{
    [SerializeField] TMP_InputField userEmail;
    [SerializeField] TMP_InputField userPassword;
    [SerializeField] Button loginButton;

    private string serverURL = "http://localhost:3000/login";

    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
    }

    public void OnSubmitLogIn()
    {
        string email = userEmail.text;
        string password = userPassword.text;

        LoginData loginData = new LoginData
        {
            email = email,
            password = password
        };

        StartCoroutine(LoginRequest(loginData));
    }

    IEnumerator LoginRequest(LoginData loginData)
    {
        string jsonData = JsonUtility.ToJson(loginData);

        using UnityWebRequest www = UnityWebRequest.PostWwwForm(serverURL, jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server Response: " + www.downloadHandler.text);

            LoginResponse response = null;

            try
            {
                response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                Debug.Log(response.message);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing JSON response: {e}");
            }

            if (response != null && response.message == "Login successful")
            {
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            Debug.LogError($"Network Error: {www.responseCode}, {www.error}");
        }
    }

}
