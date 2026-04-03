using UnityEngine;

public class Pass_through  :MonoBehaviour
{
    public GameObject passthroughLayer;
    public GameObject[] unityBackgroundObjects;

    private bool isPassthroughOn = false;

    void Start()
    {
        ApplyMode();
    }

    public void TogglePassthrough()
    {
        isPassthroughOn = !isPassthroughOn;
        ApplyMode();
    }

    private void ApplyMode()
    {
        if (passthroughLayer != null)
            passthroughLayer.SetActive(isPassthroughOn);

        if (unityBackgroundObjects != null)
        {
            foreach (GameObject obj in unityBackgroundObjects)
            {
                if (obj != null)
                    obj.SetActive(!isPassthroughOn);
            }
        }
    }
}

