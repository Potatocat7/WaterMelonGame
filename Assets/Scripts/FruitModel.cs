using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 果物の情報設定
/// </summary>
public class FruitModel
{
    public Sprite Image;
    public string Name;
    public int Number;
    public int Mass;
    /// <summary>果物の大きさ（scale） </summary>
    public float LimitScale;
    /// <summary>落下中フラグ </summary>
    public bool Falling { get; set; }
    /// <summary>スケール変更の倍率 </summary>
    private const float ScaleMagnification = 0.5f;

    public FruitModel(int cardId)
    {
        FruitEntity fruitEntity = Resources.Load<FruitEntity>("Fruit/Fruit_" + cardId.ToString());
        Image = fruitEntity.Image;
        Name = fruitEntity.Name;
        Number = fruitEntity.Number;
        Mass = fruitEntity.Mass;
        LimitScale = fruitEntity.Number * ScaleMagnification;
    }

    public FruitModel(int cardId, int size, int mass)
    {
        FruitEntity fruitEntity = Resources.Load<FruitEntity>("Fruit/Fruit_" + cardId.ToString());
        Image = fruitEntity.Image;
        Name = fruitEntity.Name;
        Number = cardId;
        Mass = mass;
        LimitScale = size * ScaleMagnification;
    }

    public FruitModel(FruitModel model)
    {
        Image = model.Image;
        Name = model.Name;
        Number = model.Number;
        Mass = model.Mass;
        LimitScale = model.LimitScale;
    }

}
