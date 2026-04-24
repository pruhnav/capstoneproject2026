using UnityEngine;

public class LoadAssetModelToAnchor : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform modelAnchor;
    public GameObject modelPrefab;

    public void LoadModel()
    {
        if (modelAnchor == null)
        {
            Debug.LogError("ModelAnchor is missing.");
            return;
        }

        if (modelPrefab == null)
        {
            Debug.LogError("Model prefab is missing.");
            return;
        }

        GameObject loadedModel = Instantiate(modelPrefab, modelAnchor);

        loadedModel.transform.localPosition = Vector3.zero;
        loadedModel.transform.localRotation = Quaternion.identity;
        loadedModel.transform.localScale = Vector3.one;

        Debug.Log("Loaded model into ModelAnchor: " + loadedModel.name);
    }
}