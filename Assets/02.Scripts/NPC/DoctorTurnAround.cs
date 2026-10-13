using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorTurnAround : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationValue;
    void OnEnable()
    {
        if (DataManager.Instance._events["Event_A008"].isExecuted)
        {
            transform.eulerAngles = _rotationValue;
        }
    }
}
