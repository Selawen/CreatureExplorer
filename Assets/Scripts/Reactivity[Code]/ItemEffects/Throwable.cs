using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Throwable : StatusEffect, IInteractable, IThrowable
{
    public Sprite InventoryGraphic { get { return inventoryGraphic; } }
    public Sprite HoverGraphic { get { return hoverGraphic; } }
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "Pick Up [E]";

    [SerializeField] private float splatVelocity = 2;
    [SerializeField] private Sprite inventoryGraphic;
    [SerializeField] private Sprite hoverGraphic;

    [ShowOnly] private bool IsGrabbed;

    private Vector3[] previousPositions;
    private int posCounter = 0;

    private Rigidbody rb;
    private Collider throwCollider;

    // TODO: implement throwing in VR
    private void Awake()
    {
        previousPositions = new Vector3[10];
        previousPositions[0] = transform.position;

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

    private void FixedUpdate()
    {
        if (IsGrabbed)
        {
            posCounter++;
            posCounter %= 10;

            previousPositions[posCounter] = transform.position;
        }
    }

    public void Grab(Transform handTransform)
    {
        transform.SetParent(handTransform, true);

        IsGrabbed = true;
    }

    public void Release()
    {
        IsGrabbed = false;
        previousPositions = new Vector3[10];
        posCounter = 0;
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
