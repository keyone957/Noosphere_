using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueObjectController : MonoBehaviour
{
     [SerializeField] private float _scaleSpeed = 0.1f; 
     [SerializeField] private float _minScale = 0.1f;   
     [SerializeField] private float _maxScale = 3f;    
     [SerializeField] private float _rotationSpeed;
     private Vector3 _previousMousePosition;
     [SerializeField] private RectTransform parentPanel;
     [SerializeField] private Canvas parentCanvas;

     private void Start()
     {
          parentPanel = GetComponentInParent<RectTransform>();
          parentCanvas = GetComponentInParent<Canvas>();
     }

     void Update()
     {
          HandleScaling();
          HandleRotation();
     }
     private void HandleScaling()
     {
          float scrollInput = Input.GetAxis("Mouse ScrollWheel");
          if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
               scrollInput += 0.1f;
          if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
               scrollInput -= 0.1f;
          // if (scrollInput != 0)// --> 마우스 위치 기준으로 확대
          // {
          //      Vector2 localPoint;
          //      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
          //               parentPanel,
          //               Input.mousePosition,
          //               parentCanvas.worldCamera,
          //               out localPoint))
          //      {
          //           Vector3 originalScale = transform.localScale;
          //           Vector3 newScale = originalScale + Vector3.one * scrollInput * _scaleSpeed;
          //           newScale = ClampScale(newScale, _minScale, _maxScale);
          //
          //           float scaleFactor = newScale.x / originalScale.x;
          //           Vector3 pivotPosition = parentPanel.TransformPoint(localPoint);
          //           Vector3 direction = transform.position - pivotPosition;
          //           Vector3 newPosition = pivotPosition + direction * scaleFactor;
          //
          //           transform.localScale = newScale;
          //           transform.position = newPosition;
          //      }
          // }
          if (scrollInput != 0)// 중앙 기준으로 확대
          {
               Vector3 originalScale = transform.localScale;
               Vector3 newScale = originalScale + Vector3.one * scrollInput * _scaleSpeed;
               newScale = ClampScale(newScale, _minScale, _maxScale);
               
               transform.localScale = newScale;
          }
     }

     // 마우스 드래그를 이용한 회전 조정
     private void HandleRotation()
     {
          if (Input.GetMouseButtonDown(0))
          {
               _previousMousePosition = Input.mousePosition;
          }

          if (Input.GetMouseButton(0))
          {
               Vector3 deltaMousePosition = Input.mousePosition - _previousMousePosition;
               _previousMousePosition = Input.mousePosition;

               float rotationX = deltaMousePosition.y * _rotationSpeed;
               float rotationY = -deltaMousePosition.x * _rotationSpeed;

               transform.Rotate(Vector3.up, rotationY, Space.World);
               transform.Rotate(Vector3.right, rotationX, Space.World);
          }
          
          float horizontal = Input.GetAxisRaw("Horizontal");
          float vertical = Input.GetAxisRaw("Vertical");

          if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
          {
               float rotateY = horizontal * _rotationSpeed * 1.5f;
               float rotateX = -vertical * _rotationSpeed * 1.5f;

               transform.Rotate(Vector3.right, rotateX, Space.World);
               transform.Rotate(Vector3.up, rotateY, Space.World);
          }
     }
     
     private Vector3 ClampScale(Vector3 scale, float min, float max)
     {
          return new Vector3(
               Mathf.Clamp(scale.x, min, max),
               Mathf.Clamp(scale.y, min, max),
               Mathf.Clamp(scale.z, min, max)
          );
     }
}
