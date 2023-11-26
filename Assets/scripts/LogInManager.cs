using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class LogInManager : MonoBehaviour
{
    [SerializeField] TMP_InputField userEmail;
    [SerializeField] TMP_InputField userPassword;
    [SerializeField] Button loginButton;

    private string serverURL = "http://localhost:3000/login"; // Adjust the port as needed

    public void OnSubmitLogIn()
    {
        string email = userEmail.text;
        string password = userPassword.text;

        StartCoroutine(LoginRequest(email, password));
    }

    IEnumerator LoginRequest(string email, string password)
    {
        var requestData = new
        {
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server Response: " + www.downloadHandler.text);

                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                // Only load the scene upon successful login
                if (response.message == "Login successful")
                {
                    SceneManager.LoadScene(1);
                }
            }
            else
            {
                Debug.LogError("Network Error: " + www.error);
            }
        }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
    }
}
