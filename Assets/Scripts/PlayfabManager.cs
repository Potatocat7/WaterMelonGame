using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;
using Cysharp.Threading.Tasks;

/// <summary>
/// Playfab通信処理
/// </summary>
public class PlayfabManager : MonoBehaviour
{
    /// <summary>アカウントを作成するか </summary>
    private bool checkCreateAccount;   
    /// <summary>ログイン時に使うID</summary>
    private string customID;
    /// <summary>セーブしたIDの保存先</summary>
    private string datapath;

    //シングルトン化
    public static PlayfabManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        datapath = Application.dataPath + "/savedata.json";
        if(SaveData.Instance.Logined == false)
        {
            Login();
        }
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    private void Login()
    {
        customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customID,
            CreateAccount = checkCreateAccount
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    /// <summary>
    /// ログイン成功処理
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result)
    {
        //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
        if (checkCreateAccount == true && result.NewlyCreated == false)
        {
            Debug.LogWarning("CustomId :" + customID + "は既に使われています。");
            Login();//ログインしなおし
            return;
        }

        //アカウント新規作成できたらIDを保存
        if (result.NewlyCreated == true)
        {
            SaveCustomID();
            Debug.Log("新規作成成功");
            NewUserDataAsync(() =>
            {
                Debug.Log("CreateUserData");
            },
            () =>
            {
                Debug.Log("failureUserData");
            }).Forget();
        }
        else
        {
            Debug.Log("ログイン成功!!");
            GetUserDataAsync(
                (size,mass) =>
                {
                    StartController.Instance.SettingSortList(size, mass);
                },
                () =>
                {
                    //データなしor失敗なので通常の配列
                }).Forget();

        }
        SaveData.Instance.Logined = true;

    }

