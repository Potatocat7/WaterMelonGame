using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//エディターで呼び出せるように
[CreateAssetMenu(fileName = "FruitEntity", menuName = "Create FruitEntity")]
public class FruitEntity : ScriptableObject
{
    public Sprite Image;
    public string Name;
    public int Number;
    public int Mass;
}
