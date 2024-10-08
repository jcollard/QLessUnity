using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DieController[] Dice;
    public DicePoolData DicePool;
    public void FindAllDieControllers()
    {
        Dice = FindObjectsByType<DieController>(FindObjectsSortMode.InstanceID);
    }

    public void Roll()
    {
        FindAllDieControllers();
        foreach (DieController die in Dice)
        {
            die.Face = DicePool.Next();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Roll();
    }
}
