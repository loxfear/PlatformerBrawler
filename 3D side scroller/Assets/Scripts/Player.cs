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

        //spherecast
        private float height;
        private Vector3 Origin;
        private Vector3 direction;
        private float distanceToObstacle;
        Renderer cachedRenderer;
        


        void Start()
		{
            height = GetComponent<Collider>().bounds.size.y;
            ready = false;
            StartingPosition = transform.position;
            // animator = GetComponent<Animator>();
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
			if (Device == null)
			{
				// If no controller set, just make it translucent white.
				cachedRenderer.material.color = new Color( 1.0f, 1.0f, 1.0f, 0.2f );
			}
			else if (!Stunned)
			{
                if(Dead & Device.Action1.WasPressed)
                {
                    //this.gameObject.SetActive(true);
                    //transform.position = StartingPosition;
                    Dead = false;
                }
				// Set object material color based on which action is pressed.
				cachedRenderer.material.color = GetColorFromInput();


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


                //check if the player is going to hit something
                RaycastHit hit;
                Origin = transform.position;
                direction = transform.forward;
                distanceToObstacle = 10;

               
                if (Physics.SphereCast(Origin, height / 3, direction, out hit, 10))
                {
                    distanceToObstacle = hit.distance;
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

                        //Vector2 movement = new Vector2(move * lerp * speed);

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

                    //Vector2 movement = new Vector2(move * lerp * speed);

                    thrust = -Device.Direction.Left * lerp * runningSpeed;
                }
                else
                {
                    thrust = 0;
                }
                
                
                

                //Move the Player
                if(distanceToObstacle > 0.3)
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Origin + direction * distanceToObstacle, height/3);

        }
    }

}

