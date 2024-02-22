using UnityEngine;
public class PlatformMovement : MonoBehaviour
{

    private void FixedUpdate()
    {
        Move();
    }
    protected void Move()
    {
        transform.position += -Vector3.forward * Time.deltaTime * PlayerMovement.currentSpeed;
    }
}
