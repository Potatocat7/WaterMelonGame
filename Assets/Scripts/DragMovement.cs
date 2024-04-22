﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
/// <summary>
/// ドラッグ用操作
/// </summary>
public class DragMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>親オブジェクト親のソートコントローラー情報</summary>
    //private ReactiveProperty<SortController> parentSortController = new ReactiveProperty<SortController>();
    private SortController parentSortController;
    /// <summary>親オブジェクトの位置情報</summary>
    private Transform parentTransform;
    /// <summary>親オブジェクトの幅情報</summary>
    private RectTransform parentRectTransform;
    /// <summary>（果物種類の数-1）*2</summary>
    private const int FruitDivisionNumber = 14;
    /// <summary>ドラッグ前のindex</summary>
    private int beforIndex;

    /// <summary>
    /// ドラッグ開始処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Transform transform1;
        beforIndex = (transform1 = transform).GetSiblingIndex();
        parentTransform = transform1.parent;
        transform.SetParent(parentTransform.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    /// <summary>
    /// ドラッグ操作　※　スタート画面のみ
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    /// <summary>
    /// ドロップの位置情報処理
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="width"></param>
    /// <returns>位置に合わせたSiblingIndex</returns>
    private int GetSiblingIndex(float pos, float width)
    {
        float check = width / FruitDivisionNumber;//（果物種類の数-1）*2
        float data = pos / check;
        int siblingIndex = (int)Math.Round(data / 2, 0, MidpointRounding.AwayFromZero);
        return siblingIndex;
    }
    /// <summary>
    /// ドラッグ終了処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentTransform, false);
        parentRectTransform = parentTransform.gameObject.GetComponent<RectTransform>();
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        int siblingIndex = GetSiblingIndex(this.gameObject.transform.position.x, parentRectTransform.rect.width);
        transform.SetSiblingIndex(siblingIndex);

        GetParentSortController(
            success =>
            {
                parentSortController = success;
                parentSortController.SetFruitControllerList(beforIndex, siblingIndex, this.gameObject.GetComponent<FruitController>());
            },
            () =>
            {
                Debug.LogError("NoComponent<SortController>");
            }).Forget();
    }

    private async UniTask GetParentSortController(Action<SortController> success,Action failure)
    {
        if (parentTransform.parent.GetComponent<SortController>())
        {
            success.Invoke(parentTransform.parent.GetComponent<SortController>());
        }
        else
        {
            failure.Invoke();
        }
    }

}
