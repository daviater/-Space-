using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSpark : MonoBehaviour
{

    /// <summary>The GameSparks Manager singleton</summary>
    private static GameSpark instance = null;
    private string s_username;
    private string s_auth;
    public InputField username;
    public InputField password;
    public Button login;
    public Button register;
    


    void Awake()
    {
        if (instance == null) // check to see if the instance has a reference
        {
            instance = this; // if not, give it a reference to this class...
            DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
           
        }
        else // if we already have a reference then remove the extra manager from the scene
        {
            Destroy(this.gameObject);
        }
    }


    /// <summary>
    /// returns gamespark username
    /// </summary>
    /// <returns>username</returns>
    internal static string getUsername()
    {
        return instance.s_username;
    }

    /// <summary>
    /// registers player and enter Game scene
    /// </summary>
    public void RegisterPlayer()
    {
        username.enabled = false;
        password.enabled = false;
        login.enabled = false;
        register.enabled = false;
        register.GetComponentInChildren<Text>().text = "loading...";
        new GameSparks.Api.Requests.RegistrationRequest()
  .SetDisplayName(username.text)
  .SetPassword(password.text)
  .SetUserName(username.text)
  .Send((response) =>
  {
      if (!response.HasErrors)
      {
          username.enabled = true;
          password.enabled = true;
          login.enabled = true;
          register.enabled = true;
          register.GetComponentInChildren<Text>().text = "[ REGISTERED ]";
          Debug.Log("Player Registered");
          s_username = username.text;
          s_auth = response.AuthToken;
         
          SceneManager.LoadScene("Game");
      }
      else
      {
          username.enabled = true;
          password.enabled = true;
          login.enabled = true;
          register.enabled = true;
          register.GetComponentInChildren<Text>().text = "[ REGISTER ]";
          Debug.Log("Error Registering Player");
      }
    }
    );
    }

    

    internal static void UpdateKills(int kills)
    {
        instance.KillsUpdate(kills);
    }

    /// <summary>
    /// Sends update to GameSpark with number of kills
    /// </summary>
    /// <param name="kills"></param>
    void KillsUpdate(int kills)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("killsevent").SetEventAttribute("kills", kills).Send((response)=>
        {
            if (!response.HasErrors)
            {
                Debug.Log("sent kills");
                
            }
            else
            {
                Debug.Log("Error sending kills");
            }
        });
    }

    internal static void UpdateWin()
    {
        instance.WinUpdate();
    }

    /// <summary>
    /// Sends update to gamespark about win
    /// </summary>
    void WinUpdate()
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("winEvent").SetEventAttribute("win", 1).Send((response) =>
        {
            if (!response.HasErrors)
            {
                Debug.Log("sent win");
                
            }
            else
            {
                Debug.Log("Error sending win");
            }
        });
    }

    /// <summary>
    /// logs in player and enters Game scene
    /// </summary>
    public void LoginPlayer()
    {
        username.enabled = false;
        password.enabled = false;
        login.enabled = false;
        login.GetComponentInChildren<Text>().text = "loading...";
        register.enabled = false;
        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(username.text).SetPassword(password.text)
            .Send((response) => {
            if (!response.HasErrors)
            {
                    username.enabled = true;
                    password.enabled = true;
                    login.enabled = true;
                    login.GetComponentInChildren<Text>().text = "[ LOGGED IN ]";
                    register.enabled = true;
                    Debug.Log("Player Authenticated...");
                    s_username = username.text;
                    s_auth = response.AuthToken;
                    
                    SceneManager.LoadScene("Game");
            }
            else
            {
                    username.enabled = true;
                    password.enabled = true;
                    login.enabled = true;
                    login.GetComponentInChildren<Text>().text = "[ LOGIN ]";
                    register.enabled = true;
                    Debug.Log("Error Authenticating Player...");
            }
        });
    }

}
