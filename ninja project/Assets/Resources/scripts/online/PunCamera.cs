using Photon.Pun;
using UnityEngine;

public class PunCamera : MonoBehaviourPun
{
    private Camera cm;
    void Start()
    {
        cm = GetComponent<Camera>();
        if (!photonView.IsMine)
        {
            cm.depth -= 2;
        }
    }
}