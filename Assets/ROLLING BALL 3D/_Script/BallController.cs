using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour, IListener
{
    public float speedMobile = 0.145f;
    public float speedPC = 0.06125f;
    public float uniAccBall;
    public GameObject destroyFX;
   
    [HideInInspector]
    public float speed;
    public float boostForce = 100;
    public Vector3 boostForceII = new Vector3(0, -6.1f, 12.2f);
    Rigidbody rig;

    [HideInInspector]
    public GameObject blockPath;

    bool isFalling = false;

    [Header("Skin")]
    public Material[] ballSkinMaterials;

    
    [Header("Magnet")]
    public GameObject magnetObj;
    public float magnetTime = 15;
    [Header("X2")]
    public GameObject x2Obj;
    public float x2Time = 15;
    [Header("Shield")]
    public GameObject shieldObj;
    public float shieldTime = 15;
    [Header("Accelerate")]
    public GameObject accObj;
    public float accTime = 10;

    [HideInInspector] public bool isUsingMagnet = false;
    [HideInInspector] public bool isUsingX2Item = false;
    [HideInInspector] public bool isUsingShield = false;
    [HideInInspector] public bool isAccel = false;

    Vector3 originalPos;

    // Use this for initialization
    void Start()
    {
        originalPos = transform.position;

        rig = GetComponent<Rigidbody>();
        if (!GlobalValue.restartAndStartGameImmediately)
            rig.isKinematic = true;

        GameManager.Instance.ballObj = gameObject;

        //random rotation
        Vector3 rotation = transform.localEulerAngles;

        float xAngle = rotation.x;
        float yAngle = rotation.y;
        float zAngle = rotation.z;

        xAngle = Random.Range(0, 360);
        yAngle = Random.Range(0, 360);
        zAngle = Random.Range(0, 360);

        transform.localEulerAngles = new Vector3(xAngle, yAngle, zAngle);

        SetBallSkin();

        //
    }

    public void SetBallSkin()
    {
        GetComponent<MeshRenderer>().material = ballSkinMaterials[GlobalValue.ballPickedID];
    }

    public void ActiveShield()
    {
        if (!isUsingShield)
        {
            StartCoroutine(ActiveShieldCo());
        }
    }

    IEnumerator ActiveShieldCo()
    {
        isUsingShield = true;
        yield return new WaitForSeconds(shieldTime);
        isUsingShield = false;
    }

    public void ActiveMagnet()
    {
        if (!isUsingMagnet)
        {
            StartCoroutine(ActiveMagnetCo());
        }
    }

    IEnumerator ActiveMagnetCo()
    {
        isUsingMagnet = true;
        yield return new WaitForSeconds(magnetTime);
        isUsingMagnet = false;
    }

    public void ActiveX2()
    {
        if (!isUsingX2Item)
        {
            StartCoroutine(ActiveX2Co());
        }
    }

    IEnumerator ActiveX2Co()
    {
        isUsingX2Item = true;
        yield return new WaitForSeconds(x2Time);
        isUsingX2Item = false;
    }

    public void ActiveAccEffect()
    {
        if (!isAccel)
        {
            StartCoroutine(ActiveAccEffectCo());
        }
    }

    IEnumerator ActiveAccEffectCo()
    {
        isAccel = true;
        yield return new WaitForSeconds(accTime);
        isAccel = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
        }
        
        speed = rig.velocity.magnitude;

        //--------------------------------Update-----------------------------------
        if (GameManager.Instance.state == GameManager.State.Playing)
            GameManager.Instance.ballDistance = (int)Mathf.Clamp(transform.position.z, 0f, float.MaxValue);

        magnetObj.SetActive(isUsingMagnet);
        shieldObj.SetActive(isUsingShield);
        x2Obj.SetActive(isUsingX2Item);
        accObj.SetActive(isAccel);

        //check if ball falling and no contact "Falling" Trigger
        if (GameManager.Instance.state == GameManager.State.Playing && !isFalling)
            CheckPlayerFallTooLong();

    }

    void FixedUpdate() 
    {
        if (GameManager.Instance.state != GameManager.State.Playing)
            return;

#if UNITY_EDITOR || UNITY_WEBGL
        float _speedX = Input.GetAxis("Horizontal") * speedPC;
        float accBall = Input.GetAxis("Vertical") * uniAccBall;
#else
        float _speedX = Input.acceleration.x * speedMobile;
#endif
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb.velocity.z <= 0 && accBall < 0) 
            accBall = 0;
        
        if (isPlaying)
        {
            GetComponent<Rigidbody>().velocity += accBall * Vector3.forward + _speedX * Vector3.right;
        }   
        GameManager.Instance.cameraFollow.FollowBall();
    }

    //Just in case the ball can't catch the Fall trigger
    float noTouchPathTime = 0;
    float noTouchPathLimitToFall = 4;
    void CheckPlayerFallTooLong()
    {
        noTouchPathTime += Time.deltaTime;
        if (Physics.Raycast(transform.position, Vector3.down, 2))
        {
            noTouchPathTime = 0;
        }

        if(noTouchPathTime > noTouchPathLimitToFall)
            StartCoroutine(FallingCo());
    }

    void SetVelocity(Vector3 _vec)
    {
        rig.velocity = _vec;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (isFalling)
            return;

        if (other.gameObject.tag == "Boost")
        {
            //Debug.LogError(other.gameObject.name);
            ActiveAccEffect();
            rig.AddForce(new Vector3(0, 0, boostForce));
        }
        else if (other.gameObject.tag == "Boost II")
        {
            ActiveAccEffect();
            rig.velocity = boostForceII;
        }
        else if (other.gameObject.tag == "Dinamond")
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundCollectGem, 0.35f);
            GlobalValue.StoreDiamond += isUsingX2Item ? 2 : 1;
            //if (diamondFX)
            //{
            HandlePollObject.Instance.GetAvailableObj(true).transform.position = other.gameObject.transform.position;
            //}
            //Instantiate(diamondFX, other.gameObject.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Falling")
        {
            StartCoroutine(FallingCo());
        }
        else if (other.gameObject.tag == "Finish")
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
            Instantiate(destroyFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            //GetComponent<csShowAllEffect>().Init();

            if (!isUsingShield)
            {
                GameManager.Instance.GameOverDelay(1);
                SoundManager.PlaySfx(SoundManager.Instance.soundBallExplosion);
                gameObject.SetActive(false);
            }
            else
                other.gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "NextLevel")
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundNextLevel);
            GameManager.Instance.currentLevel++;
        }
    }

    IEnumerator FallingCo()
    {
        isFalling = true;
        GameManager.Instance.BallFalling();
        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        if (collision.collider.gameObject.tag == "Path")
        {
            blockPath = collision.collider.gameObject;
            SetVelocity(new Vector3(0, rig.velocity.y, rig.velocity.z));
            if (speed > GameManager.Instance.speedBallAllow)
            {
                rig.velocity = GameManager.Instance.vecBallHitPath;
            }
        }
    }

    bool isPlaying = false;
    public void IPlay()
    {
        isFalling = false;
        isPlaying = true;
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        rig.isKinematic = false;
        rig.velocity = Vector3.zero;
    }

    public void ISuccess()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IGameOver()
    {
        isUsingX2Item = false;
        isUsingShield = false;
        isUsingMagnet = false;
        isAccel = false;
        isPlaying = false;

        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }
    
    public void IOnRespawn()
    {
        if (blockPath != null)
            transform.position = blockPath.transform.position + new Vector3(0, 1, 1);
        else
            transform.position = originalPos;
        gameObject.SetActive(true);
        rig.isKinematic = true;
    }

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }
}
