using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 果物の表示処理
/// </summary>
public class FruitView : MonoBehaviour
{
    /// <summary>アイコン画像</summary>
    [SerializeField] private Image iconImage;
    /// <summary>Rigidbody </summary>
    [SerializeField] private new Rigidbody2D rigidbody2D;
    /// <summary>コライダー </summary>
    [SerializeField] private CircleCollider2D circleCollider2D;
    /// <summary>この果物の位置 </summary>
    [SerializeField] private Transform fruitTransform;
    /// <summary>キー選択用のカーソルプレハブ </summary>
    [SerializeField] private FruitKeySelectFrame fruitKeySelectFramePrefab;
    private FruitKeySelectFrame keySelectFrame;

    /// <summary>
    /// モデルからデータ設定
    /// </summary>
    /// <param name="fruitModel"></param>
    public void SetFruitModel(FruitModel fruitModel)
    {
        iconImage.sprite = fruitModel.Image;
    }

    /// <summary>
    /// Rigidbodyタイプ設定
    /// </summary>
    /// <param name="type"></param>
    public void SetBodyType(RigidbodyType2D type)
    {
        rigidbody2D.bodyType = type;
    }

    /// <summary>
    /// コライダー有効無効設定
    /// </summary>
    /// <param name="switching"></param>
    public void ChangeColliderActive(bool switching)
    {
        circleCollider2D.enabled = switching;
    }

    /// <summary>
    /// 質量設定
    /// </summary>
    public void SetMass(int mass)
    {
        rigidbody2D.mass = mass;
    }

    /// <summary>
    /// 吹き飛ばし処理
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="power"></param>
    public void BlowAway(Vector3 vector, float power)
    {
        rigidbody2D.AddForce(vector * power, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 大きさの更新
    /// </summary>
    /// <param name="newScale"></param>
    public void SetLocalScale(Vector3 newScale)
    {
        fruitTransform.localScale = newScale;
    }

    /// <summary>
    /// 位置情報取得
    /// </summary>
    /// <returns></returns>
    public Transform GetTransform()
    {
        return fruitTransform;
    }

    /// <summary>
    /// 大きさ取得
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLocalScale()
    {
        return fruitTransform.localScale;
    }
    public void SetCursorFrame()
    {
        keySelectFrame = Instantiate(fruitKeySelectFramePrefab, this.gameObject.transform);
        keySelectFrame.Init();
    }
    /// <summary>
    /// 選択中
    /// </summary>
    public void Selected()
    {
        keySelectFrame.Selected();
    }

    /// <summary>
    /// 未選択
    /// </summary>
    public void UnSelected()
    {
        keySelectFrame.UnSelected();
    }

    /// <summary>
    /// 果物にドラッグ操作のコンポーネント追加
    /// </summary>
    public void AddDragMovement()
    {
        this.gameObject.AddComponent<DragMovement>();
    }

}
