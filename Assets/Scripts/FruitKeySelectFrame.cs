using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 選択時の枠
/// </summary>
public class FruitKeySelectFrame : MonoBehaviour
{
    /// <summary>選択時の枠</summary>
    [SerializeField] private Image selectFrame;
    /// <summary>
    /// 選択処理初期化
    /// </summary>
    public void Init()
    {
        UnSelected();
    }

    /// <summary>
    /// 選択中
    /// </summary>
    public void Selected()
    {
        selectFrame.gameObject.SetActive(true);
    }

    /// <summary>
    /// 未選択
    /// </summary>
    public void UnSelected()
    {
        selectFrame.gameObject.SetActive(false);
    }

}
