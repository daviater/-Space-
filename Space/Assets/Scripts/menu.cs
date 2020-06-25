using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Text T_killLeaderboard;
    public Text T_winLeaderboard;

    // Start is called before the first frame update
    void Start()
    {
        ShowLeaderBoards(true);
    }

    /// <summary>
    /// Updates and displays leaderboard or disables leaderboards
    /// </summary>
    /// <param name="show"></param>
    void ShowLeaderBoards(bool show)
    {
        if (show)
        {
            new GameSparks.Api.Requests.LeaderboardDataRequest().SetLeaderboardShortCode("winleaderboard").SetEntryCount(10).Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Found Leaderboard Data...");
                    T_winLeaderboard.text = "Win Leader Board: \n";
                    Debug.Log(response.Data);
                    foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        int rank = (int)entry.Rank;
                        string playerName = entry.UserName;
                        string score = entry.JSONData["SUM-win"].ToString();
                        Debug.Log("Rank:" + rank + " Name:" + playerName + " \n Score:" + score);
                        T_winLeaderboard.text += rank + ": " + playerName.PadRight(20) + " " + score + "\n";
                    }
                }
                else
                {
                    Debug.Log("Error Retrieving Leaderboard Data...");
                    T_winLeaderboard.text = "Cannot access Win Leader Board";
                }
            });
            new GameSparks.Api.Requests.LeaderboardDataRequest().SetLeaderboardShortCode("killleaderboard").SetEntryCount(10).Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Found Leaderboard Data...");
                    T_killLeaderboard.text = "Kill Leader Board: \n";
                    Debug.Log(response.Data);
                    foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                    {
                        int rank = (int)entry.Rank;
                        string playerName = entry.UserName;
                        string score = entry.JSONData["SUM-kills"].ToString();
                        Debug.Log("Rank:" + rank + " Name:" + playerName + " \n Score:" + score);
                        T_killLeaderboard.text += rank + ": " + playerName.PadRight(20) + " " + score + "\n";
                    }
                }
                else
                {
                    Debug.Log("Error Retrieving Leaderboard Data...");
                    T_killLeaderboard.text = "Cannot access Kill Leader Board";
                }
            });
        }
        else
        {
            T_winLeaderboard.text = "";
            T_killLeaderboard.text = "";
        }


    }
}
