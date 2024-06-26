using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using TMPro;
using UnityEngine.SceneManagement;
/// <summary>
/// ゲーム全体の処理
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> 点数表示</summary>
    [SerializeField] private TextMeshProUGUI scoreTextMeshPro;
    /// <summary> ゲームオーバーの表示</summary>
    [SerializeField] private GameObject gameoverTxtObject;
    /// <summary> 果物プレハブ</summary>
    [SerializeField] private FruitController fruitPrefab;
    /// <summary> オブジェクトの親</summary>
    [SerializeField] public Transform ParentTransform;
    /// <summary> プレイヤー処理情報</summary>
    [SerializeField] private PlayerController player;
    /// <summary> キャンバスのRect</summary>
    [SerializeField] private RectTransform canvas;
    /// <summary> 次に落とす果物</summary>
    private FruitController settingFruit;
    /// <summary> 落下中果物</summary>
    private FruitController dropingFruit;
    /// <summary> 果物番号最大値</summary>
    private const int FruitMaxId = 8;
    /// <summary> 吹き飛ばしの力</summary>
    private const int BlowPowerMagnification= 10;
    /// <summary> 落とすときの値かわ</summary>
    private const int FallPower = 250;
    /// <summary> 乱数最大最小</summary>
    private const int RandomMax = 4;
    private const int RandomMin = 1;
    /// <summary> スコア</summary>
    private int score = 0;
    /// <summary>次のシーン名 </summary>
    private const string NextScene = "EndScene";
    //シングルトン化
    public static GameManager Instance;
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
        gameoverTxtObject.SetActive(false);
        scoreTextMeshPro.text = score.ToString();
        Debug.Log(canvas.sizeDelta.x);
        MakeFruit();
        Observable.EveryUpdate()
        .Subscribe(_ => {
#if UNITY_EDITOR
            //タップ判定（マウスクリック）
            if (Input.GetMouseButton(0))
            {
                ///画面遷移時にcanvasやらがnullになってここを通ることがあるので
                ///エラーが表示される
                //Debug.Log(Input.mousePosition.x);
                //Debug.Log(canvas.sizeDelta.x / 3);
                if (canvas.sizeDelta.x / 3 > Input.mousePosition.x)
                {
                    //タップ位置が左側の1/3なら左移動操作
                    if (!player.CheckLimitPositionLeft())
                    {
                        player.MovingPlayer(true);
                    }
                }
                else if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) < Input.mousePosition.x)
                {
                    //タップ位置が右側の1/3なら右移動操作
                    if (!player.CheckLimitPositionRight())
                    {
                        player.MovingPlayer(false);
                    }
                }
                else
                {
                    //その他（タップ位置が真ん中あたり）なら落下操作
                    DropFruit();
                }
            }
            //キー入力
            if (Input.GetKey(KeyCode.A))
            {
                if (!Input.GetKey(KeyCode.D))
                {
                    if (!player.CheckLimitPositionLeft())
                    {
                        player.MovingPlayer(true);
                    }
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (!Input.GetKey(KeyCode.A))
                {
                    if (!player.CheckLimitPositionRight())
                    {
                        player.MovingPlayer(false);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DropFruit();
            }

#elif UNITY_ANDROID || UNITY_IOS
            //タップ判定
            if (Input.touchCount > 0)
            {
                // タップしたスクリーン座標
                var screenPos = Input.GetTouch(0).position;
                if(canvas.sizeDelta.x/3 > screenPos.x)
                {
                    if (Input.touchCount > 1)
                    {
                        var screenPos2 = Input.GetTouch(1).position;
                        if (Input.touchCount > 2)
                        {
                            //左右移動を押しながら落下操作をしたときのみ処理を動かす
                            var screenPos3 = Input.GetTouch(2).position;
                            if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) > screenPos3.x
                             && canvas.sizeDelta.x / 3 < screenPos3.x)
                            {
                                DropFruit();
                            }
                        }
                        else
                        {
                            if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) > screenPos2.x
                             && canvas.sizeDelta.x / 3 < screenPos2.x)
                            {
                                DropFruit();
                            }
                            else if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) < screenPos2.x)
                            {
                                //左右をタップしているので動かない
                            }
                        }
                    }
                    else
                    {
                        //タップ位置が左側の1/3なら左移動操作
                        if (!player.CheckLimitPositionLeft())
                        {
                            player.MovingPlayer(true);
                        }
                    }
                }
                else if (canvas.sizeDelta.x -(canvas.sizeDelta.x / 3) < screenPos.x)
                {
                    if (Input.touchCount > 1)
                    {
                        var screenPos2 = Input.GetTouch(1).position;
                        if (Input.touchCount > 2)
                        {
                            //左右移動を押しながら落下操作をしたときのみ処理を動かす
                            var screenPos3 = Input.GetTouch(2).position;
                            if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) > screenPos3.x
                             && canvas.sizeDelta.x / 3 < screenPos3.x)
                            {
                                DropFruit();
                            }
                        }
                        else
                        {
                            if (canvas.sizeDelta.x - (canvas.sizeDelta.x / 3) > screenPos2.x
                             && canvas.sizeDelta.x / 3 < screenPos2.x)
                            {
                                DropFruit();
                            }
                            else if (canvas.sizeDelta.x / 3 > screenPos2.x)
                            {
                                //左右をタップしているので動かない
                            }
                        }
                    }
                    else
                    {
                        //タップ位置が右側の1/3なら右移動操作
                        if (!player.CheckLimitPositionRight())
                        {
                            player.MovingPlayer(false);
                        }
                    }
                }
                else
                {
                    //その他（タップ位置が真ん中あたり）なら落下操作
                    DropFruit();
                }

            }
            if (Input.touchCount > 0)
            {
                //タップされた場所をしらべる
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var pos = touch.position;
                }
            }
