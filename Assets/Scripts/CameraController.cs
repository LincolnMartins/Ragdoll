using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerTarget;
    private Vector3 mevementDirection;
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position;
    }

    void Update()
    {
        mevementDirection = new Vector3(playerTarget.transform.position.x, offset.y, playerTarget.transform.position.z + offset.z);
    }

    private void FixedUpdate()
    {
        transform.position = mevementDirection;
    }
}
