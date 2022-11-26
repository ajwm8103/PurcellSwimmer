using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidDrag : MonoBehaviour
{
    public float viscosityDrag = 1f;
    private Rigidbody rigidBod;

    // Surface Areas for each pair of faces (neg x will be same as pos x):
    private float sa_x;
    private float sa_y;
    private float sa_z;

    // Use this for initialization
    void Start()
    {
        rigidBod = GetComponent<Rigidbody>();

        // Calculate surface areas for each face:
        sa_x = transform.localScale.y * transform.localScale.z;
        sa_y = transform.localScale.x * transform.localScale.z;
        sa_z = transform.localScale.x * transform.localScale.y;
    }

    void FixedUpdate()
    {
        // Cache positive axis vectors:
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;
        Vector3 right = transform.right;
        // Find centers of each of box's faces
        Vector3 xpos_face_center = (right * transform.localScale.x / 2) + transform.position;
        Vector3 ypos_face_center = (up * transform.localScale.y / 2) + transform.position;
        Vector3 zpos_face_center = (forward * transform.localScale.z / 2) + transform.position;
        Vector3 xneg_face_center = -(right * transform.localScale.x / 2) + transform.position;
        Vector3 yneg_face_center = -(up * transform.localScale.y / 2) + transform.position;
        Vector3 zneg_face_center = -(forward * transform.localScale.z / 2) + transform.position;

        //=== FOR EACH FACE of rigidbody box: ----------------------------------------
        //=== Get Velocity: ---------------------------------------------
        //=== Apply Opposing Force ----------------------------------------

        // FRONT (posZ):
        Vector3 pointVelPosZ = rigidBod.GetPointVelocity(zpos_face_center); // Get velocity of face's center (doesn't catch torque around center of mass)
        Vector3 fluidDragVecPosZ = -forward *     // in the direction opposite the face's normal
                                    Vector3.Dot(forward, pointVelPosZ)   // get the proportion of the velocity vector in the direction of face's normal
                                    * sa_z * viscosityDrag;   // multiplied by face's surface area, and user-defined multiplier
        rigidBod.AddForceAtPosition(fluidDragVecPosZ * 2, zpos_face_center);  // Apply force at face's center, in the direction opposite the face normal
                                                                              // the multiplied by 2 is for the opposite symmetrical face to reduce # of computations

        // TOP (posY):
        Vector3 pointVelPosY = rigidBod.GetPointVelocity(ypos_face_center);
        Vector3 fluidDragVecPosY = -up * Vector3.Dot(up, pointVelPosY) * sa_y * viscosityDrag;
        rigidBod.AddForceAtPosition(fluidDragVecPosY * 2, ypos_face_center);

        // RIGHT (posX):
        Vector3 pointVelPosX = rigidBod.GetPointVelocity(xpos_face_center);
        Vector3 fluidDragVecPosX = -right * Vector3.Dot(right, pointVelPosX) * sa_x * viscosityDrag;
        rigidBod.AddForceAtPosition(fluidDragVecPosX * 2, xpos_face_center);

    }
}