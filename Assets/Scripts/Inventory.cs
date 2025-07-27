using UnityEngine;

public class Save
{
    private static int[] stats = new[] { 2, 2, 2, 2 };
    public static int day;

    public static Flavor majorFlavor, minorFlavor;

    public static void AddItem(Ingredient type, int amt)
    {
        stats[(int)type] += amt;
    }

    public static void UseItem(Ingredient type, int amt)
    {
        stats[(int)type] -= amt;
    }

    public static int GetItem(Ingredient type)
    {
        return stats[(int)type];
    }
}

public enum Ingredient
{
    PROTIEN, CARB, PRODUCE, SPICE
}

public enum Flavor
{
    SALT, SOUR, BITTER, UMAMI, SWEET, MAX
}
