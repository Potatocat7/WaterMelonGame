using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// スタート画面処理
/// </summary>
public class StartController : MonoBehaviour
{
    private enum SortType
    {
        Size,
        Mass,
    }
    /// <summary>ゲーム開始ボタン </summary>
    [SerializeField] private Button startButton;
    /// <summary>ソートコントローラー </summary>
    [SerializeField] private SortController sizeSortController;
    /// <summary>ソートコントローラー </summary>
    [SerializeField] private SortController massSortController;
    /// <summary>果物のナンバー最大と最小 </summary>
    private const int FruitNumMax = 8;
    /// <summary>設定キー入力状態</summary>
    private bool settings = false;
    /// <summary>ソート選択中状態</summary>
    private bool selecting = false;
    /// <summary>選択中状態</summary>
    private bool choosing = false;
    /// <summary>選択するソートの種類</summary>
    private SortType sortType = SortType.Size;
    /// <summary>次のシーン名 </summary>
    private const string NextScene = "MainScene";

    //シングルトン化
    public static StartController Instance;
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
        massSortController.Init(FruitNumMax - 1);
        sizeSortController.Init(FruitNumMax - 1);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.O))
        .Subscribe(_ =>
        {
            settings = !settings;
            if (settings)
            {
                sortType = SortType.Size;
                massSortController.SelectOffSortController();
                sizeSortController.SelectOnSortController();
            }
            else
            {
                if (choosing)
                {
                    sizeSortController.ClearChoosingFruit();
                    massSortController.ClearChoosingFruit();
                    choosing = false;
                }
                selecting = false;
                sizeSortController.EndSelectFruit();
                massSortController.EndSelectFruit();
                massSortController.SelectOffSortController();
                sizeSortController.SelectOffSortController();

            }
        }).AddTo(this);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.D))
        .Subscribe(_ =>
        {
            if (!settings) return;
            if (!selecting) return;
            if (choosing)
            {
                switch (sortType)
                {
                    case SortType.Size:
                        sizeSortController.ReplaceIndex(SortController.MovingIndexType.IncreaseChoosing);
                        break;
                    case SortType.Mass:
                        massSortController.ReplaceIndex(SortController.MovingIndexType.IncreaseChoosing);
                        break;
                }
            }
            else
            {
                switch (sortType)
                {
                    case SortType.Size:
                        sizeSortController.ReplaceIndex(SortController.MovingIndexType.IncreaseSelect);
                        break;
                    case SortType.Mass:
                        massSortController.ReplaceIndex(SortController.MovingIndexType.IncreaseSelect);
                        break;
                }
            }
        }).AddTo(this);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.A))
        .Subscribe(_ =>
        {
            if (!settings) return;
            if (!selecting) return;
            if (choosing)
            {
                switch (sortType)
                {
                    case SortType.Size:
                        sizeSortController.ReplaceIndex(SortController.MovingIndexType.DecreaseChoosing);
                        break;
                    case SortType.Mass:
                        massSortController.ReplaceIndex(SortController.MovingIndexType.DecreaseChoosing);
                        break;
                }
            }
            else
            {
                switch (sortType)
                {
                    case SortType.Size:
                        sizeSortController.ReplaceIndex(SortController.MovingIndexType.DecreaseSelect);
                        break;
                    case SortType.Mass:
                        massSortController.ReplaceIndex(SortController.MovingIndexType.DecreaseSelect);
                        break;
                }
            }
        }).AddTo(this);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.W))
        .Subscribe(_ =>
        {
            if (!settings) return;
            if ((int)sortType == System.Enum.GetValues(typeof(SortType)).Cast<int>().Max())
            {
                sortType = SortType.Size;
            }
            else
            {
                sortType++;
            }
            switch (sortType)
            {
                case SortType.Size:
                    massSortController.SelectOffSortController();
                    sizeSortController.SelectOnSortController();
                    break;
                case SortType.Mass:
                    massSortController.SelectOnSortController();
                    sizeSortController.SelectOffSortController();
                    break;
            }
        }).AddTo(this);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.S))
        .Subscribe(_ =>
        {
            if (!settings) return;
            if ((int)sortType == 0)
            {
                sortType = SortType.Mass;
            }
            else
            {
                sortType--;
            }
            switch (sortType)
            {
                case SortType.Size:
                    massSortController.SelectOffSortController();
                    sizeSortController.SelectOnSortController();
                    break;
                case SortType.Mass:
                    massSortController.SelectOnSortController();
                    sizeSortController.SelectOffSortController();
                    break;
            }
        }).AddTo(this);
        Observable.EveryUpdate()
        .Where(_ => Input.GetKeyDown(KeyCode.Space))
        .Subscribe(_ =>
        {
            if (settings)
            {
                if (selecting)
                {
                    choosing = !choosing;
                    if (choosing)
                    {
                        switch (sortType)
                        {
                            case SortType.Size:
                                sizeSortController.StartChoosingSpaceAction();
                                break;
                            case SortType.Mass:
                                massSortController.StartChoosingSpaceAction();
                                break;
                        }
                    }
                    else
                    {
                        switch (sortType)
                        {
                            case SortType.Size:
                                sizeSortController.StopChoosingSpaceAction();
                                break;
                            case SortType.Mass:
                                massSortController.StopChoosingSpaceAction();
                                break;
                        }
                    }
                }
                else
                {
                    massSortController.SelectOffSortController();
                    sizeSortController.SelectOffSortController();
                    selecting = !selecting;
                    switch (sortType)
                    {
                        case SortType.Size:
                            sizeSortController.SelectFruit(0);
                            break;
                        case SortType.Mass:
                            massSortController.SelectFruit(0);
                            break;
                    }
                }
            }
            else
            {
                SaveFruitList();
                ///メインシーンへの遷移
                SceneManager.LoadScene(NextScene);
            }
        }).AddTo(this);

        startButton.onClick.AsObservable()
        .Subscribe(_ => 
        {
            SaveFruitList();
            ///メインシーンへの遷移
            SceneManager.LoadScene(NextScene);
        }).AddTo(this);
    }
    
    private void SaveFruitList()
    {
        List<FruitModel> savelist = new List<FruitModel>();
        int sizeNum = 0;
        int massNum = 0;
        for (int i = 0; i < massSortController.GetFruitList().Count; i++)
        {
            for (int size = 0; size < sizeSortController.GetFruitList().Count; size++)
            {
                if(i+1 == sizeSortController.GetFruitList()[size].Model.Number)
                {
                    sizeNum = size + 1;
                }

            }
            for (int mass = 0; mass < massSortController.GetFruitList().Count; mass++)
            {
                if (i + 1 == massSortController.GetFruitList()[mass].Model.Number)
                {
                    massNum = mass + 1;
                }

            }

            FruitModel model = new FruitModel(
                i + 1,
                sizeNum,
                massNum
                );
            savelist.Add(model);
        }
        ///savedataに保存
        SaveData.Instance.SetFruitModels(savelist);

        PlayfabManager.Instance.UpdateUserDataAsync(sizeSortController.GetFruitList(), 
                                                    massSortController.GetFruitList(),
            () =>
            {
                ///savedataに保存
                SaveData.Instance.SetFruitModels(savelist);
                ///メインシーンへの遷移
                SceneManager.LoadScene("MainScene");
            },
            () =>
            {

            }).Forget();
    }

    /// <summary>
    /// 読み込みデータから並び替え
    /// </summary>
    /// <param name="size"></param>
    /// <param name="mass"></param>
    public void SettingSortList(LoadListData size, LoadListData mass)
    {
        massSortController.SortingFruitList(mass);
        sizeSortController.SortingFruitList(size);
    }

}
