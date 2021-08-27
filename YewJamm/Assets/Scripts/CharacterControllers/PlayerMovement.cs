using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed;
    public bool canMove;
    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this) { Debug.LogWarning("There were multiple players"); Destroy(gameObject); }
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = false;
    }

    //void FixedUpdate()
    //{
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mousePos = new Vector3(Mathf.Clamp(mousePos.x, -4.0f, 4.0f), Mathf.Clamp(mousePos.y, -2.0f, 2.0f), 0);

    //    rb.MovePosition(mousePos);

    //}

    private void Update()
    {

        Vector3 vel = Vector3.zero;
        if (!canMove) { rb.velocity = vel; }

        if (transform.position.y < 4.3f)
        {
            if (Input.GetKey(KeyCode.W))
            {
                vel += transform.up * moveSpeed;
            }
        }
        if (transform.position.y > -4.3f)
        { 
            if (Input.GetKey(KeyCode.S))
            {
                vel -= transform.up * moveSpeed;
            }
        }

        if (transform.position.x > -6.9f)
        {
            if (Input.GetKey(KeyCode.A))
            {
                vel -= transform.right * moveSpeed;

            }
        }
        if (transform.position.x < 6.9f)
        { 
            if (Input.GetKey(KeyCode.D))
            {
                vel += transform.right * moveSpeed;
            }
        }

        if (canMove)
        {
            rb.velocity = new Vector3(vel.x, vel.y, 0);
        }

    }

}
