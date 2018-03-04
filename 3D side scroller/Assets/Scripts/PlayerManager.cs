namespace MultiplayerBasicExample
{
	using System.Collections.Generic;
	using InControl;
	using UnityEngine;


	// This example roughly illustrates the proper way to add multiple players from existing
	// devices. Notice how InputManager.Devices is not used and no index into it is taken.
	// Rather a device references are stored in each player and we use InputManager.OnDeviceDetached
	// to know when one is detached.
	//
	// InputManager.Devices should be considered a pool from which devices may be chosen,
	// not a player list. It could contain non-responsive or unsupported controllers, or there could
	// be more connected controllers than your game supports, so that isn't a good strategy.
	//
	// To detect a joining player, we just check the current active device (which is the last
	// device to provide input) for a relevant button press, check that it isn't already assigned
	// to a player, and then create a new player with it.
	//
	// NOTE: Due to how Unity handles joysticks, disconnecting a single device will currently cause
	// all devices to detach, and the remaining ones to reattach. There is no reliable workaround
	// for this issue. As a result, a disconnecting controller essentially resets this example.
	// In a more real world scenario, we might keep the players around and throw up some UI to let
	// users activate controllers and pick their players again before resuming.
	//
	// This example could easily be extended to use bindings. The process would be very similar,
	// just creating a new instance of your action set subclass per player and assigning the
	// device to its Device property.
	//
	public class PlayerManager : MonoBehaviour
	{
		public GameObject playerPrefab;

        public List<GameObject> spawnPoints = new List<GameObject>();
        public List<Vector3> playerPositions = new List<Vector3>();
        public bool joinRoom;
        private Player ppc;


        const int maxPlayers = 4;


		List<Player> players = new List<Player>( maxPlayers );



		void Start()
		{
            joinRoom = true;

            foreach (GameObject g in spawnPoints)
            {
                playerPositions.Add(g.transform.position);
            }
            InputManager.OnDeviceDetached += OnDeviceDetached;
		}


		void Update()
		{
			var inputDevice = InputManager.ActiveDevice;
            if (joinRoom)
            {
                if (JoinButtonWasPressedOnDevice( inputDevice ))
			    {
				    if (ThereIsNoPlayerUsingDevice( inputDevice ))
				    {
					    CreatePlayer( inputDevice );
				    }
			    }
            }
			
		}


		bool JoinButtonWasPressedOnDevice( InputDevice inputDevice )
		{
			return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
		}

        public void ResetPlayerVariables()
        {
            //Spawn new clone of a Player
            GameObject pp = (GameObject)Instantiate(playerPrefab, transform.position, transform.rotation);
            ppc = pp.GetComponent<Player>();

            //Use clones Variables as Default Variables to reset all players.
            foreach (Player p in players)
            {
                p.ammo = ppc.ammo;
                p.Health = ppc.Health;
                p.Dead = ppc.Dead;
                p.Stunned = ppc.Stunned;
                p.pRigidbody.velocity = new Vector3();
            }
            //delete Clone when done copying
            Destroy(pp);
        }

        public void RespawnPlayers()
        {
            Debug.Log("Respawning Players");
            //Remove Old spawnpoints
            spawnPoints.Clear();

            //find all new spawnpoints
            foreach (GameObject spawnpoint in GameObject.FindGameObjectsWithTag("SpawnPoints"))
            {
                spawnPoints.Add(spawnpoint);
            }
            //add their positions to list
            playerPositions.Clear();
            foreach (GameObject g in spawnPoints)
            {
                playerPositions.Add(g.transform.position);
            }

            var playerCount = players.Count;
            for (var i = 0; i < playerCount; i++)
            {
                var player = players[i];
                player.StartingPosition = playerPositions[i];
                player.transform.position = player.StartingPosition;
                player.Stunned = false;
            }
            
        }

        public Player FindPlayerUsingDevice( InputDevice inputDevice )
		{
			var playerCount = players.Count;
			for (var i = 0; i < playerCount; i++)
			{
				var player = players[i];
				if (player.Device == inputDevice)
				{
					return player;
				}
			}

			return null;
		}

        public float FindWinningPlayer()
        {
            var playerCount = players.Count;
            for (var i = 0; i < playerCount; i++)
            {
                var player = players[i];
                if (player.Dead == false)
                {
                    return i;
                }
            }

            return 0;
        }

        bool ThereIsNoPlayerUsingDevice( InputDevice inputDevice )
		{
			return FindPlayerUsingDevice( inputDevice ) == null;
		}


		void OnDeviceDetached( InputDevice inputDevice )
		{
			var player = FindPlayerUsingDevice( inputDevice );
			if (player != null)
			{
				RemovePlayer( player );
			}
		}


		Player CreatePlayer( InputDevice inputDevice )
		{
			if (players.Count < maxPlayers)
			{
				// Pop a position off the list. We'll add it back if the player is removed.
				var playerPosition = playerPositions[0];
				playerPositions.RemoveAt( 0 );

				var gameObject = (GameObject) Instantiate( playerPrefab, playerPosition, Quaternion.identity );
				var player = gameObject.GetComponent<Player>();
                DontDestroyOnLoad(player);
                player.Device = inputDevice;
				players.Add( player );

				return player;
			}

			return null;
		}


		void RemovePlayer( Player player )
		{
			playerPositions.Insert( 0, player.transform.position );
			players.Remove( player );
			player.Device = null;
			Destroy( player.gameObject );
		}


		void OnGUI()
		{
			const float h = 22.0f;
			var y = 10.0f;

			GUI.Label( new Rect( 10, y, 300, y + h ), "Active players: " + players.Count + "/" + maxPlayers );
			y += h;

		}
	}
}