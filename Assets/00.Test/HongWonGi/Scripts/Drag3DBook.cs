using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag3DBook : MonoBehaviour
{
    private Vector3 m_Offset;
    private float m_ZCoord;
    private GameObject _selectedBook;
    private Camera _mainCamera;

    // x, y축 고정을 위한 변수 (드래그 시 고정됨)
    private float _fixedX;
    private float _fixedY;

    [SerializeField] private LayerMask bookLayer;
    
    // 스냅 관련 변수들
    [SerializeField] private Transform[] _snapPoints;  // 스냅될 위치들
    [SerializeField] private float _snapDistances = 0.5f; // 스냅 작동 범위
    [SerializeField] private float _snapSpeed = 10f;// 스냅 시 보간 속도

    // 스냅 중 상태를 관리하는 변수
    private bool _isSnapping = false;
    private Transform _targetSnapPoint = null;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // 스냅 진행 중이면 드래그 입력은 무시하고 스냅 처리 진행
        if (_isSnapping)
        {
            SnapToTarget();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // 지정된 레이어(bookLayer)에 해당하는 객체만 선택
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, bookLayer))
            {
                _selectedBook = hit.collider.gameObject;
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(_selectedBook.transform.position);
                m_ZCoord = screenPos.z;

                // 고정할 x, y 값 저장 (드래그 동안 유지됨)
                _fixedX = _selectedBook.transform.position.x;
                _fixedY = _selectedBook.transform.position.y;

                // 처음 선택 시 z축 오프셋 계산 (마우스 좌표와 실제 z차이 보정)
                Vector3 initialMouseWorldPos = GetMouseWorldPosition();
                m_Offset = new Vector3(0, 0, _selectedBook.transform.position.z - initialMouseWorldPos.z);
            }
        }

        // 드래그 처리: x, y는 고정하고 z축만 마우스 입력에 따라 업데이트
        if (Input.GetMouseButton(0) && _selectedBook != null)
        {
            Vector3 worldPos = GetMouseWorldPosition();
            _selectedBook.transform.position = new Vector3(
                _fixedX,
                _fixedY,
                worldPos.z + m_Offset.z
            );
        }

        // 마우스 버튼을 놓으면 스냅 기능 실행
        if (Input.GetMouseButtonUp(0) && _selectedBook != null)
        {
            Transform closestSnap = FindClosestSnapPoint();
            if (closestSnap != null)
            {
                StartSnapping(closestSnap);
            }
            else
            {
                // 스냅 가능한 포인트가 없으면 선택 해제
                _selectedBook = null;
            }
        }
    }

    // 스크린 좌표를 이용하여 저장된 깊이(m_ZCoord)를 기준으로 월드 좌표 계산
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = m_ZCoord;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // 사용 가능한 스냅 포인트 중 현재 책과 z축 차이가 snapDistance 이하인 가장 가까운 포인트를 찾음
    private Transform FindClosestSnapPoint()
    {
        if (_selectedBook == null || _snapPoints.Length == 0)
            return null;

        Transform closest = null;
        float closestDistance = _snapDistances;
        
        foreach (Transform point in _snapPoints)
        {
            // x, y는 고정되었으므로 z축 차이만 계산
            float distance = Mathf.Abs(_selectedBook.transform.position.z - point.position.z);
            if (distance < closestDistance)
            {
                closest = point;
                closestDistance = distance;
            }
        }

        return closest;
    }

    // 스냅을 시작하기 위해 타겟 스냅 포인트를 설정하고 스냅 상태로 전환
    private void StartSnapping(Transform snapPoint)
    {
        _targetSnapPoint = snapPoint;
        _isSnapping = true;
    }

    // 스냅 대상까지 부드럽게 이동 (x, y는 고정, z만 보간)
    private void SnapToTarget()
    {
        if (_selectedBook == null || _targetSnapPoint == null)
        {
            _isSnapping = false;
            _selectedBook = null;
            return;
        }

        Vector3 currentPos = _selectedBook.transform.position;
        Vector3 targetPos = new Vector3(_fixedX, _fixedY, _targetSnapPoint.position.z);

        // Lerp를 사용해 부드럽게 이동
        _selectedBook.transform.position = Vector3.Lerp(
            currentPos,
            targetPos,
            Time.deltaTime * _snapSpeed
        );

        // 충분히 가까워지면 최종 위치로 고정하고 스냅 종료
        if (Mathf.Abs(_selectedBook.transform.position.z - targetPos.z) < 0.01f)
        {
            _selectedBook.transform.position = targetPos;
            _isSnapping = false;
            _selectedBook = null;
        }
    }
}