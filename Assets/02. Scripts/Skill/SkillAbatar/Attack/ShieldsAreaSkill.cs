using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Skill/AreaSkill/ShieldAreaSkill")]
public class ShieldsAreaSkill : AreaSkill
{
    [SerializeField] private GameObject shieldPrefab;
    

    private GameObject shieldInstance;
    private float originAlpha;

    public override void Init()
    {
        base.Init();
        originAlpha = shieldPrefab.GetComponent<SpriteRenderer>().color.a;
    }
    public override void UseSkill()
    {
        player.ArmorAmount += effectAmount;
        if (shieldInstance == null)
        {
            shieldInstance = GameObject.Instantiate(shieldPrefab, player.transform);
            shieldInstance.SetActive(false);
        }

        // 쉴드 크기 설정
        shieldInstance.transform.position = PlayerPosition();
        shieldInstance.transform.localScale = Vector3.one * radius * 2f;

        // 쉴드 콜라이더 크기 설정
        CircleCollider2D collider = shieldInstance.GetComponent<CircleCollider2D>();
        if(collider != null) collider.radius = radius;

        // 쉴드 활성화
        shieldInstance.SetActive(true);

        // 쉴드 페이드인
        FadeController fadeController = shieldInstance.GetComponent<FadeController>();
        fadeController?.FadeIn(originAlpha);

        // 일정 시간 후 꺼주기
        Observable.Timer(System.TimeSpan.FromSeconds(duration))
                  .Subscribe(_ =>
                  {
                      if (fadeController != null)
                          fadeController?.FadeOut(() => {
                              shieldInstance.SetActive(false);  // 페이드 컨트롤러 있으면 페이드 아웃 이후에 액티브 폴스
                              player.ArmorAmount -= effectAmount;
                          }); 
                      else {                                    // 컨트롤러 없으면 걍 바로 끔
                          shieldPrefab.SetActive(false);
                          player.ArmorAmount -= effectAmount;
                      }                               
                  })
                  .AddTo(shieldInstance); // 쉴드에 붙여둠
    }
}

