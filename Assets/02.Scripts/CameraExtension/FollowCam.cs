using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform cam;
     private void LateUpdate()
     {
         transform.SetPositionAndRotation(cam.position, cam.rotation);
     }
}
