using UnityEngine;

/// <summary>
/// 무기의 강화 수치를 한 곳에 모아 관리한다.
/// 업그레이드 카드에서의 선택 결과는 이 스크립트의 함수를 호출해서 들어오게 한다.
/// 무기 스크립트는 이 값을 읽어서 동작한다.
/// </summary>
public class PlayerWeaponStatsRuntime : MonoBehaviour
{
    [SerializeField] private int bulletPierceCount = 0; // 총알이 추가로 관통할 수 있는 회수.
    [SerializeField] private float bulletDamage = 1.0f; // 총알 대미지.
    [SerializeField] private float bulletHitCooldownSec = 0.15f;    // 대미지를 주는 쿨타임.

    [SerializeField] private int bladeCount = 3;    // 주변을 도는 칼날의 개수.
    [SerializeField] private float bladeRadius = 2.0f;  // 칼날의 반경.
    [SerializeField] private float bladeRotationSpeedDeg = 180.0f;  // 칼날의 초당 회전 각도.
    [SerializeField] private float bladeDamage = 1.0f;  // 칼날의 대미지.
    [SerializeField] private float bladeHitCooldownSec = 0.2f;  // 대미지를 주는 쿨타임.

    public int GetBulletPierceCount()
    {
        return bulletPierceCount;
    }

    public float GetBulletDamage()
    {
        return bulletDamage;
    }

    public float GetBulletHitCooldownSec()
    {
        return bulletHitCooldownSec;
    }

    public int GetBladeCount()
    {
        return bladeCount;
    }

    public float GetBladeRadius()
    {
        return bladeRadius;
    }

    public float GetBladeRotationSpeedDeg()
    {
        return bladeRotationSpeedDeg;
    }

    public float GetBladeDamage()
    {
        return bladeDamage;
    }

    public float GetBladeHitCooldownSec()
    {
        return bladeHitCooldownSec;
    }

    /// <summary>
    /// 관통 회수와 대미지를 더한다.
    /// </summary>
    /// <param name="pierceDelta"></param>
    /// <param name="damageDelta"></param>
    public void AddPierceBulletUpgrade(int pierceDelta, float damageDelta)
    {
        bulletPierceCount += pierceDelta;
        bulletDamage += damageDelta;

        Debug.Log("bulletPierceCount: " + bulletPierceCount);
        Debug.Log("bulletDamage: " + bulletDamage);
    }

    /// <summary>
    /// 회전 칼날의 개수, 대미지, 회전 속도 값을 더한다.
    /// </summary>
    /// <param name="countDelta"></param>
    /// <param name="damageDelta"></param>
    /// <param name="rotationSpeedDelta"></param>
    public void AddOrbitBladesUpgrade(int countDelta, float damageDelta, float rotationSpeedDelta)
    {
        bladeCount += countDelta;
        bladeDamage += damageDelta;
        bladeRotationSpeedDeg += rotationSpeedDelta;

        Debug.Log("bladeCount: " + bladeCount);
        Debug.Log("bladeDamage: " + bladeDamage);
        Debug.Log("bladeRotationSpeedDeg: " + bladeRotationSpeedDeg);
    }
}
