using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Net.NetworkInformation;

public class AuthManager : MonoBehaviour
{
    public GameObject registerForm;
    public GameObject loginForm;

    [Header("Login / Sign In")]
    public InputField signInName;
    public InputField signInPassword;

    [Header("Register / Sign Up")]
    public InputField signUpName;
    public InputField signUpPassword;
    public InputField signUpConfirmPassword;

    [Space(10)]
    public Text signUpMessageText;
    public Text signInMessageText;


    public static Action OnUserSignIn; // Login
    public static Action OnUserSignUp; // Register

    private void Update()
    {
        if(signInMessageText.text == "������� ��������������� � �������!")
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    private void Start()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }
    private void OnEnable()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }
    private void OnDisable()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }
    private void OnDestroy()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }
    public void ButtonRegister()
    {
        registerForm.SetActive(true);
        loginForm.SetActive(false);
    }
    public void ButtonSingIn()
    {
        registerForm.SetActive(false);
        loginForm.SetActive(true);
    }

    public void SignIn()
    {
        string name = signInName.text;
        string password = signInPassword.text;

        SignIn(name, password);
    }
    public void SignIn(string email, string password)
    {
        signInMessageText.text = "����� ��������...";
        signInMessageText.color = Color.blue;

        if(Database.FindUserByDB(email, password))
        {
            try
            {
                signInMessageText.text = "������� ��������������� � �������!";
                signInMessageText.color = Color.green;
                Database.FindIdUserOnAutorization(email, password);

            }
            catch (Exception)
            {
                signInMessageText.text = "��������� ������ ��� ����������� ������������. ����������, ��������� �������.";
                signInMessageText.color = Color.red;
            }
        }
        else if (email == "" || password == "")
        {
            signInMessageText.text = "��������� ��� ����.";
            signInMessageText.color = Color.red;
        }
        else
        {
            signInMessageText.text = "������������ �� ������. ��������� ��������� ������.";
            signInMessageText.color = Color.red;
        }
    }
    
    public void SignUp()
    {
        string name = signUpName.text;
        string password = signUpPassword.text;
        string �onfirmPassword = signUpConfirmPassword.text;

        bool IsNameEmpty = string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name);
        bool IsPasswordEmpty = string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password);
        bool IsConfirmPasswordEmpty = string.IsNullOrEmpty(�onfirmPassword) || string.IsNullOrWhiteSpace(�onfirmPassword);

        signUpMessageText.text = "���������, ��� ������ ���������...";
        signUpMessageText.color = Color.blue;

        if (signUpName.text.Length != 0 || signUpPassword.text.Length != 0 || signUpConfirmPassword.text.Length != 0)
        {
            
            //�������� �������
            if (PasswordVerification())
            {
                /*�������� �� ���������� ���*/
                if (!string.IsNullOrEmpty(name))
                {
                    if (Database.FindUserNimnameByDB(name))
                    {
                        Database.SendUserToDB(name, password);
                        signUpMessageText.text = "������� ������!";
                        signUpMessageText.color = Color.green;
                    }
                    else
                    {
                        signUpMessageText.text = "������ ��� ��� ������ ������ �������������!";
                        signUpMessageText.color = Color.red;
                    }
                }
            }
        }
        else
        {
            signUpMessageText.text = "��������� ��� ����!";
            signUpMessageText.color = Color.red;
        }

    }

    public bool PasswordVerification()
    {
        if (signUpPassword.text.Length >= 6 && signUpConfirmPassword.text.Length >= 6)
        {
            if (signUpPassword.text == signUpConfirmPassword.text)
            {
                return true;
            }
            else if(signUpPassword.text != signUpConfirmPassword.text)
            {
                signUpMessageText.text = "������ �� ���������!";
                signUpMessageText.color = Color.red;
                return false;
            }
            else return false;
        }
        else if(signUpPassword.text.Length < 6 && signUpConfirmPassword.text.Length < 6)
        {
            signUpMessageText.text = "������ ������������ ������!";
            signUpMessageText.color = Color.red;
            return false;
        }
        else
        {
            signUpMessageText.text = "������ �� ���������!";
            signUpMessageText.color = Color.red;
            return false;
        }
    }
}
[System.Serializable]
public class AuthData
{
    public string localId;
    public string email;
}