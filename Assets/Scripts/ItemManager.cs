using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public Item[] ItemBox;
    ///�V���O���g��
    public static ItemManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void GetItem(int siblingIndex)
    {
        Debug.Log("siblingIndex:" + siblingIndex);
        Debug.Log("ItemBox:" + ItemBox[siblingIndex].gameObject.transform.GetSiblingIndex());
        ///siblingIndex�ɑΉ�����Item[]���Ă�
    }
}
