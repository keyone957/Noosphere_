using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    private int _snapOffsst = 30;
    [SerializeField] private GameObject _piecePos; //정답 위치 오브젝트
    public bool isRight = false; // 각 퍼즐이 정답 위치에 들어갔을 때 다시 못움직이게
    [SerializeField] public string _glassNum;
    private Vector2 _initialAnchoredPosition;
    private Transform _initialParent;
    private RectTransform _rectTransform;
    public Action OnResetPuzzle;
    

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        // UI 요소의 초기 anchored position과 부모 저장
        _initialAnchoredPosition = _rectTransform.anchoredPosition;
        _initialParent = transform.parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       SoundManager.Instance.PlaySFX($"Soundresource_0{_glassNum}");
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isRight) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Vector3.Distance(_piecePos.transform.position, transform.position) < _snapOffsst)
        {
            transform.SetParent(_piecePos.transform);
            transform.localPosition = Vector3.zero;
            isRight = true;
            MirrorPuzzleManager.Instance.CheckAnswer();
        }
    }

    public void ResetPosition()
    {
        isRight = false;
        transform.SetParent(_initialParent);
        _rectTransform.anchoredPosition = _initialAnchoredPosition;
    }

    private void OnEnable()
    {
        if (MirrorPuzzleManager.Instance != null)
        {
            MirrorPuzzleManager.Instance.OnResetPuzzle += ResetPosition;
        }
    }

    private void OnDisable()
    {
        if (MirrorPuzzleManager.Instance != null)
        {
            MirrorPuzzleManager.Instance.OnResetPuzzle -= ResetPosition;
        }
    }
}