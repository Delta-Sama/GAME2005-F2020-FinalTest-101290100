﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] cubes;
    public BulletBehaviour[] bullets;

    private static Vector3[] faces;

    // Start is called before the first frame update
    void Start()
    {
        cubes = FindObjectsOfType<CubeBehaviour>();

        faces = new Vector3[]
        {
            Vector3.left, Vector3.right,
            Vector3.down, Vector3.up,
            Vector3.back , Vector3.forward
        };
    }

    // Update is called once per frame
    void Update()
    {
        bullets = FindObjectsOfType<BulletBehaviour>();

        // check each AABB with every other AABB in the scene
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(cubes[i], cubes[j]);
                }
            }
        }

        // Check each sphere against each AABB in the scene
        foreach (var bullet in bullets)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    CheckBulletAABB(bullet, cube);
                }
                
            }
        }


    }

    public static void CheckBulletAABB(BulletBehaviour a, CubeBehaviour b)
    {
        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z) && (!a.isColliding))
        {
            float depth = 0.0f;
            Vector3 normal = Vector3.zero;

            Vector3[] faces = new Vector3[6];
            faces[0] = new Vector3(-1, 0, 0);
            faces[1] = new Vector3(1, 0, 0);
            faces[2] = new Vector3(0, -1, 0);
            faces[3] = new Vector3(0, 1, 0);
            faces[4] = new Vector3(0, 0, -1);
            faces[5] = new Vector3(0, 0, 1);

            float[] dists = new float[6];
            dists[0] = a.max.x - b.min.x;
            dists[1] = b.max.x - a.min.x;
            dists[2] = a.max.y - b.min.y;
            dists[3] = b.max.y - a.min.y;
            dists[4] = a.max.z - b.min.z;
            dists[5] = b.max.z - a.min.z;

            float min = float.MaxValue;
            for (int i = 0; i < 6; i++)
            {
                if (dists[i] < min || i == 0)
                {
                    normal = faces[i];
                    depth = dists[i];
                    min = dists[i];
                }
            }

            a.penetration = depth;
            a.collisionNormal = normal;

            a.transform.position = a.transform.position + normal * depth;
            
            Reflect(a);

            a._Move();
        }
    }

    public static void CheckSphereAABB(BulletBehaviour s, CubeBehaviour b)
    {
        // get box closest point to sphere center by clamping
        var x = Mathf.Max(b.min.x, Mathf.Min(s.transform.position.x, b.max.x));
        var y = Mathf.Max(b.min.y, Mathf.Min(s.transform.position.y, b.max.y));
        var z = Mathf.Max(b.min.z, Mathf.Min(s.transform.position.z, b.max.z));

        var distance = Math.Sqrt((x - s.transform.position.x) * (x - s.transform.position.x) +
                                 (y - s.transform.position.y) * (y - s.transform.position.y) +
                                 (z - s.transform.position.z) * (z - s.transform.position.z));

        if ((distance < s.radius) && (!s.isColliding))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - s.transform.position.x),
                (s.transform.position.x - b.min.x),
                (b.max.y - s.transform.position.y),
                (s.transform.position.y - b.min.y),
                (b.max.z - s.transform.position.z),
                (s.transform.position.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }

            s.penetration = penetration;
            s.collisionNormal = face;
            //s.isColliding = true;

            Reflect(s);
        }

    }
    
    // This helper function reflects the bullet when it hits an AABB face
    private static void Reflect(BulletBehaviour s)
    {
        if ((s.collisionNormal == Vector3.forward) || (s.collisionNormal == Vector3.back))
        {
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.right) || (s.collisionNormal == Vector3.left))
        {
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
        }
        else if ((s.collisionNormal == Vector3.up) || (s.collisionNormal == Vector3.down))
        {
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
        }
    }


    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        Contact contactB = new Contact(b);

        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - a.min.x),
                (a.max.x - b.min.x),
                (b.max.y - a.min.y),
                (a.max.y - b.min.y),
                (b.max.z - a.min.z),
                (a.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }
            
            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;

            // check if contact does not exist
            if (!a.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = a.contacts.Count - 1; i > -1; i--)
                {
                    if (a.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        a.contacts.RemoveAt(i);
                    }
                }

                if (contactB.face == Vector3.down)
                {
                    a.gameObject.GetComponent<RigidBody3D>().Stop();
                    a.gameObject.GetComponent<RigidBody3D>().velocity.y = 0.0f;
                    a.isGrounded = true;
                }
                

                // add the new contact
                a.contacts.Add(contactB);
                a.isColliding = true;
            }

            // Resolving even if they had a collision before
            RigidBody3D a_rid = a.GetComponentInParent<RigidBody3D>();
            RigidBody3D b_rid = b.GetComponentInParent<RigidBody3D>();

            if (a_rid.bodyType == BodyType.DYNAMIC && b_rid.bodyType == BodyType.STATIC)
            {
                a.transform.position = a.transform.position - contactB.face * contactB.penetration * 1.0f;
            }
            else if (a_rid.bodyType == BodyType.DYNAMIC && b_rid.bodyType == BodyType.DYNAMIC)
            {
                float prop1 = 0.5f;
                float prop2 = 0.5f;

                if (contactB.face == Vector3.up)
                {
                    prop1 = 0.0f;
                    prop2 = 0.5f;
                }
                else if (contactB.face == Vector3.down)
                {
                    prop1 = 0.5f;
                    prop2 = 0.0f;
                }

                a.transform.position = a.transform.position - contactB.face * contactB.penetration * prop1;
                b.transform.position = b.transform.position + contactB.face * contactB.penetration * prop2;
            }
        }
        else
        if (a.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
        {
            {
                a.contacts.Remove(a.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                a.isColliding = false;

                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                    a.gameObject.GetComponent<RigidBody3D>().isFalling = 1;
                    a.isGrounded = false;
                }
            }
        }
    }

}
