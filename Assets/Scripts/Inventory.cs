using UnityEngine;

public class Inventory
{
    private static int[] stats = new[] { 2, 2, 2, 2 };

    public static void Add(Ingredient type, int amt)
    {
        stats[(int)type] += amt;
    }

    public static void Use(Ingredient type, int amt)
    {
        stats[(int)type] -= amt;
    }

    public static int Get(Ingredient type)
    {
        return stats[(int)type];
    }
}

public enum Ingredient
{
    PROTIEN, CARB, PRODUCE, SPICE
}
