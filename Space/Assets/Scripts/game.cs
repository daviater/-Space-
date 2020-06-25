using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game : NetworkBehaviour
{
    public GameObject playerPrefab;
    public static GameObject localPlayer;
    public static List<player> aP_players;
    public static List<player> aP_playerKills;
    public static List<GameObject> aGO_destroyList;
    bool b_inGame = false;
    int i_topScore = 0;
    string s_winner;
    string s_scoreboard;
    public Text T_scoreText;
    public Text T_WinLose;
    public Text T_localAddress;
    public Text T_ping;
    float f_pingTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        aP_playerKills = new List<player>();
        aGO_destroyList = new List<GameObject>();
        aP_players = new List<player>();
        T_localAddress.text = "Local Address: " + GetLocalIPAddress() ;
    }

    /// <summary>
    /// returns ip address to share with other players
    /// </summary>
    /// <returns>ip address</returns>
    public static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkServer.active)
        {
            b_inGame = true;
        }
        else
        {
            b_inGame = false;
        }
        if (b_inGame)
        {
            if (isServer)
            {
                //sorts list of scores
                aP_playerKills.Sort(delegate (player pair1, player pair2)
                {
                    return pair2.i_kills.CompareTo(pair1.i_kills);
                });

                //prints scores to scoreboard
                s_scoreboard = "Scores: \n";
                foreach (player p in aP_playerKills)
                {
                    if(p.i_kills > i_topScore)
                    {
                        i_topScore = p.i_kills;
                        s_winner = p.s_username;
                    }
                    s_scoreboard += p.s_username + ": " + p.i_kills + "\n";
                }
                if (T_scoreText.text != s_scoreboard)
                {
                    RpcUpdateScoreboard(s_scoreboard);
                    T_scoreText.text = s_scoreboard;
                }
                if(i_topScore >= 10)
                {
                    //finishes game when player reaches 10 kills
                    gameOver(s_winner);
                }

                //updates ping display
                if(f_pingTimer > 1.0f)
                {
                    T_ping.text = "Ping:\n";
                    foreach(player p in aP_players)
                    {
                        T_ping.text += p.s_username + ": ".PadRight(5) + p.i_pingToServer + "\n";
                    }
                    RpcPingUpdate(T_ping.text);
                    f_pingTimer = 0;
                }
                else
                {
                    f_pingTimer += Time.deltaTime;
                }
            }
            
           
            if (aGO_destroyList.Count != 0)
            {
                for (int i = 0; i < aGO_destroyList.Count && i > -1; i++)
                {
                    Destroy(aGO_destroyList[i]);
                }
            }

            
        }
    }

    /// <summary>
    /// Tells the player hosting if they won
    /// then send updates to gamesparks
    /// Tells clients game is over and who won
    /// Disconnects server
    /// </summary>
    /// <param name="winner">Username of winning player</param>
    void gameOver(string winner)
    {
        if (localPlayer.GetComponent<player>().s_username == winner)
        {
            T_WinLose.text = "WINNER";
            GameSpark.UpdateWin();
        }
        else
        {
            T_WinLose.text = "LOSER\n" + winner + " won";
        }
        GameSpark.UpdateKills(localPlayer.GetComponent<player>().i_kills);
        
        RpcGameOver(winner);

        NetworkServer.DisconnectAll();
        NetworkManager.Shutdown();

        SceneManager.LoadScene("EndGame");
       
    }

    /// <summary>
    /// updates scoreboard on clients end
    /// </summary>
    /// <param name="scoreboard">string containing all scoreboard info</param>
    [ClientRpc]
    void RpcUpdateScoreboard(string scoreboard)
    {
        T_scoreText.text = scoreboard;
    }

    [ClientRpc]
    void RpcPingUpdate(string s_pingText)
    {
        T_ping.text = s_pingText;
    }

    /// <summary>
    /// Receives winner username from server
    /// tells player if they won
    /// updates gamesparks
    /// </summary>
    /// <param name="winner"></param>
    [ClientRpc]
    void RpcGameOver(string winner)
    {
        if(localPlayer.GetComponent<player>().s_username == winner)
        {
            T_WinLose.text = "WINNER";
            GameSpark.UpdateWin();
        }
        else
        {
            T_WinLose.text = "LOSER\n" + winner + " won";
        }
        GameSpark.UpdateKills(localPlayer.GetComponent<player>().i_kills);
        NetworkManager.Shutdown();
        SceneManager.LoadScene("EndGame");
    }

  
}
