using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallMovement : MonoBehaviour
{
    // we need to define a variable that controls for the speed with which the ball rolls
    public float Speed;

    // FixedUpdate is called several times per frame which will create the illusion of smooth movement
    void FixedUpdate()
    {
        // we need to take the input from the arrow keys pressed by the player. By default in unity, the left and right arrow keys are labeled as "Horizontal" and the up and down arrow keys as "Vertical"
        // we will define two variables where the value of each will be set "several times per frame" if the player presses the HorizontalMovement (i.e. the left or right key) and/or the VerticalMovement (i.e. the up or down key)
        float HorizontalMovement = Input.GetAxis("Horizontal");
        float VerticalMovement = Input.GetAxis("Vertical");

        // since we are dealing with three dimensional space, we will need to define a 3D vector based on which the ball should move. As such, we define a Vector3 called MoveBall that takes three parameters of X,Y,Z
        // movement on the X axis will be our HorizontalMovement (i.e. the left or right key) and on the Z axis the VerticalMovement (i.e. the up or down key). As we do not want to move the ball in the Y axis in the 3D space (i.e. up and down in the 3D space), the Y value will be set to 0
        Vector3 MoveBall = new Vector3(HorizontalMovement,0,VerticalMovement);
        UnityEngine.Debug.Log("x: " + HorizontalMovement + ", z: " + VerticalMovement);
        UnityEngine.Debug.Log("MoveBall: " + MoveBall + ", Speed: " + Speed);

        //lastly, we will need to access the physics component of our ball game object (i.e. the Rigidbody) and add a force to it based on the values of the vector we just defined. We will multiple this force value with our Speed variable to be able to control the Speed of the force as we wish.
        gameObject.transform.GetComponent<Rigidbody>().AddForce(MoveBall * Speed);
    }
}

