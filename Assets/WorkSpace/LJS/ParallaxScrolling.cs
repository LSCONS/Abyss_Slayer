using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    // 카메라 움직임에 얼마나 민감하게 따라올지 결정하는 값
    // 낮을수록 멀리 있는 배경처럼 느껴짐 (느리게 움직임)
    public float parallaxSpeed = 0.5f;

    // 메인 카메라 Transform
    private Transform cam;

    // 이전 프레임의 카메라 위치 저장용
    private Vector3 previousCamPos;
    void Start()
    {
        SetCamera();
    }


    void LateUpdate()
    {
        Parallax();
    }

    private void SetCamera()
    {
        // 현재 메인 카메라를 찾음
        cam = Camera.main.transform;

        // 시작 시점에 카메라 위치 저장
        previousCamPos = cam.position;
    }
    /// <summary>
    /// 배경 이미지 움직이게하는 메서드
    /// </summary>
    private void Parallax()
    {
        // 현재 카메라 위치와 이전 위치의 차이 계산
        Vector3 deltaMovement = cam.position - previousCamPos;
        // 해당 차이값에 parallaxSpeed를 곱해서 배경을 이동시킴
        // x와 y 방향 모두 적용 가능 (예: 위아래 움직임도 가능)
        transform.position += new Vector3(deltaMovement.x * parallaxSpeed,
                                          deltaMovement.y * parallaxSpeed * 0,
                                          0);
        // 다음 프레임을 위해 현재 카메라 위치를 저장해둠
        previousCamPos = cam.position;
    }
}
