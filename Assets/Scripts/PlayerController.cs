using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    //Camera
    public Camera camera;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public float movementSpeed = 8.0f;
    public float turningSpeed = 10.0f;

    public Material greenMaterial;
    public Material redMaterial;

    private GameObject gameManager;
    private GameManager gm;

    private Rigidbody rb;

    private Transform balltrans;

    //Wheels
    public GameObject wheelFL;
    public GameObject wheelFR;
    public GameObject wheelBL;
    public GameObject wheelBR;

    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderBL;
    public WheelCollider wheelColliderBR;

    private int maxTorque = 8000;

    private int playerTeam; //green 1 - red 0
    
    private bool InAir;
    private float rotatespeed = 100f;
    
    private AudioSource carAccSource;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.9f);

        //Stops car from vibrating on movement
        wheelColliderFL.ConfigureVehicleSubsteps(5, 12, 15);
        wheelColliderFR.ConfigureVehicleSubsteps(5, 12, 15);
        wheelColliderBL.ConfigureVehicleSubsteps(5, 12, 15);
        wheelColliderBR.ConfigureVehicleSubsteps(5, 12, 15);

        //Get game manager
        gameManager = GameObject.Find("Game Manager");
        gm = gameManager.GetComponent<GameManager>();

        //Instantiate player UI
        playerUIInstance = Instantiate(playerUIPrefab);
        playerUIInstance.name = playerUIPrefab.name;
    }

    void Awake()
    {
        if (!isLocalPlayer)
        {
            camera.enabled = false;
        }

        carAccSource = GetComponent<AudioSource>();
    }
    
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        updateBallTransform();
        
        InputMovement();
        
        MoveCamera();
    }

    void updateBallTransform()
    {
        balltrans = gm.getBallTrans();
    }

    void InputMovement()
    {
        if (!isLocalPlayer)
            return;

        if (PauseMenu.IsOn)
            return;

        //Movement
        wheelColliderBR.motorTorque = maxTorque * Input.GetAxis("Vertical");
        wheelColliderBL.motorTorque = maxTorque * Input.GetAxis("Vertical");
        wheelColliderFL.steerAngle = 40 * Input.GetAxis("Horizontal");
        wheelColliderFR.steerAngle = 40 * Input.GetAxis("Horizontal");
       
        //Wheelrotation
        wheelFL.transform.localEulerAngles = new Vector3(0, wheelColliderFL.steerAngle - 90, wheelFL.transform.localEulerAngles.z);
        wheelFR.transform.localEulerAngles = new Vector3(0, wheelColliderFR.steerAngle - 90, wheelFR.transform.localEulerAngles.z);

        wheelFL.transform.Rotate(0, 0, -wheelColliderFL.rpm / 60 * 360 * Time.deltaTime);
        wheelFR.transform.Rotate(0, 0, -wheelColliderFR.rpm / 60 * 360 * Time.deltaTime);
        wheelBL.transform.Rotate(0, 0, -wheelColliderBL.rpm / 60 * 360 * Time.deltaTime);
        wheelBR.transform.Rotate(0, 0, -wheelColliderBR.rpm / 60 * 360 * Time.deltaTime);

        //Reset player
        if (Input.GetKeyDown(KeyCode.R))
        {
            //rb.MovePosition(new Vector3(0.0f, 0.5f, 0.0f));
            rb.MovePosition(transform.position + new Vector3(0.0f, 1.0f, 0.0f));
            rb.rotation = Quaternion.identity;
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        //Play sound on accelerate
        if (Input.GetKeyDown(KeyCode.W))
        {
            carAccSource.volume = 0.8f;
            carAccSource.Play();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            carAccSource.volume = 0.2f;
        }

        //Rotate player gameobject if we move while in the air
        if (InAir)
            transform.Rotate(new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal")) * Time.deltaTime * rotatespeed);

        //Reset player
        if (Input.GetKeyDown(KeyCode.Space) && !InAir)
        {
            rb.AddExplosionForce(10, rb.position - new Vector3(0, 0.5f, 0), 30, 1, ForceMode.VelocityChange);
            InAir = true;
        }
    }

    void MoveCamera()
    {
        Vector3 targetPosition = transform.TransformPoint(new Vector3(0, 10, -50));
        camera.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        
        if(Input.GetKey(KeyCode.F))
        { 
            camera.transform.LookAt(balltrans);
        }

        else
        { 
            camera.transform.LookAt(rb.transform);
        }
    }

    public override void OnStartLocalPlayer()
    {        
        camera.enabled = true;
    }
        
    void OnCollisionEnter(Collision col)
    {
        if(isLocalPlayer)
        {
            if (col.gameObject.tag == "Player")
                rb.AddExplosionForce(2, col.rigidbody.position, 30, 0, ForceMode.Impulse);

            if (col.gameObject.tag == "Arena")
                InAir = false;
        }
    }

    //When we are destroyed
    void OnDisable()
    {
        Destroy(playerUIInstance);
    }
}