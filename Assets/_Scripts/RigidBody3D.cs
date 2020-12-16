using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyType
{
    STATIC,
    DYNAMIC
}


[System.Serializable]
public class RigidBody3D : MonoBehaviour
{
    [Header("Gravity Simulation")]
    public float gravityScale;
    public float mass;
    public BodyType bodyType;
    public float timer;
    public int isFalling;

    [Header("Attributes")]
    public Vector3 velocity;
    public Vector3 acceleration;
    private float gravity;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        gravity = -0.001f;
        velocity = Vector3.zero;
        acceleration = new Vector3(0.0f, gravity * gravityScale, 0.0f);
        if (bodyType == BodyType.DYNAMIC)
        {
            isFalling = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bodyType == BodyType.DYNAMIC)
        {
            if (isFalling == 1)
            {
                timer += Time.deltaTime;
                
                if (gravityScale < 0)
                {
                    gravityScale = 0;
                }

                if (gravityScale > 0)
                {
                    velocity += acceleration * 0.5f * timer * timer;
                    transform.position += velocity;
                }
            }
            else if (isFalling > 1)
            {
                isFalling -= 1;
            }
        }
    }

    public void Stop()
    {
        timer = 0;
        isFalling = 0;
    }
}
