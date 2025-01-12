using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        //rotation = player.transform.rotation;
    }

    // LateUpdate is called once per frame after all Update functions habe been called
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
        Vector3 direction = player.GetComponent<Rigidbody>().velocity;

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
