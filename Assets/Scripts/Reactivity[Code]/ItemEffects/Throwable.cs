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

    [ShowOnly] private bool isGrabbed;

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
        if (isGrabbed)
        {
            posCounter++;
            posCounter %= 10;

            previousPositions[posCounter] = transform.position;
        }
    }

    public void Grab(Transform handTransform)
    {
        Interact();

        Debug.Log("grabbed");
        transform.SetParent(handTransform, true);

        isGrabbed = true;
    }

    public void Release()
    {
        Debug.Log("released");

        isGrabbed = false;

        Vector3 throwVelocity = ThrowVelocity();
        Throw(previousPositions[0] - previousPositions[1]);

        previousPositions = new Vector3[10];
        posCounter = 0;
    }

    public void Throw(Vector3 direction, float force =1)
    {
        Debug.Log(direction);
        throwCollider.enabled = true;
        if (force == 1)
            rb.velocity = direction;
        else
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

    private Vector3 ThrowVelocity()
    {
        Vector3 averageVelocity = previousPositions[0] - previousPositions[1];

        for (int x = 1; x < previousPositions.Length-1; x++)
        {
            averageVelocity += previousPositions[x] - previousPositions[x + 1];
        }
        averageVelocity /= 9;

        return averageVelocity;
    }
}
