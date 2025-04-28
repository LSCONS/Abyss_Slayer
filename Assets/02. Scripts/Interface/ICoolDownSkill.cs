using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 이 스킬이 쿨타임을 줄이는 스킬이라는 것을 명시해주는 인터페이스
/// 해당 인터페이스를 상속 받는 스킬은 스킬 적중 시 쿨타임 다운에 관여하지 않음.
/// </summary>
public interface ICoolDownSkill { }
