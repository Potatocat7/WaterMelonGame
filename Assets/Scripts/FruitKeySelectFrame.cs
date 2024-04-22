using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �I�����̘g
/// </summary>
public class FruitKeySelectFrame : MonoBehaviour
{
    /// <summary>�I�����̘g</summary>
    [SerializeField] private Image selectFrame;
    /// <summary>
    /// �I������������
    /// </summary>
    public void Init()
    {
        UnSelected();
    }

    /// <summary>
    /// �I��
    /// </summary>
    public void Selected()
    {
        selectFrame.gameObject.SetActive(true);
    }

    /// <summary>
    /// ���I��
    /// </summary>
    public void UnSelected()
    {
        selectFrame.gameObject.SetActive(false);
    }

}
