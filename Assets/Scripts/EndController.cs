using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゲームオーバー画面処理
/// </summary>
public class EndController : MonoBehaviour
{
    /// <summary>戻るボタン </summary>
    [SerializeField] private Button backButton;
    /// <summary>リザルト表示テキスト </summary>
    [SerializeField] private TextMeshProUGUI resultScoreMeshPro;
    /// <summary>自己ベスト表示テキスト </summary>
    [SerializeField] private TextMeshProUGUI BestScoreMeshPro;
    /// <summary>Ranking表示テキスト </summary>
    [SerializeField] private TextMeshProUGUI RankingScoreMeshPro;
    /// <summary>次のシーン名 </summary>
    private const string NextScene = "StartScene";

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        resultScoreMeshPro.text = SaveData.Instance.GetScore().ToString();
        backButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                ///セーブオブジェクトを削除しておく
                SaveData.Instance.ResetData();
                ///スタートシーンへの遷移
                SceneManager.LoadScene(NextScene);
            }).AddTo(gameObject);
        Observable.EveryUpdate().Subscribe(_ => {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ///セーブオブジェクトを削除しておく
                SaveData.Instance.ResetData();
                ///スタートシーンへの遷移
                SceneManager.LoadScene(NextScene);
            }
        }).AddTo(gameObject);
        GetRanking();
        GetBestScore();
    }
    private void GetRanking()
    {
        PlayfabManager.Instance.GetDataRankingAsync(
            (success) =>
            {
                Debug.Log("success");
                RankingScoreMeshPro.text = success;
            },
            () =>
            {
                Debug.Log("failure");
            }).Forget();
    }
    private void GetBestScore()
    {
            PlayfabManager.Instance.GetMyBestScoreAsync( 
            (success) =>
            {
                Debug.Log("success");
                //BestScoreMeshPro.text = "Best:" + success.ToString();
            },
            () =>
            {
                Debug.Log("failure");
            }).Forget();
    }

}
