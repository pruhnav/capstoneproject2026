using UnityEngine;

public class SimpleDelete : MonoBehaviour
{
    // This is the function you will link to your "X" button in the Inspector
    public void DeleteThisLabel()
    {
        // 'gameObject' refers to the pin this script is attached to
        Destroy(gameObject);
    }
}