using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
 
    // this is the default unity function that will be called whenever two objects collide and one has the Trigger parameter enabled
    // the argument "other" refers to the object that hits the game object this script is attached to. 
    private void OnTriggerEnter(Collider other)
    {
        // In this case the we want to check if the "other" is the "Ball"
       // Make sure your ball gameobject is named “Ball”, otherwise you will receive an error!
        if (other.name == "Ball")
        {
            // if this condition is correct and indeed the Ball game object has hit this game object where the script is attached (i.e. the coin), we will remove this game object
            Destroy(this.gameObject);
        }
    }
}