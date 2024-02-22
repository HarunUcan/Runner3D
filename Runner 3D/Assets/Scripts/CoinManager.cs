using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed, 0f);
    }


}