#else
#endif
        }).AddTo(gameObject);

    }

    /// <summary>
    /// 同じものが接触した場合の処理
    /// </summary>
    /// <param name="fruit1"></param>
    /// <param name="fruit2"></param>
    public void Contact(FruitController fruit1, FruitController fruit2)
    {
        if (fruit1.Model.Number == fruit2.Model.Number)
        {
            if (fruit1.Model.Number == FruitMaxId)
            {
                score += fruit1.Model.Number + 1; //これ以上の果物がないため得点だけにする
                scoreTextMeshPro.text = score.ToString();
            }
            else
            {
                Evolution(fruit1.View.GetTransform(), fruit2.View.GetTransform(), fruit1.Model.Number + 1);
            }
            fruit1.Destroy();
            fruit2.Destroy();
        }
        else
        {
            Vector3 ForceVec1 = new Vector3(fruit1.transform.localPosition.x - fruit2.transform.localPosition.x,
                                            fruit1.transform.localPosition.y - fruit2.transform.localPosition.y,
                                            0);
            Vector3 ForceVec2 = new Vector3(fruit2.transform.localPosition.x - fruit1.transform.localPosition.x,
                                            fruit2.transform.localPosition.y - fruit1.transform.localPosition.y,
                                            0);
            fruit1.View.BlowAway(ForceVec1, fruit2.GetBlowPower() * BlowPowerMagnification);
            fruit2.View.BlowAway(ForceVec2, fruit1.GetBlowPower() * BlowPowerMagnification);
        }
    }

    /// <summary>
    /// 進化処理
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="next"></param>
    private void Evolution(Transform pos1, Transform pos2, int next)
    {
        score += next;
        scoreTextMeshPro.text = score.ToString();
        FruitController fruit = Instantiate(fruitPrefab, ParentTransform);
        fruit.transform.localPosition = Vector3.Lerp(pos1.localPosition, pos2.localPosition, 0.5f);
        fruit.Init(next,FruitController.GeneratePatterns.Evolution).Forget();
    }

    /// <summary>
    /// 果物落下処理
    /// </summary>
    private void DropFruit()
    {
        if (dropingFruit == null) //落下中のオブジェクトがない場合(最初等)
        {
            settingFruit.transform.parent = ParentTransform;
            settingFruit.View.SetBodyType(RigidbodyType2D.Dynamic);
            settingFruit.View.ChangeColliderActive(true);
            settingFruit.View.BlowAway(Vector3.down, 250);
            dropingFruit = settingFruit;
            dropingFruit.Model.Falling = true;
            dropingFruit.SetMass();
            MakeFruit();
        }
        if (!dropingFruit.Model.Falling)
        {
            //先にあった果物が落ちてからしばらくは落とせない
            settingFruit.transform.parent = ParentTransform;
            settingFruit.View.SetBodyType(RigidbodyType2D.Dynamic);
            settingFruit.View.ChangeColliderActive(true);
            settingFruit.View.BlowAway(Vector3.down, FallPower);
            dropingFruit = settingFruit;
            dropingFruit.Model.Falling = true;
            dropingFruit.SetMass();
            MakeFruit();
        }
    }

    /// <summary>
    /// 乱取得
    /// </summary>
    /// <returns>0～3の乱数値</returns>
    private int GetRandomID()
    {
        return UnityEngine.Random.Range(RandomMin, RandomMax); 
    }

    /// <summary>
    /// 果物生成
    /// </summary>
    /// <param name="makePosision"></param>
    public void MakeFruit()
    {
        FruitController fruit = Instantiate(fruitPrefab, player.transform);
        fruit.transform.position = player.transform.position;
        fruit.Init(GetRandomID(),FruitController.GeneratePatterns.Addition).Forget();
        fruit.View.SetBodyType(RigidbodyType2D.Static);
        fruit.View.ChangeColliderActive(false);
        player.SettingFruit(fruit);
        settingFruit = fruit;
        player.ResetEdgePosition();
    }

    /// <summary>
    /// ゲームオーバーテキスト表示
    /// </summary>
    public void ActiveOnGameOver()
    {
        gameoverTxtObject.SetActive(true);
        SaveData.Instance.SaveScore(score);
        PlayfabManager.Instance.SendDataRankingAsync(score,
            () =>
            {
                Debug.Log("success");
                SceneManager.LoadScene(NextScene);
            },
            () =>
            {
                Debug.Log("failure");
            }).Forget();

    }

}
