using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Vector3 TrueOffset { get; private set;}

    [SerializeField] private bool followPosition;
    [SerializeField] private bool maintainCurrentOffset;

    [SerializeField] private Vector3 customOffset;

    [SerializeField] private Transform target;


    // Start is called before the first frame update
    void Start()
    {
        TrueOffset = customOffset;
        if (maintainCurrentOffset)
        {
            TrueOffset += transform.position - target.position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (followPosition)
        {
            transform.position = target.position + TrueOffset;
        }
    }

    public void ChangeOffset(Vector3 newOffset)
    {
        TrueOffset = newOffset;
    }
}
