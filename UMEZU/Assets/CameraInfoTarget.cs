using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class CameraInfoTarget : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera CmCam;

    public void ChangeInfoTarget(Transform infoTarget)
    {
        Transform playerSlime = GameObject.FindGameObjectWithTag("Player").transform;
        if (CmCam == null) return;
        if (playerSlime == null) return;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            CmCam.m_Follow = infoTarget;
        })
            .AppendInterval(1)
            .OnComplete(() =>
            {
                CmCam.m_Follow = playerSlime;
            });

    }


}
