using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirewallProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed       = 8f;
    public float maxDistance = 3f;

    [Header("Life & Fade")]
    public float lifeTime = 1f;
    public float fadeTime = 0.5f;

    [Header("Damage")]
    public float initialDamage = 10f;
    public float burnDamage    = 2f;
    public float burnDuration  = 3f;
    public float burnInterval  = 1f;

    private Vector3 startPos;
    private Vector3 moveDir;
    private Renderer rend;
    private HashSet<LivingThing> burned = new HashSet<LivingThing>();

    // เรียกก่อนปล่อย projectile
    public void Initialize(Vector3 direction)
    {
        moveDir  = direction.normalized;
        Debug.Log($"Initialize got dir={moveDir}");
        startPos = transform.position;
        transform.rotation = Quaternion.LookRotation(moveDir);
    }

    void Awake()
    {
        // รองรับได้ทั้ง ParticleSystemRenderer หรือ MeshRenderer
        rend     = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        StartCoroutine(LifeAndFade());
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            enabled = false;
    }

    IEnumerator LifeAndFade()
    {
        yield return new WaitForSeconds(lifeTime);

        if (rend != null)
        {
            float t = 0f;
            Color c = rend.material.color;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, t/fadeTime);
                rend.material.color = c;
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            var lt = other.GetComponent<LivingThing>();
            if (lt != null)
            {
                lt.TakeDamage(initialDamage);
                if (!burned.Contains(lt))
                {
                    burned.Add(lt);
                    StartCoroutine(ApplyBurn(lt));
                }
            }
        }
    }

    IEnumerator ApplyBurn(LivingThing target)
    {
        float elapsed = 0f;
        while (elapsed < burnDuration)
        {
            yield return new WaitForSeconds(burnInterval);
            target.TakeDamage(burnDamage);
            elapsed += burnInterval;
        }
    }
}
