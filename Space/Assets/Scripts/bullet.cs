using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class bullet : NetworkBehaviour{

    public float f_speed = 5.0f;
    Vector3 V_position;
    Vector3 V_rotation;
    public player P_owner;
    public float f_lifeTime = 100;

	
	// Update is called once per frame
	void Update () {
            //moves bullet
            this.transform.position = this.transform.position + ((-this.transform.up * f_speed) * Time.deltaTime);
            if (f_lifeTime < 0)
            {
                //destroys bullet if alive too long
                game.aGO_destroyList.Add(this.gameObject);
            }
            f_lifeTime -= Time.deltaTime;

	}

    /// <summary>
    /// sets owner when spawning
    /// </summary>
    /// <param name="player">owner</param>
    /// <returns>this bullet</returns>
    public GameObject setOwner(player player)
    {
        P_owner = player;
        return this.gameObject;
    }
}
