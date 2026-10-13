using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    private Material mat; //화면에 사용할 머터리얼
    private Coroutine routine; //코루틴
    private float alpha; //알파값

    private void Start()
    {
        // ToggleFade(true,3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleFade(true,3f);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleFade(false,2.0f);
        }
    }

    //__________________________________________________ Render
    private void OnPostRender()
    {
        if (!mat) //Material이 존재하지 않으면
        {
            mat = new Material(Shader.Find("Hidden/Internal-Colored")); //Material 생성
        }

        mat.SetPass(0);

        GL.PushMatrix(); //GL 렌더링 시작
        GL.LoadOrtho(); //화면 범위를 0~1로 변환

        GL.Begin(GL.QUADS); //Quad 그리기 시작
        GL.Color(new Color(0f, 0f, 0f, alpha)); //알파값을 적용한 색상 지정
        GL.Vertex3(0f, 0f, 0f); //왼쪽 밑
        GL.Vertex3(1f, 0f, 0f); //오른쪽 밑
        GL.Vertex3(1f, 1f, 0f); //오른쪽 위
        GL.Vertex3(0f, 1f, 0f); //왼쪽 위
        GL.End(); //Quad 그리기 종료

        GL.PopMatrix(); //GL 렌더링 종료
    }

    //__________________________________________________ Fade
    public void ToggleFade(bool value, float duration = 1f) //Fade를 수행하는 함수
    {
        if (routine != null) //이미 코루틴이 존재하면
        {
            StopCoroutine(routine); //코루틴 정지
            routine = null; //코루틴 제거
        }
        routine = StartCoroutine(FadeAlpha(value, duration)); //코루틴 실행
    }
    private IEnumerator FadeAlpha(bool value, float duration = 1f) //알파값을 Fade하는 코루틴 함수
    {
        float t = 0f; //0부터 1까지 증가할 변수
        while (t < 1f) //t가 1보다 작으면 반복
        {
            alpha = Mathf.Lerp(value ? 0f : 1f, value ? 1f : 0f, t); //t와 value에 따라 알파값 갱신
            yield return null; //프레임 대기
            t = Mathf.Clamp01(t + Time.deltaTime / duration); //속도를 반영하여 t 증가
        }
        alpha = value ? 1f : 0f; //빠져나오기전 알파값 정리
    }
}
