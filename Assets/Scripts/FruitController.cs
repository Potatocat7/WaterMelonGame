using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

/// <summary>
/// 果物挙動処理
/// </summary>
public class FruitController : MonoBehaviour
{
    /// <summary>果物の生成パターン </summary>
    public enum GeneratePatterns
    {
        Addition,
        Evolution,
    }
    /// <summary>果物の接触トリガー </summary>
    [SerializeField] private ObservableCollision2DTrigger fruitObservableCollision2DTrigger;
    /// <summary>果物の大きさ取得用 </summary>
    [SerializeField] private RectTransform fruitRectTransform;
    /// <summary>view見かけに関する操作 </summary>
    [SerializeField] public FruitView View;
    /// <summary>modelデータに関する操作 </summary>
    public FruitModel Model;
    /// <summary>拡大中の接触で吹き飛ばす速度 </summary>
    private const int pushingPower = 10;
    /// <summary>キャンセルトークン</summary>
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        IObservable<Collision2D> fruitOnCollisionEnterAsObservable = fruitObservableCollision2DTrigger.OnCollisionEnter2DAsObservable();
        fruitOnCollisionEnterAsObservable.Subscribe(collider =>
        {
            Model.Falling = false;
            //接触したのがこのスクリプトを持っていないと壁なので無視する
            if (collider.gameObject.GetComponent<FruitController>())
            {
                //同じスクリプトを持つオブジェクト同士で接触するので、２回処理が走ってしまうのでIndexをみて片方だけを走らせる
                if (collider.gameObject.transform.GetSiblingIndex() < this.gameObject.transform.GetSiblingIndex())
                {
                    GameManager.Instance.Contact(collider.gameObject.GetComponent<FruitController>(), this);
                }
            }
        }).AddTo(this);
        IObservable<Collider2D> fruitOnTriggerEnterAsObservable = fruitObservableCollision2DTrigger.OnTriggerEnter2DAsObservable();
        fruitOnTriggerEnterAsObservable.Subscribe(_ =>
        {
            if (!Model.Falling)
            {
                Debug.Log("接触");
                GameManager.Instance.ActiveOnGameOver();
            }
        }).AddTo(this);
        IObservable<Collider2D> fruitOnTriggerStayAsObservable = fruitObservableCollision2DTrigger.OnTriggerStay2DAsObservable();
        fruitOnTriggerStayAsObservable.Subscribe(_ =>
        {
            if (!Model.Falling)
            {
                Debug.Log("接触中");
                GameManager.Instance.ActiveOnGameOver();
            }
        }).AddTo(this);
    }

    /// <summary>
    /// スタートシーンでの表示用初期化
    /// </summary>
    /// <param name="cardID"></param>
    public void SettingOrder(int cardID)
    {
        Model = new FruitModel(cardID);
        View.SetFruitModel(Model);

        //ドラッグ用
        View.AddDragMovement();

        //キー入力用
        View.SetCursorFrame();

    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cardID"></param>
    /// <param name="pattern">生成パターン</param>
    public async UniTask Init(int cardID, GeneratePatterns pattern)
    {
        Debug.Log(cardID);
        if (SaveData.Instance.GetFruitModels() != null)
        {
            Model = new FruitModel(SaveData.Instance.GetFruitModels()[cardID - 1]);
        }
        else
        {
            Model = new FruitModel(cardID);
        }
        Debug.Log(SaveData.Instance.GetFruitModels()[cardID]);
        View.SetFruitModel(Model);

        switch (pattern)
        {
            case GeneratePatterns.Addition:
                ChangeSize();
                break;
            case GeneratePatterns.Evolution:
                await UpdateSizeAsync();
                break;
        }
    }

    /// <summary>
    /// 質量設定
    /// </summary>
    public void SetMass()
    {
        View.SetMass(Model.Mass);
    }

    /// <summary>
    /// サイズ変化
    /// </summary>
    private void ChangeSize()
    {
        Vector3 newScale = new Vector3(Model.LimitScale, Model.LimitScale, 1);
        View.SetLocalScale(newScale);
    }

    /// <summary>
    /// サイズ更新
    /// </summary>
    /// <returns></returns>
    private async UniTask UpdateSizeAsync()
    {
        Vector3 newScale;
        while (true)
        {
            if (View.GetTransform() == null)
            {
                Debug.Log("null Transform");
                return;
            }
            if (View.GetLocalScale().x >= Model.LimitScale)
            {
                newScale = new Vector3(Model.LimitScale, Model.LimitScale, 1);
                View.SetLocalScale(newScale);
                // 終了
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                return;
            }
            newScale = new Vector3(View.GetLocalScale().x + pushingPower * Time.deltaTime, View.GetLocalScale().y + pushingPower * Time.deltaTime, 1);
            View.SetLocalScale(newScale);
            // 1フレーム待機する
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 吹き飛ばしの力取得
    /// </summary>
    /// <returns></returns>
    public float GetBlowPower()
    {
        return Model.LimitScale - View.GetLocalScale().x;
    }

    /// <summary>
    /// 削除処理
    /// </summary>
    public void Destroy()
    {
        cancellationTokenSource.Dispose();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 果物のサイズ取得
    /// </summary>
    /// <returns></returns>
    public float GetFruitSize()
    {
        return (fruitRectTransform.sizeDelta.x / 2) * Model.LimitScale;
    }

}
