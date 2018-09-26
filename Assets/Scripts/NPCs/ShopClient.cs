using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShopClient : MonoBehaviour {

    public delegate void ClientAction(ShopClient Sender, int State, int ShopPos);
    public static event ClientAction ClientStateSwitch;
    public enum EClientState { NewSpawn, SentToShop, AtShop, Served, SentAway };

    public PizzaRater rater = null;
    public BoxGraber graber = null;

    public float F_WaitingTime = 60f;

    private NavMeshAgent _NavMeshAgent = null;
    private OrderManager OrderManager = null;
    private PizzaOrder Order;
    private ShopSpot ReachedSpot = null;

    private int I_ShopPos = -1;
    private int I_State = -1; 

    // FOR TESTING ONLY!
    // --------------------
    public bool B_Served = false;
    public void Update() 
    {
        if (B_Served && I_State == (int)EClientState.AtShop) Serve();
    }
    // --------------------

    private void OnEnable()
    {
        Init();    
    }

    private void Init()
    {
        OrderManager = GetComponent<OrderManager>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();
        if (_NavMeshAgent != null)
        {
            _NavMeshAgent.isStopped = true;
            I_State = (int)EClientState.NewSpawn;
        }
        else Debug.LogError("ShopClient can't find any attached NavMeshAgent Components.");
    }

    public void Serve()
    {
        if (I_State == (int)EClientState.AtShop)
        {
            if (graber.B_GrabbedBox)
            {
                if (graber.GO_InZoneBox.GetComponent<PizzaBox>().B_PizzaStored == false) rater.RatePizza();
                else
                {
                    PizzaData Pizza = graber.GO_InZoneBox.GetComponent<PizzaBox>().StoredPizza;
                    rater.RatePizza(Order, Pizza);
                }
            }

            ReachedSpot.ToggleOrder(Order);
            AdvanceState(); B_Served = true;
        }
    }

    public bool SendToShop(Transform target, byte b_Pos)
    {
        if (I_State != (int)EClientState.NewSpawn) return false;

        // Set Agent to travel to shop spot.
        I_State = (int)EClientState.SentToShop;
        I_ShopPos = b_Pos;
        _NavMeshAgent.isStopped = false;
        _NavMeshAgent.SetDestination(target.position);
        return true;
    }

    public bool SendToExit(Transform target)
    {
        if (I_State != (int)EClientState.Served) return false;

        OrderManager.ToggleOrderDisplay();
        _NavMeshAgent.SetDestination(target.position);
        AdvanceState();
        return true;                
    }

    private void OnTriggerEnter(Collider other)
    {
        if (I_State == (int)EClientState.SentToShop) // Travelling to the Shop.
        {
            // Checking if the ShopClient is at the destination shop spot trigger.
            ReachedSpot = other.gameObject.GetComponent<ShopSpot>();
            if (ReachedSpot != null && ReachedSpot.I_ID == I_ShopPos)
            {
                Order = OrderManager.GetOrder();
                OrderManager.ToggleOrderDisplay(ReachedSpot);
                StartCoroutine(WaitToBeServed());
                AdvanceState();
            }
        }
        else if (I_State == (int)EClientState.SentAway)  // Travelling away from the Shop.
        {
            if (other.tag == "ClientEndPoint")
            {
                if (graber.GO_InZoneBox != null) graber.GO_InZoneBox.transform.parent = null;
                Destroy(gameObject);
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (I_State == (int)EClientState.AtShop)
    //    {
    //        if (collision.collider.tag == "Box") Serve();
    //        // TODO Add pizzabox parenting and checking HERE !@!@!@!@!@!@!@!@!@!@
    //    }
    //}

    private void AdvanceState()
    {
        I_State++;
        if (ClientStateSwitch != null) ClientStateSwitch(this, I_State, I_ShopPos);
    }

    private IEnumerator WaitToBeServed()
    {
        float f_ArriveTime = Time.time;
        float f_CurrentWait = 0f;

        Debug.Log("ClientNow Waiting to be served.");
        while (f_CurrentWait < F_WaitingTime && !B_Served)
        {
            f_CurrentWait = Time.time - f_ArriveTime;
            if (graber != null && graber.B_GrabbedBox) Serve();
            yield return null;
        }

        if (f_CurrentWait >= F_WaitingTime)
        {
            Debug.Log("Took too long to serve a client.");
            Serve();
        }

        yield return null;
    }
}