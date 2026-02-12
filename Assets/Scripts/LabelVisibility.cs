using UnityEngine;

public class LabelVisibility : MonoBehaviour
{
    // These two lines create the empty boxes you need in the Inspector
    public GameObject highlightObject; 
    public CanvasGroup mainUI;        

    void Update()
    {
        // This makes the highlight show ONLY when you click the label in Unity
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            if(highlightObject != null) highlightObject.SetActive(true);
        }
        else
        {
            if(highlightObject != null) highlightObject.SetActive(false);
        }
        #endif
    }
}