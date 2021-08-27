using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor { Red, Blue, Green, Yellow, Orange, Purple, Pink, White}
public class Block : MonoBehaviour
{
    Rigidbody rb;
    public float maxSpeed = 5.0f, minSpeed = 0.5f, maxRotSpeed = 5.0f;
    public bool dominant;
    public BlockColor myColor;
    public GameObject colorBlock;
    public bool tutorialBlock = false;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
        moving = !tutorialBlock;
        rb = GetComponent<Rigidbody>();
        colorBlock.GetComponent<Renderer>().material = GameManager.Instance.BlockColorToMat(myColor);
    }

    public void Init()
    {
        rb = GetComponent<Rigidbody>();
        colorBlock.GetComponent<Renderer>().material = GameManager.Instance.BlockColorToMat(myColor);
        rb.velocity = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0);
        rb.angularVelocity = new Vector3(0, 0, Random.Range(-maxRotSpeed, maxRotSpeed));
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            if (rb.velocity.magnitude > maxSpeed) { rb.velocity *= maxSpeed / rb.velocity.magnitude; }
            if (rb.angularVelocity.magnitude > maxRotSpeed) { rb.angularVelocity *= maxRotSpeed / rb.angularVelocity.magnitude; }

            if (rb.velocity.magnitude < minSpeed && rb.velocity.magnitude != 0.0f) { rb.velocity *= minSpeed / rb.velocity.magnitude; }
            if (rb.velocity.magnitude == 0.0f) { rb.velocity = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!dominant || !other.CompareTag("BlockTrigger")) { return; }

        Block otherBlock = other.GetComponentInParent<Block>();
        if(otherBlock.myColor != myColor) { return; }

        StartCoroutine(Connect(otherBlock));
    }

    IEnumerator Connect(Block otherBlock)
    {
        moving = false;
        otherBlock.moving = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        otherBlock.rb.velocity = Vector3.zero;
        otherBlock.rb.angularVelocity = Vector3.zero;

        foreach(Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
        
        foreach(Collider c in otherBlock.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        Renderer[] myRends, otherRends;
        myRends = GetComponentsInChildren<Renderer>();
        otherRends = otherBlock.GetComponentsInChildren<Renderer>();

        float t = 0.0f;
        while(t < 1.0f)
        {
            foreach (Renderer r in myRends)
            {
                r.material.color = Color.Lerp(
                    new Color(r.material.color.r, r.material.color.g, r.material.color.b, 1.0f),
                    new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.0f),
                    t);
            }
            
            foreach (Renderer r in otherRends)
            {
                r.material.color = Color.Lerp(
                    new Color(r.material.color.r, r.material.color.g, r.material.color.b, 1.0f),
                    new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.0f),
                    t);
            }

            t += Time.deltaTime * 2;
            yield return null;
        }

        GameManager.Instance.BlockDestroyed(myColor);

        Destroy(otherBlock.gameObject);
        Destroy(gameObject);
    }
}
