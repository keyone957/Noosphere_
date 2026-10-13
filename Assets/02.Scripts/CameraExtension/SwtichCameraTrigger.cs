using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class SwtichCameraTrigger : MonoBehaviour
{
    [Header("모든 시네머신 버츄얼 카메라 (리스트에 모두 등록)")]
    public List<GameObject> virtualCameras;   

    [Header("트리거 시 활성화할 카메라")]
    public GameObject targetCamera;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ActivateTargetCamera();
        }
    }

    // 지정한 targetCamera만 활성화, 나머지는 비활성화하는 메서드
    private void ActivateTargetCamera()
    {
        foreach (var cam in virtualCameras)
        {
            bool shouldBeActive = (cam == targetCamera);
            if(cam.activeSelf != shouldBeActive)
            {
                cam.SetActive(shouldBeActive);
            }
        }
    }
}
