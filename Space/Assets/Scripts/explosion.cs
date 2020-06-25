using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class explosion : NetworkBehaviour
{
    public Sprite[] aS_explosion;
    SpriteRenderer SR_renderer;
    float f_scale;
    float f_timer;
    // Start is called before the first frame update
    void Start()
    {
        SR_renderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// sets size of explosion
    /// </summary>
    /// <param name="i">1 for small, 2 for big</param>
    /// <returns>this explosion</returns>
    public GameObject setType(int i)
    {
        if (i == 1)
        {
            this.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        else if (i == 2)
        {
            this.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        }
        return this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        f_timer += Time.deltaTime;
        if (f_timer > 1)
        {
            //destroys this after 1 second
            game.aGO_destroyList.Add(this.gameObject);
        }
        else
        {
            //changes sprite
            SR_renderer.sprite = aS_explosion[(int)f_timer];
        }
    }
}