    /// <summary>
    /// ログイン失敗処理
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("PlayFabのログインに失敗\n" + error.GenerateErrorReport());
    }

    /// <summary>
    /// ID読み込み処理
    /// </summary>
    /// <returns></returns>
    private string LoadCustomID()
    {
        Savedata savedata = new Savedata();

        if (FindJsonfile())
        {
            savedata = LoadPlayerData();
            checkCreateAccount = false;
            return savedata.userid;
        }
        else
        {
            checkCreateAccount = true;
            return GenerateCustomID();
        }
    }

    /// <summary>
    /// ID保存処理
    /// </summary>
    private void SaveCustomID()
    {
        Savedata savedata = new Savedata();
        savedata.userid = customID;
        SavePlayerData(savedata);
    }

    /// <summary>
    /// ID作成処理
    /// </summary>
    /// <returns></returns>
    private string GenerateCustomID()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString("N");
    }

    /// <summary>
    /// ID保存書き出し処理
    /// </summary>
    /// <param name="json"></param>
    public void SavePlayerData(Savedata json)
    {
        StreamWriter writer;
        //playerデータをJSONに変換
        string jsonstr = JsonUtility.ToJson(json);
        //JSONファイルに書き込み
        writer = new StreamWriter(datapath, false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// ファイルからのID読み込み
    /// </summary>
    /// <returns></returns>
    public Savedata LoadPlayerData()
    {
        StreamReader reader = new StreamReader(datapath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<Savedata>(datastr);
    }

    /// <summary>
    /// ファイルチェック
    /// </summary>
    /// <returns></returns>
    public bool FindJsonfile()
    {
        if (File.Exists(datapath))
        {
            return true;
        }
        else
        {
            Debug.Log("Jsonファイルがなかった");
            return false;
        }
    }

    /// <summary>
    /// ランキング送信
    /// </summary>
    /// <param name="score"></param>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask SendDataRankingAsync(int score, Action success, Action failure)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Ranking",
                        Value = score
                    }
                }
        }, result =>
        {
            Debug.Log($"スコア {score} 送信完了！");
            success.Invoke();
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
            failure.Invoke();
        });
    }

    /// <summary>
    /// ランキング取得
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask GetDataRankingAsync(Action<string> success, Action failure)
    {
        PlayFabClientAPI.GetLeaderboard(
           new GetLeaderboardRequest
           {
               StatisticName = "Ranking",
               StartPosition = 0,
               MaxResultsCount = 10
           },
           result => {
               string resulttext="";
               foreach (PlayerLeaderboardEntry entry in result.Leaderboard)
               {
                   resulttext += $"{entry.DisplayName}: {entry.StatValue}" + "\n";
                   Debug.Log($"{entry.DisplayName}: {entry.StatValue}");
               }
               success.Invoke(resulttext);
           },
           error => {
               Debug.LogError(error.GenerateErrorReport());
               failure.Invoke();
           });
    }

    /// <summary>
    /// 自己ベスト取得
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask GetMyBestScoreAsync(Action<int> success, Action failure)
    {
        PlayFabClientAPI.GetLeaderboardAroundPlayer(
           new GetLeaderboardAroundPlayerRequest
           {
                StatisticName = "Ranking",
                MaxResultsCount = 1 
           },
           result => {
                Debug.Log($"{result.Leaderboard[0].Position + 1}位:{result.Leaderboard[0].DisplayName}: {result.Leaderboard[0].StatValue}");
                success.Invoke(result.Leaderboard[0].StatValue);
           },
           error => {
                Debug.LogError(error.GenerateErrorReport());
                failure.Invoke();
           });
    }

    /// <summary>
    /// ユーザーデータ取得
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask GetUserDataAsync(Action<LoadListData, LoadListData> success, Action failure)
    {
        PlayFabClientAPI.GetUserData(
            new GetUserDataRequest { },
            result =>
            {
                if (result.Data["SizeList"] == null
                 || result.Data["MassList"] == null)
                {
                    failure.Invoke();
                }
                else
                {
                    // 読み取った文字列をオブジェクト型に変換
                    LoadListData inputSize = JsonUtility.FromJson<LoadListData>(result.Data["SizeList"].Value);
                    LoadListData inputMass = JsonUtility.FromJson<LoadListData>(result.Data["MassList"].Value);

                    success.Invoke(inputSize, inputMass);
                }
            },
            error =>
            {
                failure.Invoke();
            });
    }

    /// <summary>
    /// 並び替え情報を更新
    /// </summary>
    /// <param name="name"></param>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask UpdateUserDataAsync(List<FruitController> size, List<FruitController> mass, Action success, Action failure)
    {
        LoadListData sizelist = new LoadListData();
        sizelist.list = new int[size.Count];
        LoadListData masslist = new LoadListData();
        masslist.list = new int[mass.Count];
        for (int i = 0; i < size.Count; i++)
        {
            sizelist.list[i] = size[i].Model.Number;
            masslist.list[i] = mass[i].Model.Number;
        }

        string jsonSize = JsonUtility.ToJson(sizelist);
        string jsonMass = JsonUtility.ToJson(masslist);
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                    {

                        { "SizeList", jsonSize },
                        { "MassList", jsonMass }
                    }
            },
            (result) =>
            {
                Debug.Log("UpdateUserData");
                success.Invoke();
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
                failure.Invoke();
            });
    }

    /// <summary>
    /// ユーザーデータ新規作成
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public async UniTask NewUserDataAsync(Action success, Action failure)
    {
        LoadListData sizelist = new LoadListData();
        sizelist.list = new int[8];
        sizelist.list[0] = 1;
        sizelist.list[1] = 2;
        sizelist.list[2] = 3;
        sizelist.list[3] = 4;
        sizelist.list[4] = 5;
        sizelist.list[5] = 6;
        sizelist.list[6] = 7;
        sizelist.list[7] = 8;
        LoadListData masslist = new LoadListData();
        masslist.list = new int[8];
        masslist.list[0] = 8;
        masslist.list[1] = 7;
        masslist.list[2] = 6;
        masslist.list[3] = 5;
        masslist.list[4] = 4;
        masslist.list[5] = 3;
        masslist.list[6] = 2;
        masslist.list[7] = 1;
        string jsonSize = JsonUtility.ToJson(sizelist);
        string jsonMass = JsonUtility.ToJson(masslist);
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                    {
                        { "SizeList", jsonSize },
                        { "MassList", jsonMass }
                    }
            },
            (result) =>
            {
                Debug.Log("UpdateUserData");
                success.Invoke();
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
                failure.Invoke();
            });

    }

}
[Serializable]
public class Savedata
{
    public string userid;
}
[Serializable]
public class LoadListData
{
    public int[] list;
}