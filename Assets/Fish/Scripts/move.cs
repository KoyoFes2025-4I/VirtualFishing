using UnityEngine;

[RequireComponent(typeof(Rigidbody))]�@// Rigidbody�̎����A�^�b�`
public class WASDMovementWithRigidbody : MonoBehaviour
{

    // WSAD�L�[�ŃI�u�W�F�N�g���ړ�������

    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed;
        Vector3 newPosition = rb.position + move * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
}