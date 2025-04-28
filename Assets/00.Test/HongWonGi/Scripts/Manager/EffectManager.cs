using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;


public class EffectManager : Singleton<EffectManager>
{
    private string _currentEffectID;
    public Action OnEffectEnd;
    [SerializeField] private RawImage _vhsImage;
    [SerializeField] private Volume _vhsVolume;
    public GameObject vhsObj;
    private Camera _mainCamera;
    private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _cameraData;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }
    }

    //
    //SetEffect불러오기
    //
    public void SetEffect(string id)
    {
        _currentEffectID = id;
        // isEffectEnd = false;
        if (DataManager.Instance._effect.TryGetValue(_currentEffectID, out EffectStructure effect))
        {
            if (!string.IsNullOrEmpty(effect.artresourceId))
            {
                DoEffect(effect.artresourceId);

                if (effect.artresourceId == "Artresource_0035" || effect.artresourceId == "Artresource_0040")
                {
                    EscapeUI.Instance.Active();
                }
            }

            if (!string.IsNullOrEmpty(effect.soundresourceId))
            {
                SoundResourceStructure sound = DataManager.Instance._sound[effect.soundresourceId];
                if (sound.soundresourceType == "Sound")
                {
                    SoundManager.Instance.PlaySFX(sound.soundresourceId);
                }
                else
                {
                    SoundManager.Instance.PlayBGM(effect.soundresourceId);
                }
            }
            
            
            //effectType에 따라 onEffectEnd 호출
            if (effect.effectType == "sound" || effect.effectType == "ui" || string.IsNullOrEmpty(effect.effectType))
            {
                StartCoroutine(EndEffect());
            }
        }
        else
        {
            Debug.LogWarning($" {_currentEffectID} 못찾음");
        }
    }

    private void DoEffect(string effectObjectName)
    {
        GameObject effectObj = GameObject.Find(effectObjectName);
        effectObj.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void StartMentalEffect(float value)
    {
        vhsObj.SetActive(true);
        _cameraData.renderPostProcessing = true;

        Color color = _vhsImage.color;
        color.a = value;
        _vhsImage.color = color;
        _vhsVolume.weight = value;
        if (value >= 1f)
        {
            DOTween.To(() => _vhsImage.color.a, x => {
                Color newColor = _vhsImage.color;
                newColor.a = x;
                _vhsImage.color = newColor;
                _vhsVolume.weight = x;
            }, 0f, 3f).SetDelay(0.5f)/*.OnComplete(() => ResetMetalEffect())*/;
        }
    }
    public void ResetMetalEffect()
    {
        vhsObj.SetActive(false);
        
        Color color = _vhsImage.color;
        color.a = 0f;
        _vhsImage.color = color;
        _vhsVolume.weight = 0;
        _cameraData.renderPostProcessing = false;
    }
    
    public async UniTask StartMentalEffectReverse(string eventId=null)  // UniTaskVoid -> UniTask로 변경
    {
        vhsObj.SetActive(true);
        _cameraData.renderPostProcessing = true;
        PlayerInteract.Instance.canInteract = false;
        Color color = _vhsImage.color;
        color.a = 1f;
        _vhsImage.color = color;
        _vhsVolume.weight = 1f;

        await DOTween.To(() => _vhsImage.color.a, x => {
            Color newColor = _vhsImage.color;
            newColor.a = x;
            _vhsImage.color = newColor;
            _vhsVolume.weight = x;
        }, 0f, 1.5f).AsyncWaitForCompletion();

        vhsObj.SetActive(false);
        _cameraData.renderPostProcessing = false;
        PlayerInteract.Instance.canInteract = true;
        if (!string.IsNullOrEmpty(eventId))
        {
            EventManager.Instance.ExecuteEvent(eventId).Forget();
        }
    }
    public async UniTask ReverseProcessEffect()  // UniTaskVoid -> UniTask로 변경
    {
        vhsObj.SetActive(true);
        _cameraData.renderPostProcessing = true;
        PlayerInteract.Instance.canInteract = false;
        Color color = _vhsImage.color;
        color.a = 1f;
        _vhsImage.color = color;
        _vhsVolume.weight = 1f;

        await DOTween.To(() => _vhsImage.color.a, x => {
            Color newColor = _vhsImage.color;
            newColor.a = x;
            _vhsImage.color = newColor;
            _vhsVolume.weight = x;
        }, 0f, 1.5f).AsyncWaitForCompletion();

        vhsObj.SetActive(false);
        _cameraData.renderPostProcessing = false;
        PlayerInteract.Instance.canInteract = true;
        EventManager.Instance.ExecuteEvent("Event_C074").Forget();
    }

    IEnumerator EndEffect()
    {
        yield return null;
        OnEffectEnd?.Invoke();
    }
}