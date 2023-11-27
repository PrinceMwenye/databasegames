using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class SignUpManager : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button signupButton;

    private string serverURL = "http://localhost:3000/signup";

    [System.Serializable]
    public class SignupData
    {
        public string email;
        public string password;
    }

    public void OnSubmitSignUp()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        SignupData signupData = new SignupData
        {
            email = email,
            password = password
        };

        StartCoroutine(SignupRequest(signupData));
    }

    IEnumerator SignupRequest(SignupData signupData)
    {
        string jsonData = JsonUtility.ToJson(signupData);

        Debug.Log($"Request JSON: {jsonData}");

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(serverURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Server Response Code: {www.responseCode}");
                Debug.Log($"Server Response: {www.downloadHandler.text}");

                SignupResponse response = null;

                try
                {
                    response = JsonUtility.FromJson<SignupResponse>(www.downloadHandler.text);
                    Debug.Log("Response " + response.message);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing JSON response: {e}");
                }

                if (response != null && response.message == "Signup successful")
                {
                    SceneManager.LoadScene(1); // Adjust the scene index as needed
                }
                else
                {
                    Debug.LogWarning("Signup not successful. Response may not match expected format.");
                }
            }
            else
            {
                Debug.LogError($"Network Error: {www.error}");
            }
        }
    }

    [System.Serializable]
    public class SignupResponse
    {
        public string message;
    }
}
