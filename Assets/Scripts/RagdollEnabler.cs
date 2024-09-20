using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnabler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform ragdollRoot;

    private Rigidbody[] rigidbodies;
    private CharacterJoint[] characterJoints;
    private Collider[] colliders;

    private void Awake()
    {
        rigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        characterJoints = ragdollRoot.GetComponentsInChildren<CharacterJoint>();
        colliders = ragdollRoot.GetComponentsInChildren<Collider>();

    }

    [ContextMenu("Enable Ragdoll")]
    public void EnableRagdoll()
    {
        animator.enabled = false; // Disable animator to allow physics control

        // Enable physics and collisions for the ragdoll
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // Physics controls the bones now
            rb.velocity = Vector3.zero; // Reset velocity
            rb.detectCollisions = true; // Enable collision detection
        }

        // Enable colliders for ragdoll
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

    }

    [ContextMenu("Enable Animator")]
    public void EnableAnimator()
    {
        animator.enabled = true; // Enable animator to control bones

        // Disable physics for ragdoll rigidbodies
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true; // Animator controls bones, no physics
            rb.detectCollisions = true; // Disable collision detection
        }

        // Disable colliders to avoid physics interactions
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

    }
}
