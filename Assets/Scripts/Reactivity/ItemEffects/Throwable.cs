using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Throwable : StatusEffect, IInteractable
{
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Pick up";
    
    [SerializeField] private float splatVelocity = 2;

    private Rigidbody rb;
    private Collider throwCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        throwCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude > splatVelocity && collision.gameObject.TryGetComponent(out Creature creature))
        {
            GetComponent<MeshRenderer>().enabled = false;

            TriggerStatusEffect(creature);
        }
    }

    public void Throw(Vector3 direction, float force)
    {
        throwCollider.enabled = true;
        rb.AddForce(direction * force);
        if (TryGetComponent(out Food food))
        {
            food.ActivatePhysics();
        }
        else
        {
            transform.SetParent(null);
            throwCollider.isTrigger = false;
            rb.useGravity = true;
        }
    }

    public void Interact()
    {
        rb.velocity = Vector3.zero;
        throwCollider.enabled = false;
        rb.useGravity = false;
        if(TryGetComponent(out Food food))
        {
            food.StopAllCoroutines();
        }

    }
}
