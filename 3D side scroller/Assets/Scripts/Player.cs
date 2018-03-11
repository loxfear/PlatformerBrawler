namespace MultiplayerBasicExample
{
    using InControl;
    using System.Collections;
    using UnityEngine;


    public class Player : MonoBehaviour
	{
		public InputDevice Device { get; set; }

        [Header("Movement")]
        public float runningSpeed;
        public float maxSpeed = 15f; // units/sec
        public float jumpSpeed;
        public float gravityMultiplier;
        public float Health;
        [Space(10)]

        [Header("Ammo")]
        public float maxAmmo;
        public Rigidbody rocketPrefab;
        public Transform barrelEnd;
        public float hitForce;
        public float shootingForce;
        public float ammo;
        public float Stuntime;
        [Space(10)]

        [Header("Debug")]
        public float deviceDirection;
        public GameManager gameManager;

        private GameObject gameManagerObj;
        public Rigidbody pRigidbody;
        private float speed;
        private float hSpeed;
        private float leftSpeed;
        private float rightSpeed;
        private float thrust;
        private float jumpAmount;
        private float hitDirection;
        private float DeltaHit;
        public bool Stunned;
        public bool Dead;
        private bool notJumped;
        private bool ready;
        public Vector3 StartingPosition;
        public bool grounded;
        public bool obstacle;

        //spherecast
        private CapsuleCollider Capsule;
        private float height;
        private Vector3 Origin;
        private Vector3 direction;
        private float distanceToObstacle;
        private float distanceToGround;
        Renderer cachedRenderer;
        private Animator animator;


        void Start()
		{
            obstacle = false;
            Capsule = GetComponent<CapsuleCollider>();
            height = GetComponent<Collider>().bounds.size.y;
            ready = false;
            StartingPosition = transform.position;
            animator = GetComponent<Animator>();
            notJumped = true;
            pRigidbody = GetComponent<Rigidbody>();
            cachedRenderer = GetComponent<Renderer>();
            gameManagerObj = GameObject.Find("GameManager");
            gameManager = gameManagerObj.GetComponent("GameManager") as GameManager;
        }
        
        //Stunned
        IEnumerator Stun()
        {
            Stunned = true;
            yield return new WaitForSeconds(Stuntime);
            Stunned = false;
            Device.StopVibration();
        }



        void OnCollisionEnter(Collision col)
        {
            //when floor is hit, jump is reset
            if (col.gameObject.tag == "Floor" )
            {
                if(grounded == false)
                {
                    grounded = true;
                    animator.Play("Landing", 0, 0.2f);
                }
                notJumped = true;
                jumpAmount = 3;
               // animator.SetTrigger("Land");
            }

            //if colliding with a DeathPlane, Die
            if (col.gameObject.tag == "DeathPlane")
            {
                Health -= 1;
                //Screenshake Duration Amount
                CameraShake.Shake(0.4f, 0.5f);
                if(Health < 1)
                {
                    Dead = true;
                    gameManager.PlayersAlive -= 1;

                    if (gameManager.PlayersAlive < 3)
                    {
                        gameManager.GameEnd();
                    }
                }
                
                //this.gameObject.SetActive(false);
                transform.position = StartingPosition;
                pRigidbody.velocity = new Vector3(0,0,0);
            }


            // Getting Hit By A Projectile
            if (col.gameObject.tag == "Rocket")
            {
                //Screenshake Duration Amount
                CameraShake.Shake(0.2f, 0.1f);

                StartCoroutine(Stun());

                var rocket = col.gameObject.GetComponent<Rocket>();
                var rocketStart = rocket.startingPos.x;
                var rocketEnd = rocket.endPos.x;
                //destroy the rocket
                Destroy(col.gameObject);
                
                DeltaHit = rocketEnd - rocketStart;
                if (DeltaHit > 0)
                {
                    var direction = new Vector3(1, 0.5f, 0);
                    pRigidbody.AddForce(direction * hitForce);
                }
                else
                {
                    var direction = new Vector3(-1, 0.5f, 0);
                    pRigidbody.AddForce(direction * hitForce);
                }
            }
        }

        
        void OnTriggerEnter(Collider col)
        {
            // Picking up Ammo
            if (col.gameObject.tag == "Ammo" && ammo < maxAmmo)
            {
                ammo += 1;
                Renderer aRender = col.GetComponent<Renderer>();
                aRender.enabled = !aRender.enabled;
                col.enabled = !col.enabled;
            }
        }


        void OnTriggerStay(Collider col)
        {
            //Ready Area - getting ready
            if (col.gameObject.tag == "ReadyArea")
            {
                if (Device.Action2 && !Stunned)
                {
                    Stunned = true;
                    pRigidbody.velocity = new Vector3(0,0,0);
                    ready = true;
                    gameManager.PlayersReady += 1;
                }
                if (Device.Action4 && Stunned)
                {
                    ready = false;
                    Stunned = false;
                    gameManager.PlayersReady -= 1;
                }
                if (Device.Command && ready)
                {
                    gameManager.StartGame();
                }
                
            }
        }

        void FixedUpdate()
        {
            //Get the current speed
            speed = pRigidbody.velocity.magnitude;

            //Gravity Multiplyer
            pRigidbody.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

          

        }

        void Update()
		{

            //+ Capsule.center + Vector3.up * Capsule.height * 0.5F
            RaycastHit hit;
            Vector3 p1 = transform.position + new Vector3(0, Capsule.radius, 0);
            Vector3 p2 = p1 + Vector3.up * (Capsule.height - Capsule.radius);
            Origin = transform.position;
            direction = transform.forward;
            distanceToObstacle = 10;


            //check if something is infront of the player
            if (Physics.CapsuleCast(p1, p2, Capsule.radius - 0.1f, transform.forward, out hit, 10))
            {
                distanceToObstacle = hit.distance;
            }

            //check the distance to the ground under the player
            if (Physics.SphereCast(p1, Capsule.radius / 2, -transform.up, out hit, 10))
            {
                distanceToGround = hit.distance;
            }

            //check if grounded
            if (distanceToGround > 1)
            {
                grounded = false;
            }


            //walking animations

            if(!Stunned && grounded && !obstacle)
            {
                float deviceAmount = Device.Direction.X;
                if (deviceAmount < 0)
                {
                    deviceAmount = -deviceAmount;
                }
                Debug.Log("ready ");
                animator.SetFloat("Forward", deviceAmount);
                if (Device.Direction.X > 0.1)
                {
                    animator.SetBool("running", true);
                }
                else if (Device.Direction.X < -0.1)
                {
                    animator.SetBool("running", true);
                }
                else
                {
                    animator.SetBool("running", false);
                }
            }


			if (!Stunned)
			{
                if(Dead & Device.Action1.WasPressed)
                {
                    //this.gameObject.SetActive(true);
                    //transform.position = StartingPosition;
                    Dead = false;
                }
				// Set object material color based on which action is pressed.
				//cachedRenderer.material.color = GetColorFromInput();


                //--------------------------------------------------------------Movement----------------------------------------------------------------------------------


                //Calculate Speed
                hSpeed = pRigidbody.velocity.x;
                if(hSpeed > 0)
                {
                    //Moving Right
                    rightSpeed = hSpeed;
                    leftSpeed = 0;
                }
                else if (hSpeed < 0)
                {
                    leftSpeed = -hSpeed;
                    rightSpeed = 0;
                }


               

                //move right
                if (Device.Direction.Right > 0)
                {
                    float t = rightSpeed / maxSpeed;

                    float lerp = 0f;

                    if (t >= 0f)
                        lerp = Mathf.Lerp(maxSpeed, 0f, t);
                    else if (t < 0f)
                        lerp = Mathf.Lerp(maxSpeed, 0f, Mathf.Abs(t));

                    thrust = Device.Direction.Right * lerp * runningSpeed;
                }
                //move Left
                else if(Device.Direction.Left > 0)
                {
                    float t = leftSpeed / maxSpeed;

                    float lerp = 0f;

                    if (t >= 0f)
                        lerp = Mathf.Lerp(maxSpeed, 0f, t);
                    else if (t < 0f)
                        lerp = Mathf.Lerp(maxSpeed, 0f, Mathf.Abs(t));

                    thrust = -Device.Direction.Left * lerp * runningSpeed;
                }
                else
                {
                    thrust = 0;
                }
                
                
                if (distanceToObstacle > 0.3)
                {
                    obstacle = false;
                }
                else
                {
                    obstacle = true;
                }

                //Move the Player
                if(!obstacle)
                {
                    pRigidbody.AddForce(new Vector3(1, 0, 0) * thrust);
                }


                //--------------------------------------------------------------Movement----------------------------------------------------------------------------------

                // Shooting
                if (Device.RightBumper.WasPressed && ammo > 0)
                {
                    Rigidbody rocketInstance;
                    rocketInstance = Instantiate(rocketPrefab, barrelEnd.position, barrelEnd.rotation) as Rigidbody;
                    rocketInstance.AddForce(barrelEnd.forward * shootingForce);
                    ammo -= 1;
                }

                //Jump
                if (Device.Action1.WasPressed & jumpAmount > 0)
                {
                    animator.Play("Jumping", 0, 0.5f);
                    pRigidbody.AddForce(Vector3.up * jumpSpeed);
                    jumpAmount -= 1;
                    notJumped = false;
                }


                if (Device.Direction.X > 0)
                {
                    transform.eulerAngles = new Vector3(0, 90, 0);
                }
                else if (Device.Direction.X < 0)
                {
                    transform.eulerAngles = new Vector3(0, -90, 0);
                }
            }
		}


		Color GetColorFromInput()
		{
			if (Device.Action1)
			{
				return Color.green;
			}

			if (Device.Action2)
			{
				return Color.red;
			}

			if (Device.Action3)
			{
				return Color.blue;
			}

			if (Device.Action4)
			{
				return Color.yellow;
			}

			return Color.white;
		}

        void OnDrawGizmosSelected()
        {
            Vector3 p1 = transform.position + Capsule.center + Vector3.up * Capsule.height * 0.5F;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(p1 - transform.up * distanceToGround, Capsule.radius/2);
            Gizmos.DrawWireSphere(p1 + transform.forward * distanceToObstacle, Capsule.radius);
                //if (Physics.CapsuleCast(p1, p2, Capsule.radius, transform.forward, out hit, 10))

        }
    }

}

