using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ユーザー操作処理
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤー位置情報 </summary>
    [SerializeField] private Transform playerTransform;
    /// <summary>壁位置情報 </summary>
    [SerializeField] private RectTransform wallLeftTransform;
    [SerializeField] private RectTransform wallRightTransform;
    /// <summary>移動情報 </summary>
    private int vec;
    private bool limitLeft = false;
    private bool limitRight = false;
    /// <summary>次に落とす果物 </summary>
    private FruitController settingFruit;
    /// <summary>
    /// プレイヤー移動処理
    /// </summary>
    /// <param name="vector"></param>
    public void MovingPlayer(bool vector)//方向をboolで参照 true:左　false:右
    {
        //ここに来ることは移動できるということなので解除
        limitLeft = false;
        limitRight = false;
        if (vector)
        {
            vec = -1;
        }
        else
        {
            vec = 1;
        }
        //左右に上限が必要
        Vector3 changePos = this.transform.position;
        changePos.x += vec;
        playerTransform.position = changePos;
        if (changePos.x <= wallLeftTransform.position.x 
            + (settingFruit.GetFruitSize())
            + (wallLeftTransform.sizeDelta.x/2)
            + 1)//落とすときに壁の接触を避けるため
        {
            limitLeft = true;
        }
        if (changePos.x >= wallRightTransform.position.x
            - (settingFruit.GetFruitSize())
            - (wallLeftTransform.sizeDelta.x / 2)
            - 1)//落とすときに壁の接触を避けるため
        {
            limitRight = true;
        }

    }

    /// <summary>
    /// プレイヤーの位置情報再設定
    /// </summary>
    public void ResetEdgePosition()
    {
        //現在地取得
        Vector3 changePos = this.transform.position;
        playerTransform.position = changePos;
        if (changePos.x <= wallLeftTransform.position.x
            + (settingFruit.GetFruitSize())
            + (wallLeftTransform.sizeDelta.x / 2)
            + 1)//落とすときに壁の接触を避けるため
        {
            limitLeft = true;
            changePos.x = wallLeftTransform.position.x
            + (settingFruit.GetFruitSize())
            + (wallLeftTransform.sizeDelta.x / 2)
            + 1;
            this.transform.position = changePos;
        }
        else
        {
            limitLeft = false;
        }
        if (changePos.x >= wallRightTransform.position.x
            - (settingFruit.GetFruitSize())
            - (wallLeftTransform.sizeDelta.x / 2)
            - 1)//落とすときに壁の接触を避けるため
        {
            limitRight = true;
            changePos.x = wallRightTransform.position.x
            - (settingFruit.GetFruitSize())
            - (wallLeftTransform.sizeDelta.x / 2)
            - 1;
            this.transform.position = changePos;
        }
        else
        {
            limitRight = false;
        }
    }

    /// <summary>
    /// 次に落とす果物設定
    /// </summary>
    /// <param name="newFruit"></param>
    public void SettingFruit(FruitController newFruit)
    {
        settingFruit = newFruit;
    }

    /// <summary>
    /// 壁端上限処理左
    /// </summary>
    /// <returns></returns>
    public bool CheckLimitPositionLeft()
    {
        return limitLeft;
    }

    /// <summary>
    /// 壁端上限処理右
    /// </summary>
    /// <returns></returns>
    public bool CheckLimitPositionRight()
    {
        return limitRight;
    }
}
