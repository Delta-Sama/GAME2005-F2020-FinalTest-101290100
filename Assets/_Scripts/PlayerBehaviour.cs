using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bullet;
    public int fireRate;


    public BulletManager bulletManager;

    [Header("Movement")]
    public float speed;
    public bool isGrounded;


    public RigidBody3D body;
    public CubeBehaviour cube;
    public Camera playerCam;

    void start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _Fire();
        _Move();
    }

    private void _Move()
    {
        Vector3 move = Vector3.zero;
        Vector3 forward = new Vector3(playerCam.transform.forward.x, 0.0f, playerCam.transform.forward.z).normalized;
        Vector3 right = new Vector3(playerCam.transform.right.x, 0.0f, playerCam.transform.right.z).normalized;

        if (Input.GetAxisRaw("Horizontal") > 0.0f)
        {
            // move right
            move += right * speed * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Horizontal") < 0.0f)
        {
            // move left
            move += -right * speed * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Vertical") > 0.0f)
        {
            // move forward
            move += forward * speed * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Vertical") < 0.0f)
        {
            // move Back
            move += -forward * speed * Time.deltaTime;
        }

        if (isGrounded)
        {
            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.9f);

            if (Input.GetAxisRaw("Jump") > 0.0f)
            {
                body.velocity = transform.up * speed * 2.0f * Time.deltaTime;
            }

            transform.position += body.velocity;
        }

        transform.position += move;
    }


    private void _Fire()
    {
        if (Input.GetAxisRaw("Fire1") > 0.0f)
        {
            // delays firing
            if (Time.frameCount % fireRate == 0)
            {

                var tempBullet = bulletManager.GetBullet(bulletSpawn.position, bulletSpawn.forward);
                tempBullet.transform.SetParent(bulletManager.gameObject.transform);
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        isGrounded = cube.isGrounded;
    }

}
