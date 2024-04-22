using System.Collections;
using System.Collections.Generic;

public class SaveDataStatus
{
    /// <summary>選択用果物のリスト</summary>
    public List<FruitModel> fruitModels { get; set; }
    /// <summary>スコアデータ</summary>
    private int score = 0;

    /// <summary>
    /// スコア保存
    /// </summary>
    /// <param name="data"></param>
    public void SetScore(int data)
    {
        score = data;
    }
    /// <summary>
    /// スコア読み込み
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// セーブデータ初期化
    /// </summary>
    public void ResetData()
    {
        fruitModels = new List<FruitModel>();
        score = 0;
    }

}
