using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothing = 5f;

    private GameObject player;
    private Vector3 offset;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        offset = transform.position - player.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = player.transform.position + offset;
        //»ºÂý¸úËæ
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
    }
}