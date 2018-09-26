using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public GameObject GO_OrderPanel = null;
    private PizzaOrder Order;

    public PizzaOrder GetOrder()
    {
        return Order;
    }

    public void ToggleOrderDisplay(ShopSpot shopSpot = null)
    {
        if (shopSpot != null) shopSpot.ToggleOrder(Order);
        if (GO_OrderPanel != null) GO_OrderPanel.SetActive(!GO_OrderPanel.activeSelf);
    }

    private void OnEnable()
    {
        GenerateOrder();
        if (GO_OrderPanel != null && GO_OrderPanel.activeSelf) GO_OrderPanel.SetActive(false);
    }

    private void GenerateOrder()
    {
        // Generate random requested ingredients.
        Order = new PizzaOrder();
        Order.Ingredients = new Dictionary<PizzaData.EIngredients, bool>();
        for (int i = 0; i < System.Enum.GetValues(typeof(PizzaData.EIngredients)).Length; i++)
        {
            int i_State = Random.Range(0, 3);
            Debug.Log("GeneratingPizza Ingredient " + i + " | state = " + i_State);
            if (i_State == 0) Order.Ingredients.Add((PizzaData.EIngredients)i, false);
            else Order.Ingredients.Add((PizzaData.EIngredients)i, true);
        }

        // Check if no ingredients have been selected.
        bool b_OneTrue = false;
        for (int i = 0; i < System.Enum.GetValues(typeof(PizzaData.EIngredients)).Length; i++)
        {
            Order.Ingredients.TryGetValue((PizzaData.EIngredients)i, out b_OneTrue);
            if (b_OneTrue) break;
        }

        // If no ingredients have been selected, select TomatoSauce.
        if (!b_OneTrue)
        {
            Order.Ingredients.Remove(PizzaData.EIngredients.TomatoSauce);
            Order.Ingredients.Add(PizzaData.EIngredients.TomatoSauce, true);
        }
    }
}

public struct PizzaOrder
{
        public Dictionary<PizzaData.EIngredients, bool> Ingredients;
}