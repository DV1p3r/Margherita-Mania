using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClientManager : MonoBehaviour {

    public Transform[] NormalSpawnPoints = null;
    public Transform[] ExpertSpawnPoints = null;
    //public Transform[] ManiacSpawnPoints = null;

    public Transform[] SpawnPoints = null;
    public Transform[] EndPoints = null;
    public Transform[] ClientSpots = null;
    public GameObject _Client = null;

    public int I_ClientsToSpawn = 7;
    public int I_ClientsSpawned = 0;
    public float F_SpawnDelay = 3f;

    public delegate void ClientAction(int iValue);
    public static event ClientAction ClientsState;

    public bool B_Initialised = false;

    private List<Transform[]> ClientSpotModes = null;
    private bool[] B_ActiveSpots = null;    
    private int I_State = -1;

    public void StartRound()
    {
        if (B_Initialised) StartCoroutine(StartSpawns());
        else Debug.LogError("CLient Manager requested to start spawning before Init() finished.");
    }

    private void OnEnable()
    {
        if (SpawnPoints != null && EndPoints != null && ClientSpots != null && _Client != null) Init();
        MenuManager.GameMode += HandleModeUpdates;
        MenuManager.GameState += HandleStateUpdates;

        ClientSpotModes = new List<Transform[]>
        {
            new Transform[NormalSpawnPoints.Length],
            new Transform[ExpertSpawnPoints.Length],
            //new Transform[ManiacSpawnPoints.Length]
        };
        
        for (int i = 0; i < ExpertSpawnPoints.Length; i++) ClientSpotModes[1][i] = ExpertSpawnPoints[i];
        ClientSpots = ClientSpotModes[1];
        ToggleSpots(false);

        //for (int i = 0; i < ManiacSpawnPoints.Length; i++) ClientSpotModes[2][i] = ManiacSpawnPoints[i];
        //ClientSpots = ClientSpotModes[2];
        //ToggleSpots(false);

        for (int i = 0; i < NormalSpawnPoints.Length; i++) ClientSpotModes[0][i] = NormalSpawnPoints[i];
        ClientSpots = ClientSpotModes[0];
        ToggleSpots(true);
    }

    private void Init()
    {
        B_ActiveSpots = new bool[ClientSpots.Length];
        for (byte b = 0; b < B_ActiveSpots.Length; b++) B_ActiveSpots[b] = false;
        ShopClient.ClientStateSwitch += HandleClientUpdates;
        ToggleSpots(true);
        B_Initialised = true;
    }

    private void HandleClientUpdates(ShopClient Sender, int State, int ShopPos)
    {
        if (State == (int)ShopClient.EClientState.Served) StartCoroutine(HandleServedClient(Sender, ShopPos));
    }

    private void HandleModeUpdates(int iValue)
    {
        Debug.Log("Client Spots Updating");
        ToggleSpots(false);
        ClientSpots = ClientSpotModes[iValue];
        ToggleSpots(true);
    }

    private void ToggleSpots(bool b_State)
    {
        foreach (Transform t_Spot in ClientSpots)
        {
            ShopSpot spot = t_Spot.GetComponent<ShopSpot>();
            if (spot != null) spot.ToggleActive(b_State);
            else Debug.LogError("UnableTofindShopSpot");
        }
    }

    private void HandleStateUpdates(int iValue)
    {
        if (I_State == iValue) return;

        I_State = iValue;
        if (I_State == (int)MenuManager.EGameState.Playing) StartRound();
        if (I_State == (int)MenuManager.EGameState.End) Debug.Log("CleintManager sucessfully reached round end state.");
    }

    private IEnumerator SpawnClient()
    {        
        int i_Destination = GetFirstOpenSpot();

        if (i_Destination == -1) // Ensure Spawn Possible.
            Debug.Log("Client Manager unable to find destination for client");
        else
        {
            I_ClientsSpawned++;
            B_ActiveSpots[i_Destination] = true;

            // Spawn Client at a random spawn point.
            byte b_SpawnPos = (byte)Random.Range(0, SpawnPoints.Length);
            GameObject go_NewClient = Instantiate(_Client, SpawnPoints[b_SpawnPos]);

            // Attempt to send off client until successful. - TODO: ADD A FAIL-SAFE COUNTER + FRAME DELAY...
            while (!go_NewClient.GetComponent<ShopClient>().SendToShop(ClientSpots[i_Destination], (byte)i_Destination));            
        }
                  
        yield return null;
    }

    private int GetFirstOpenSpot()
    {
        Debug.Log("Getting First Active Spot. B_ActiveSpots.Length = " + B_ActiveSpots.Length);
        byte b = 0;
        while (b < B_ActiveSpots.Length)
        {
            Debug.Log("B = " + b);
            if (B_ActiveSpots[b] == false) return b;
            b++;
        }
        return -1;
    }   
    
    private void EndRound()
    {
        bool b_AllServed = true;
        foreach (bool bClientPresent in B_ActiveSpots)
        {
            if (bClientPresent) { b_AllServed = false; break; }
        }
        if (b_AllServed)
        {
            Debug.Log("All clients for the round have been served");

            if (ClientsState != null) ClientsState((int)MenuManager.EGameState.End);
        }
    }

    private IEnumerator HandleServedClient(ShopClient _ServedClient, int ShopPos)
    {
        B_ActiveSpots[ShopPos] = false;
        byte b_EndPos = (byte)Random.Range(0, EndPoints.Length);
        _ServedClient.SendToExit(EndPoints[b_EndPos]);
        StartCoroutine(SendNewClient());
        yield return null;
    }

    private IEnumerator SendNewClient()
    {
        if (I_ClientsSpawned >= I_ClientsToSpawn) // End-Of-Game
        {
            EndRound();
            yield break;
        }   
        float f_CurrentCount = 0;

        // Delay spawns on start.
        while (f_CurrentCount < F_SpawnDelay)
        {
            f_CurrentCount += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(SpawnClient());
        yield return null;
    }

    private IEnumerator StartSpawns()
    {
        int i_SpawnedClients = 0;
        while (i_SpawnedClients < ClientSpots.Length)
        {
            float f_CurrentCount = 0;

            // Delay spawns on start.
            while (f_CurrentCount < F_SpawnDelay)
            {
                f_CurrentCount += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(SpawnClient());
            i_SpawnedClients++;
            yield return null;
        }
    }
}