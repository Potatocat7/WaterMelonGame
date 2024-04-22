using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;

public class player : MonoBehaviour
{
    /// <summary> �v���C���[</summary>
    [SerializeField] private GameObject playerObj;
    /// <summary>�g���K�[ </summary>
    [SerializeField] private ObservableCollision2DTrigger observableCollision2DTrigger;
    // Start is called before the first frame update
    void Start()
    {
        ///����
        Observable.EveryUpdate().Subscribe(_ =>
        {
            //�L�[����
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
        ///�ڐG����
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
