using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioClip[] hitSounds;

    public static event Action<Enemy> OnEnemyDied;

    private Health health;
    private DamageFlash damageFlash;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private AudioSource audioSource;

    private bool isDead = false;
    private Vector3 targetDestination;
    private int punchIndex = 0;
    private bool isRunning = true;
    private bool isPunching = false;

    private Transform hitColliderTransform;
    private int playerLayer;
    private int damage = 15;

    private void Awake()
    {
        health = GetComponent<Health>();
        damageFlash = GetComponent<DamageFlash>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        hitColliderTransform = transform.GetChild(3);

        playerLayer = LayerMask.GetMask("Player");
    }
    private void OnEnable()
    {
        isDead = false;
        StopPauseNavMeshAgent(false);
        GetComponent<RagdollEnabler>().EnableAnimator();
        health.SetToMax();

        health.OnHealthZero += Health_OnHealthZero;
    }
    private void OnDisable()
    {
        health.OnHealthZero -= Health_OnHealthZero;
    }
    private void Update()
    {
        targetDestination = PlayerController.Instance.transform.position;
        if (!isDead)
        {
            animator.SetBool("isRunning", isRunning);
            navMeshAgent.SetDestination(targetDestination);

            if (Vector3.Distance(transform.position, targetDestination) < 2f)
            {
                if (!isPunching)
                {
                    StartCoroutine(Punch());
                }
            }
            else
            {
                isPunching = false;
                StopPauseNavMeshAgent(false);
                isRunning = true;
            }
        }
    }
    private IEnumerator Punch()
    {
        isPunching = true;

        StopPauseNavMeshAgent(true);
        isRunning = false;
        animator.SetInteger("PunchIndex", punchIndex++);
        punchIndex %= 2;
        animator.SetTrigger("Punch");

        yield return new WaitForSeconds(1f);

        isPunching = false;
    }

    private void Health_OnHealthZero()
    {
        print(name + " is died");
        isDead = true;
        StopPauseNavMeshAgent(true);
        OnEnemyDied?.Invoke(this);
        //Destroy(gameObject, 5f);
        Lean.Pool.LeanPool.Despawn(this, 2f);
    }

    public void TakeDamage(int damage)
    {
        health.ChangeHealth(-damage);
        damageFlash.CallDamageFlash();
    }
    public bool GetIsDead() => isDead;
    public void StopPauseNavMeshAgent(bool cond) => navMeshAgent.isStopped = cond;
    private void OnDrawGizmos()
    {
        if (hitColliderTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitColliderTransform.position, 1f);
        }
    }
    public void PunchAnimEvent()
    {
        Collider[] hits = Physics.OverlapSphere(hitColliderTransform.position, 1f, playerLayer);
        if (hits.Length > 0)
        {
            PlayerController.Instance.GetComponent<Health>().ChangeHealth(-damage);
            PlayRandomClip();
            CameraShake.Instance.Shake(0.15f, 0.2f);
        }
    }
    private void PlayRandomClip()
    {
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
        audioSource.Play();
    }
}
