using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaRater : MonoBehaviour
{
    public delegate void ScoreEvent(float F_Score);
    public static event ScoreEvent AddScore;

    [Tooltip("[0] = Standard TSauce, [1] = Standard Cheese")]
    public float[] F_StandardAmounts = new float[2];

    private float F_PizzaScore = -10f;

    public void RatePizza()
    {
        Debug.Log("NoPizza Penalty! Score -10f");
        // TODO - broadcast the terrible score for no pizza received here...
    }

    public void RatePizza(PizzaOrder order, PizzaData pizza)
    {
        foreach (PizzaData.EIngredients ingredient in order.Ingredients.Keys)
        {
            float f_Amount = 0f;
            pizza.Ingredients.TryGetValue(ingredient, out f_Amount);
            F_PizzaScore += (100f * (f_Amount / (F_StandardAmounts[(int)ingredient])));
            Debug.Log(ingredient.ToString() + " | New PizzaScore = " + F_PizzaScore);
        }

        if (pizza.state == PizzaData.ECookedState.Raw)
        {
            if (F_PizzaScore < 0) F_PizzaScore *= 2f;
            else F_PizzaScore *= 0.5f;
        }

        if (pizza.state == PizzaData.ECookedState.Burnt)
        {
            if (F_PizzaScore < 0) F_PizzaScore *= 3f;
            else F_PizzaScore *= 0.3f;
        }

        if (AddScore != null) AddScore(F_PizzaScore);
    }
}