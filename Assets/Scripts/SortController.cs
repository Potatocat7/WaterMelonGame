using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
public class SortController : MonoBehaviour
{
    public enum MovingIndexType
    {
        IncreaseSelect,
        DecreaseSelect,
        IncreaseChoosing,
        DecreaseChoosing,
    }
    /// <summary>選択フレーム </summary>
    [SerializeField] private GameObject selectFrame;
    /// <summary>順番表示用プレハブ </summary>
    [SerializeField] private FruitController fruitPrefab;
    /// <summary>順番表示選択中スペースプレハブ </summary>
    [SerializeField] private GameObject choosingSpacePrefab;
    /// <summary>順番表示場所用 </summary>
    [SerializeField] private Transform keyFruitOrderTransform;
    /// <summary>順番表示選択中用 </summary>
    [SerializeField] private Transform keyChoosingSpaceTransform;
    /// <summary>選択用果物のリスト</summary>
    private List<FruitController> fruitControllerList = new List<FruitController>();
    /// <summary>選択前インデックス</summary>
    private int selectIndex;
    /// <summary>選択中インデックス</summary>
    private int choosingIndex;
    /// <summary>選択中の果物そのもの</summary>
    private FruitController choosingFruit;
    /// <summary>最大値</summary>
    private int fruitMax;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="Max"></param>
    public void Init(int Max)
    {
        selectFrame.SetActive(false);
        choosingFruit = null;
        selectIndex = 0;
        fruitMax = Max;
        //キー入力用
        for (int i = 0; i <= Max; i++)
        {
            //キー入力用
            FruitController fruit = Instantiate(fruitPrefab, keyFruitOrderTransform);
            fruit.SettingOrder(i + 1);
            fruit.View.SetBodyType(RigidbodyType2D.Static);
            fruit.View.ChangeColliderActive(false);
            fruitControllerList.Add(fruit);
        }

        for (int i = 0; i <= Max - 1; i++)
        {
            GameObject space = Instantiate(choosingSpacePrefab, keyChoosingSpaceTransform);
        }
    }

    /// <summary>
    /// 選択on
    /// </summary>
    public void SelectOnSortController()
    {
        ChangeSelectFrame(true);
    }

    /// <summary>
    /// 選択off
    /// </summary>
    public void SelectOffSortController()
    {
        ChangeSelectFrame(false);
    }

    /// <summary>
    /// selectFrame切り替え
    /// </summary>
    /// <param name="check"></param>
    private void ChangeSelectFrame(bool check)
    {
        selectFrame.SetActive(check);
    }

    /// <summary>
    /// 選択状態開始処理
    /// </summary>
    public void StartChoosingSpaceAction()
    {
        //場所の移動
        choosingIndex = selectIndex;
        fruitControllerList[selectIndex].gameObject.transform.SetParent(keyChoosingSpaceTransform, false);
        fruitControllerList[selectIndex].gameObject.transform.SetSiblingIndex(selectIndex);
        choosingFruit = fruitControllerList[selectIndex];
        fruitControllerList.RemoveAt(selectIndex);
    }

    /// <summary>
    /// 選択状態終了処理
    /// </summary>
    public void StopChoosingSpaceAction()
    {
        //場所の移動
        selectIndex = choosingIndex;
        if (choosingFruit != null)
        {
            choosingFruit.gameObject.transform.SetParent(keyFruitOrderTransform, false);
            choosingFruit.gameObject.transform.SetSiblingIndex(choosingIndex);
            fruitControllerList.Insert(choosingIndex, choosingFruit);
        }
    }

    /// <summary>
    /// 選択中表示
    /// </summary>
    /// <param name="select"></param>
    public void SelectFruit(int select)
    {
        for (int i = 0; i < fruitMax + 1; i++)
        {
            fruitControllerList[i].View.UnSelected();
        }
        fruitControllerList[select].View.Selected();
    }

    /// <summary>
    /// 選択中状態からのクリア処理
    /// </summary>
    public void ClearChoosingFruit()
    {
        choosingIndex = selectIndex;
        StopChoosingSpaceAction();
    }

    /// <summary>
    /// カーソル全非表示処理
    /// </summary>
    public void EndSelectFruit()
    {
        selectIndex = 0;
        for (int i = 0; i < fruitMax + 1; i++)
        {
            fruitControllerList[i].View.UnSelected();
        }
    }

    /// <summary>
    /// 操作中オブジェクトのインデックス入れ替え
    /// </summary>
    /// <param name="type"></param>
    public void ReplaceIndex(MovingIndexType type)
    {
        switch (type)
        {
            case MovingIndexType.IncreaseSelect:
                selectIndex++;
                if (selectIndex > fruitMax)
                {
                    selectIndex = 0;
                }
                SelectFruit(selectIndex);
                break;
            case MovingIndexType.DecreaseSelect:
                selectIndex--;
                if (selectIndex < 0)
                {
                    selectIndex = fruitMax;
                }
                SelectFruit(selectIndex);
                break;
            case MovingIndexType.IncreaseChoosing:
                choosingIndex++;
                if (choosingIndex > fruitMax)
                {
                    choosingIndex = 0;
                }
                choosingFruit.gameObject.transform.SetSiblingIndex(choosingIndex);
                break;
            case MovingIndexType.DecreaseChoosing:
                choosingIndex--;
                if (choosingIndex < 0)
                {
                    choosingIndex = fruitMax;
                }
                choosingFruit.gameObject.transform.SetSiblingIndex(choosingIndex);
                break;
        }
    }

    /// <summary>
    /// 並び替えたリストを取得
    /// </summary>
    /// <returns></returns>
    public List<FruitController> GetFruitList()
    {
        return fruitControllerList;
    }

    /// <summary>
    /// リストの並び替え処理
    /// </summary>
    /// <param name="beforeIndex"></param>
    /// <param name="afterIndex"></param>
    /// <param name="fruit"></param>
    public void SetFruitControllerList(int beforeIndex, int afterIndex, FruitController fruit)
    {
        if (beforeIndex >= fruitControllerList.Count || beforeIndex < 0)
        {
            Debug.LogError("fruitControllerList.Count:" + fruitControllerList.Count);
            Debug.LogError("beforeIndex:" + beforeIndex);
            return;
        }
        if (afterIndex >= fruitControllerList.Count || afterIndex < 0)
        {
            Debug.LogError("fruitControllerList.Count:" + fruitControllerList.Count);
            Debug.LogError("afterIndex:" + afterIndex);
            return;
        }
        fruitControllerList.RemoveAt(beforeIndex);
        fruitControllerList.Insert(afterIndex, fruit);
    }

    /// <summary>
    /// リストの並び替え
    /// </summary>
    /// <param name="list"></param>
    public void SortingFruitList(LoadListData data)
    {
        List<FruitController> newFruitList = new List<FruitController>();
        for (int i = 0; i < data.list.Length; i++)
        {
            newFruitList.Add(fruitControllerList[data.list[i] - 1]);
        }
        for (int i = 0; i < data.list.Length; i++)
        {
            newFruitList[i].gameObject.transform.SetSiblingIndex(i);
        }
        fruitControllerList = newFruitList;
    }
}
