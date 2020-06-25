using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : NetworkBehaviour {

    public Sprite S_spriteCyan;
    public Sprite S_spriteGreen;
    public Sprite S_spriteBlue;
    public Sprite S_spriteOrange;
    public Sprite S_spritePurple;
    public Sprite S_spriteRed;
    public Sprite S_spriteYellow;
    int i_sprite;
    SpriteRenderer SR_renderer;

    public GameObject bullet;
    public GameObject explosion;

    [SyncVar]
    public int i_kills =0;
    int i_screensize = 100;

    [SyncVar]
    public string s_username;

    Vector3 V_cameraPos;
    [SyncVar]
	Vector3 V_position;
    [SyncVar]
	Vector3 V_rotation;
    public float f_speed = 2.5f;

    [SyncVar]
    public int i_health = 100;
    [SyncVar]
    public int i_shield = 100;
    [SyncVar]
    float f_deathTimer = 0;
    [SyncVar]
    bool b_dead = false;
    float f_shootTimer = 0;


    [SyncVar]
    public int i_pingToServer = 0;
    Ping P_pinger;
    float f_pingTimer = 0;

    bool b_usernameSent = false;
    float f_usernameTimer = 5;

    [SyncVar]
    public bool b_winning = false;
    // Use this for initialization
    void Start () { 
        if (isLocalPlayer)
        {
            s_username = GameSpark.getUsername();
            game.localPlayer = this.gameObject;
            if (isClientOnly)
            {
                CmdAddPlayer();
                P_pinger = new Ping(NetworkManager.singleton.networkAddress);
            }
            else
            {
                game.aP_players.Add(this);
            }
        }
        
        SR_renderer = GetComponent<SpriteRenderer>();
        changeSprite();
        
        //Random spawn position
        V_position = new Vector3(Random.Range(-(i_screensize * 0.8f), (i_screensize * 0.8f)), Random.Range(-(i_screensize * 0.8f), (i_screensize * 0.8f)), 0);

        V_cameraPos = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (isClientOnly)
        {
            //Send Username
            if (!b_usernameSent)
            {
                if (f_usernameTimer > 0)
                {
                    f_usernameTimer -= Time.deltaTime;
                }
                else
                {
                    CmdUsername(s_username);
                    b_usernameSent = true;
                }
            }
            //Send Ping
            if (base.hasAuthority)
            {
                if (f_pingTimer > 0.5f)
                {
                    f_pingTimer = 0;
                    if (P_pinger.isDone)
                    {
                        i_pingToServer = P_pinger.time;
                        CmdSendPing(i_pingToServer);
                        Debug.Log(P_pinger.time);
                    }
                }
                else
                {
                    f_pingTimer += Time.deltaTime;
                }
            }
        }
        //if alive else not
        if (i_health > 0)
        {
            changeSprite();
            if (base.hasAuthority)
            {
                move();
                rotate();
                
                if (f_shootTimer > 5)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        shoot();
                        f_shootTimer = 0;
                    }
                }
                f_shootTimer += 10 * Time.deltaTime;
            }
        }
        else
        {
            //turn of sprite
            SR_renderer.enabled = false;
            //wait to "respawn"
            f_deathTimer += Time.deltaTime;
            if(f_deathTimer > 5)
            {
                V_position = new Vector3(Random.Range(-(i_screensize * 0.8f), (i_screensize * 0.8f)), Random.Range(-(i_screensize * 0.8f), (i_screensize * 0.8f)), 0);
                i_shield = 100;
                i_health = 100;
                f_deathTimer = 0;
                SR_renderer.enabled = true;
                b_dead = false;
            }
        }
	}

    /// <summary>
    /// Sends ping to server
    /// </summary>
    /// <param name="ping"></param>
    [Command]
    void CmdSendPing(int ping)
    {
        i_pingToServer = ping;
    }

    /// <summary>
    /// Sends username to server
    /// </summary>
    /// <param name="username"></param>
    [Command]
    void CmdUsername(string username)
    {
        s_username = username;
    }

    /// <summary>
    /// adds player to lest on server
    /// </summary>
    [Command]
    void CmdAddPlayer()
    {
        game.aP_players.Add(this);
    }

    /// <summary>
    /// controls movement
    /// </summary>
    void move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            V_position.x -= (f_speed * Time.deltaTime);
            if(V_position.x < -i_screensize)
            {
                V_position.x = -i_screensize;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            V_position.y -= (f_speed * Time.deltaTime);
            if (V_position.y < -i_screensize)
            {
                V_position.y = -i_screensize;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            V_position.x += (f_speed * Time.deltaTime);
            if (V_position.x > i_screensize)
            {
                V_position.x = i_screensize;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            V_position.y += (f_speed * Time.deltaTime);
            if (V_position.y >i_screensize)
            {
                V_position.y = i_screensize;
            }
        }

        V_cameraPos.x = V_position.x;
        V_cameraPos.y = V_position.y;

        Camera.main.transform.position = V_cameraPos;
        this.transform.position = V_position;
    }


    /// <summary>
    /// Spawns bullets on server
    /// </summary>
    [Command]
    void CmdShoot()
    {
        NetworkServer.Spawn(Instantiate(bullet, this.transform.position - transform.up * 2, this.transform.rotation).GetComponent<bullet>().setOwner(this), connectionToClient);
    }

    /// <summary>
    /// spawns bullet if running on server
    /// send cmd to server if client only
    /// </summary>
    void shoot()
    {
        if (isClientOnly)
        {
            CmdShoot();
           }
        else
        {
            NetworkServer.Spawn(Instantiate(bullet, V_position - transform.up * 2, this.transform.rotation).GetComponent<bullet>().setOwner(this), this.gameObject);
        }
        //Instantiate(bullet, V_position - transform.up*2, this.transform.rotation).GetComponent<bullet>().setOwner(this);
    }

    /// <summary>
    /// controls rotation
    /// </summary>
    void rotate()
    {
        V_rotation.z = getAngle();
        this.transform.rotation = Quaternion.Euler(V_rotation);
    }

    /// <summary>
    /// gets angle from mouse to player
    /// </summary>
    /// <returns>direction of ship</returns>
    float getAngle()
    {
        float angle = 0;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = mousePos.x - this.transform.position.x;
        float y = mousePos.y - this.transform.position.y;

        if(x < 0)
        {
            angle = 270 - (Mathf.Atan(y / -x) * 180 / Mathf.PI);
        }
        else
        {
            angle = 90 + (Mathf.Atan(y / x) * 180 / Mathf.PI);
        }
        

        return angle;
    }

    /// <summary>
    /// Handles collision on server
    /// spawns explosions and handles kills
    /// </summary>
    /// <param name="other">bullet object</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isClientOnly)
        {
            Debug.Log("collision with " + other.ToString());
            if (other.collider.gameObject.tag == "Bullet")
            {
                //Spawns explosion
                NetworkServer.Spawn(Instantiate(explosion, other.transform.position, this.transform.rotation).GetComponent<explosion>().setType(1), this.gameObject);

                if (i_shield > 0)
                {
                    i_shield -= 10;
                }
                else
                {
                    i_health -= 10;
                }

                if (i_health <= 0)
                {
                    if (!b_dead) {
                        //spawn big explosion
                        NetworkServer.Spawn(Instantiate(explosion, V_position, this.transform.rotation).GetComponent<explosion>().setType(2), this.gameObject);
                        //increases kills on bullet owner
                        player p = other.gameObject.GetComponent<bullet>().P_owner;
                        b_dead = true;
                        p.i_kills++;
                        if (!game.aP_playerKills.Contains(p))
                        {
                            Debug.Log(p.s_username);
                            game.aP_playerKills.Add(p);
                        }
                      }
                }
                //removes bullet
                Destroy(other.gameObject);
                Debug.Log("boom");
            }
        }
    }

    /// <summary>
    /// changes sprite based on shield and health
    /// </summary>
    void changeSprite()
    {
        if (i_shield > 0)
        {
            if (i_shield < 30)
            {
                if (i_sprite != 4)
                {
                    SR_renderer.sprite = S_spriteBlue;
                    i_sprite = 4;
                }
            }
            else if (i_shield < 60)
            {
                if (i_sprite != 5)
                {
                    SR_renderer.sprite = S_spriteCyan;
                    i_sprite = 5;
                }
            }
            else
            {
                if (i_sprite != 6)
                {
                    SR_renderer.sprite = S_spritePurple;
                    i_sprite = 6;
                }
            }
        }
        else
        {
            if (i_health > 65)
            {
                if (i_sprite != 3)
                {
                    SR_renderer.sprite = S_spriteGreen;
                    i_sprite = 3;
                }
            }
            else if (i_health > 40)
            {
                if (i_sprite != 2)
                {
                    SR_renderer.sprite = S_spriteYellow;
                    i_sprite = 2;
                }
            }
            else if (i_health > 20)
            {
                if (i_sprite != 1)
                {
                    SR_renderer.sprite = S_spriteOrange;
                    i_sprite = 1;
                }
            }
            else
            {
                if (i_sprite != 0)
                {
                    SR_renderer.sprite = S_spriteRed;
                    i_sprite = 0;
                }
            }

        }
    }
}
