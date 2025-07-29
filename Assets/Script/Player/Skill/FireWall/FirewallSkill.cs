using UnityEngine;
using System.Collections;

public class FirewallSkill : ISkill
{
    public string Name => "Firewall";
    public bool IsOnCooldown { get; private set; }

    // References & params
    private readonly PlayerController _pc;
    private readonly GameObject       _prefab;
    private readonly Transform        _playerTransform;
    private readonly Camera           _playerCamera;
    private readonly float            _castDelay;
    private readonly float            _lockDuration;
    private readonly float            _speedMultiplier;
    private readonly float            _cooldown;
    private readonly float            _offset;
    private readonly float            _height;

    public FirewallSkill(
        PlayerController pc,
        GameObject prefab,
        Transform playerTransform,
        Camera playerCamera,
        float castDelay,
        float lockDuration,
        float speedMultiplier,
        float cooldown,
        float offset,
        float height)
    {
        _pc              = pc;
        _prefab          = prefab;
        _playerTransform = playerTransform;
        _playerCamera    = playerCamera;
        _castDelay       = castDelay;
        _lockDuration    = lockDuration;
        _speedMultiplier = speedMultiplier;
        _cooldown        = cooldown;
        _offset          = offset;
        _height          = height;
    }

    public IEnumerator Activate()
    {
        IsOnCooldown = true;
        if (_pc.playerWeapon != null)
            _pc.playerWeapon.SetActive(false);

        // 1) Lock movement + slow
        _pc.StartCoroutine(_pc.SkillLock(_lockDuration, _speedMultiplier));

        // 2) Trigger animation
        _pc.animator?.SetTrigger("Firewall");

        // 3) Wait cast delay
        yield return new WaitForSeconds(_castDelay);

        // 4) Spawn projectile
        CastFirewall();
        yield return new WaitForSeconds(1.2f);
        if (_pc.playerWeapon != null)
            _pc.playerWeapon.SetActive(true);
        
        

        // 5) Wait cooldown
        yield return new WaitForSeconds(_cooldown);
        IsOnCooldown = false;
    }

    private void CastFirewall()
    {
        Vector3 dir = _playerCamera.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        Vector3 origin = _playerTransform.position
                       + dir * _offset
                       + Vector3.up * _height;

        var go = Object.Instantiate(_prefab, origin, Quaternion.identity);
        go.GetComponent<FirewallProjectile>().Initialize(dir);
        PlayerAudioManager.Instance.PlayOneShot(PlayerAudioManager.Instance.firewall);
    }
}
