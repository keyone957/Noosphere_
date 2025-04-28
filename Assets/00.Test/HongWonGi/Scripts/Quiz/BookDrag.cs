using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class BookDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    [SerializeField] private GameObject _placeholderObject;
    [SerializeField] public int bookIdx;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private int _originalSiblingIndex;
    private Transform _originalParent;
    private int _currentPlaceholderIndex = -1;
    private RectTransform _layoutGroupRect;

    // 민감도 조절을 위한 변수들
    private float _lastUpdateTime = 0f;
    private float _updateDelay = 0.1f; // 업데이트 간격 (초)
    private Vector2 _lastMousePosition;
    private float _minMoveDelta = 60f; // 최소 이동 거리 (픽셀)
    private float _proximityThreshold = 75f; // 다른 이미지와의 최소 감지 거리

    private BookShelfManager _bookShelfManager;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _originalParent = transform.parent;
        _layoutGroupRect = _originalParent.GetComponent<RectTransform>();
    }

    void Start()
    {
        _bookShelfManager = transform.GetComponentInParent<BookShelfManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalSiblingIndex = transform.GetSiblingIndex();
        _lastMousePosition = eventData.position;
        //horizontal layout 사용하기 때문에 부모 바꿔서 자동정렬 된 것처럼 보이게함
        transform.SetParent(_canvas.transform);

        _placeholderObject.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        int randNum = Random.Range(57, 61);
        SoundManager.Instance.PlaySFX($"Soundresource_0{randNum}");
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = Input.mousePosition;

        // 시간과 거리 체크
        if (Time.time - _lastUpdateTime > _updateDelay &&
            Vector2.Distance(_lastMousePosition, eventData.position) > _minMoveDelta)
        {
            _lastMousePosition = eventData.position;
            _lastUpdateTime = Time.time;

            if (IsInsideLayoutGroup(eventData.position) && IsValidDropPosition(eventData.position))
            {
                int newIndex = GetInsertIndex(eventData.position);
                if (!_placeholderObject.activeSelf || _currentPlaceholderIndex != newIndex)
                {
                    _placeholderObject.transform.SetParent(_originalParent);
                    _placeholderObject.SetActive(true);
                    _placeholderObject.transform.SetSiblingIndex(newIndex);
                    _currentPlaceholderIndex = newIndex;
                }
            }
            else
            {
                _placeholderObject.SetActive(false);
                _currentPlaceholderIndex = -1;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsInsideLayoutGroup(eventData.position) && _placeholderObject.activeSelf)
        {
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_placeholderObject.transform.GetSiblingIndex());
        }
        else
        {
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
        }

        _placeholderObject.SetActive(false);
        _bookShelfManager.CheckBookOrder();
    }

    private bool IsInsideLayoutGroup(Vector2 position)
    {
        //범위 밖에 나가면 placeholder안뜨게
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _layoutGroupRect,
            position,
            _canvas.worldCamera,
            out localPoint);

        Rect rect = _layoutGroupRect.rect;
        return rect.Contains(localPoint);
    }

    private bool IsValidDropPosition(Vector2 position)
    {
        foreach (Transform child in _originalParent)
        {
            if (child != transform && child != _placeholderObject.transform)
            {
                RectTransform childRect = child as RectTransform;
                if (childRect != null)
                {
                    float distance = Mathf.Abs(position.x - childRect.position.x);
                    if (distance < _proximityThreshold)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private int GetInsertIndex(Vector2 mousePosition)
    {
        //마우스 위치에 따라 몇 번째로 들어갈까 계산 
        for (int i = 0; i < _originalParent.childCount; i++)
        {
            RectTransform child = _originalParent.GetChild(i) as RectTransform;
            if (child != null && child.gameObject != _placeholderObject && child.gameObject != gameObject)
            {
                if (mousePosition.x < child.position.x + (child.rect.width / 2))
                {
                    //중간점 기준으로 판단
                    return i;
                }
            }
        }

        return _originalParent.childCount;
    }
}