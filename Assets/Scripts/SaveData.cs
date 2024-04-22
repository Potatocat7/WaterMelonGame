using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン跨ぎで持ち越すデータ
/// </summary>
public class SaveData : MonoBehaviour
{
    /// <summary>セーブデータ</summary>
    private SaveDataStatus saveDataStatus;
    /// <summary>ログイン状態</summary>
    public bool Logined { get; set; } = false;

    //シングルトン化
    public static SaveData Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        //シーンを跨いで残るDontDestroyOnLoadに移動
        saveDataStatus = new SaveDataStatus();
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// スコア保存
    /// </summary>
    /// <param name="data"></param>
    public void SaveScore(int data)
    {
        saveDataStatus.SetScore(data);
    }

    /// <summary>
    /// スコア読み込み
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return saveDataStatus.GetScore();
    }

    /// <summary>
    /// セーブデータ初期化
    /// </summary>
    public void ResetData()
    {
        saveDataStatus.ResetData();        
    }

    /// <summary>
    /// リスト保存
    /// </summary>
    /// <param name="list"></param>
    public void SetFruitModels(List<FruitModel> list)
    {
        saveDataStatus.fruitModels = list;
    }

    /// <summary>
    /// リスト読み込み
    /// </summary>
    /// <returns></returns>
    public List<FruitModel> GetFruitModels()
    {
        return saveDataStatus.fruitModels;
    }

}
