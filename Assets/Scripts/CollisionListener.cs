﻿using System;
using UnityEngine;

// 충돌 감지 전용 Monobehaviour
public class CollisionListener : MonoBehaviour
{
    public Action<Collision> OnCollisionEnterCallBack;
    public Action<Collision> OnCollisionExitCallBack;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterCallBack?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
         OnCollisionExitCallBack?.Invoke(collision);
    }
}
