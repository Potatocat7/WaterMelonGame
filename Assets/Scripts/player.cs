using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;

public class player : MonoBehaviour
{
    /// <summary> プレイヤー</summary>
    [SerializeField] private GameObject playerObj;
    /// <summary>トリガー </summary>
    [SerializeField] private ObservableCollision2DTrigger observableCollision2DTrigger;
    // Start is called before the first frame update
    void Start()
    {
        ///操作
        Observable.EveryUpdate().Subscribe(_ =>
        {
            //キー入力
            if (Input.GetKey(KeyCode.A))
            {
                playerObj.transform.position = playerObj.transform.position + Vector3.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                playerObj.transform.position = playerObj.transform.position + Vector3.right;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                playerObj.transform.position = playerObj.transform.position + Vector3.up;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                playerObj.transform.position = playerObj.transform.position + Vector3.down;
            }
        });
        ///接触判定
        IObservable<Collision2D> onCollisionEnterAsObservable = observableCollision2DTrigger.OnCollisionEnter2DAsObservable();
        onCollisionEnterAsObservable.Subscribe(collision =>
        {
            ItemManager.Instance.GetItem(collision.gameObject.transform.GetSiblingIndex());
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
