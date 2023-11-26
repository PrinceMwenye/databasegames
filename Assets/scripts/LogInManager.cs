using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogInManager : MonoBehaviour
{

    [SerializeField] TMP_InputField userEmail;
    [SerializeField] TMP_InputField userPassword;
    [SerializeField] Button loginButton;

    public void OnSubmitLogIn()
    {
        string email = userEmail.text;
        string password = userPassword.text;
        Debug.Log("Useremail " + email);

        SceneManager.LoadScene(1);
    }

}
