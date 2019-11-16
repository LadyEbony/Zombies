﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class NetworkManager : MonoBehaviour {

    #region Inspector

    public string serverAddress, appID, gameVersion;

    [Header("Debug")]
    public ParticleSystem NetworkDebugParticles;

    public bool expectedOnline = false;
    public byte expectedMaxPlayers = 2;

    #endregion

	 #region Particles
	public static bool visParticles = true;
	// Spawn a particle on an entity, used for visualizing updates
	public static void netParticle(EntityBase eb, Color pColor) {
		if (!visParticles) return;

		if (instance.NetworkDebugParticles) {
			var pparams = new ParticleSystem.EmitParams();
			pparams.position = eb.transform.position;
			pparams.startColor = pColor;
			instance.NetworkDebugParticles.Emit(pparams, 1);
		}
	}

  #endregion

  #region Network Helpers


  public static Player[] getSortedPlayers {
    get {
      var players = NetworkManager.net.CurrentRoom.Players.Values;
      var playersSorted = players.OrderBy(p => p.ID);
      return playersSorted.ToArray();
    }
  }

  public static NetLogic net { get; private set; }

    // A time value that is 'approximately' (+/- 10ms most of the time) synced across clients
    public static double ServerTime {
      get {
		    if (net == null || net.loadBalancingPeer == null)
		      return Time.realtimeSinceStartup;

		    return net.loadBalancingPeer.ServerTimeInMilliSeconds * (1d/1000d);
      }
    }

	/// <summary>
	/// On non WebSocketSecure platforms, encryption handshake must occur before opCustom can be sent.
	/// This is important in cases such as getting the room or region list.
	/// </summary>
	public static bool delayForEncrypt {
		get {
			// We use secure websockets in UnityGL, so no encrypt handshake needs to occur
			#if UNITY_WEBGL
			return true;
			#else
			return !net.loadBalancingPeer.IsEncryptionAvailable;
			#endif
		}
	}

	/// <summary>
	/// Returns true if able to send network connections.
	/// </summary>
	public static bool isReady {
		get {
			if (net == null) return false;

			return net.IsConnectedAndReady;
		}
	}

	/// <summary>
	/// If this client is considered owner of the room. 
	/// </summary>
	public static bool isMaster {
		get {
			// If have no networking, we're the owner.
			if (net == null) return true;
			if (!inRoom) return true;

			return net.LocalPlayer.IsMasterClient;
		}
	}

	/// <summary>
	/// Boolean for if we are on the name server. Used for awaiting the name server connection.
	/// Can be set to true to connect to name server.
	/// </summary>
	/// <value><c>true</c> if on name server; otherwise, <c>false</c>.</value>//
	public static bool onNameServer {
		get {
			if (net == null) return false;

			return net.State.Equals(ClientState.ConnectedToNameServer);
		} set {
			if (value) net.ConnectToNameServer();
		}
	}

	/// <summary>
	/// Boolean to check if the network is in the master lobby, will be true after we've found a region.
	/// </summary>
	/// <value><c>true</c> if on master lobby; otherwise, <c>false</c>.</value>
	public static bool onMasterLobby {
		get {
			if (net == null) return false;

			return net.State.Equals(ClientState.JoinedLobby);
		}
	}

   /// <summary>
   /// Boolean to check if we're in a room or not.
   /// </summary>
   /// <value><c>true</c> if in room; otherwise, <c>false</c>.</value>
   public static bool inRoom{
      get {
          if (net == null) return false;

          return net.State.Equals(ClientState.Joined);
      }
   }

    /// <summary>
    /// Boolean to check if we're in a lobby or not.
    /// </summary>
    /// <value><c>true</c> if in lobby; otherwise, <c>false</c>.</value>
    public static bool inLobby{
        get {
            if (!inRoom) return false;

            return (string)net.CurrentRoom.CustomProperties[PhotonConstants.propScene] == "lobby";
        }
      }

	/// <summary>
	/// Boolean to check if we're in a game or not.
	/// </summary>
	/// <value><c>true</c> if in game; otherwise, <c>false</c>.</value>
	public static bool inGame {
		get {
          if (!inRoom) return false;
          Debug.Log((string)net.CurrentRoom.CustomProperties[PhotonConstants.propScene]);
          return (string)net.CurrentRoom.CustomProperties[PhotonConstants.propScene] != "lobby";
      }
	}

    /// <summary>
    /// Boolean to check if all players are ready in a game scene or not.
    /// </summary>
    /// <value><c>true</c> if in game; otherwise, <c>false</c>.</value>
    public static bool inGamePlayersReady {
      get {
          if (!inGame) return false;

          var pReady = 0;
          var lp = -1;
          var room = net.CurrentRoom;
          foreach(var playerPair in room.Players) {
            var player = playerPair.Value;
            object value;
            if (player.CustomProperties.TryGetValue(ClientEntity.playerStatus, out value)){
                if ((bool)value) pReady++;
                if (player.IsLocal) lp = (bool)value ? 2 : 1;
            }
          }
          return pReady == room.MaxPlayers;
      }
   }

	/// <summary>
	/// Enqueue a network update to be sent. Network events are processed both on Update and LateUpdate timings.
	/// </summary>
	public static bool netMessage(byte eventCode, object eventContent, bool sendReliable = false, RaiseEventOptions options = null) {
    if (!inRoom || !isReady) return false; // Only actually send messages when in game and ready

		if (options == null) options = RaiseEventOptions.Default;

		return net.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
	}

	/// <summary>
	/// Get the local player id. if isOnline isn't true, this will be -1
	/// </summary>
	public static int localID {
		get {
			if (net == null) return 0;

			return net.LocalPlayer.ID;
		}
	}

	#endregion

  public static event System.Action<EventData> netHook;
  public static event System.Action<EventData> onLeave;
  public static event System.Action<EventData> onJoin;

  private static NetworkManager _instance;
  public static NetworkManager instance {
    get {
		if (_instance) return _instance;
		_instance = FindObjectOfType<NetworkManager>();
		if (_instance) return _instance;
		throw new System.Exception("Network manager not instanced in scene");
    }
    set {
	   _instance = value;
    }
  }

  public void Awake() {
    if (_instance != null) {
      Destroy(gameObject);
      return;
    }

    Debug.Log("Aweake");

	 instance = this;
	 DontDestroyOnLoad(gameObject);

	 // Initialize network

	 net = new NetLogic();
	}

  void OnDestroy() {
    if (net == null) return;

    // Only do service
    if (_instance == this) {
      net.Service(); // Service before disconnect to clear any blockers
      net.Disconnect();
    }
  }

  void Update() {
    net.Service();
  }


  void LateUpdate () {
    net.Service();
  }

  public class NetLogic : LoadBalancingClient {
    public NetLogic() {
      // Setup and launch network service
      AppId = NetworkManager.instance.appID;
		  AppVersion = NetworkManager.instance.gameVersion;

		  AutoJoinLobby = true;

		  // Register custom type handlers
		  StreamCustomTypes.Register();
      // StreamINightTypes.Register();   // Specific to Impurrishable Night

		  #if UNITY_WEBGL
		  Debug.Log("Using secure websockets");
		  this.TransportProtocol = ConnectionProtocol.WebSocketSecure;
		  #endif

		  if (ConnectToNameServer()){
		    Debug.Log("Connecting to Name Server");
		  } else {
		    Debug.Log("Name server connection failed.");
		  }
    }

    public event System.Action GamelistRefresh;

    public override void OnEvent(EventData photonEvent) {
	   base.OnEvent(photonEvent);

	   switch(photonEvent.Code) {
		  case EventCode.GameList:
		  case EventCode.GameListUpdate:
		    Debug.Log("Server List recieved");
		    if (GamelistRefresh != null) GamelistRefresh();
		    break;
      case EventCode.Join:
        if (onJoin != null)
          onJoin(photonEvent);
        break;
      case EventCode.Leave:
        if (onLeave != null)
          onLeave(photonEvent);
        break;
	   }

	   if (netHook != null)
		  netHook(photonEvent);

    }

    /// <summary>
    /// Joins a specific room by name. If the room doesn't exist (yet), it will be created implicitiy.
    /// Creates custom properties automatically for the room.
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="options"></param>
    /// <param name="lobby"></param>
    /// <param name="startingScene"></param>
    /// <returns></returns>
    public bool OpJoinOrCreateRoomWithProperties(string roomName, RoomOptions options, TypedLobby lobby, string startingScene = "lobby") {
      options.CustomRoomPropertiesForLobby = new [] { PhotonConstants.propScene };
      options.CustomRoomProperties = new Hashtable() { { PhotonConstants.propScene, startingScene }  };

      ClientEntity.CreatePlayerHashtable();

      return OpJoinOrCreateRoom(roomName, options, lobby);
    }

    public bool OpCreateRoomWithProperties(string roomName, RoomOptions options, TypedLobby lobby, string startingScene = "lobby") {
      options.CustomRoomPropertiesForLobby = new[] { PhotonConstants.propScene };
      options.CustomRoomProperties = new Hashtable() { { PhotonConstants.propScene, startingScene } };

      ClientEntity.CreatePlayerHashtable();

      return OpCreateRoom(roomName, options, lobby);
    }

  }
}

