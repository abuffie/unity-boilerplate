/// <summary>
/// Script:  CanvasBounds.cs
/// Created: --
/// Author:  abuffie
/// </summary>
using UnityEngine;
using UnityEngine.UI;
namespace Aarware.Core{
[RequireComponent(typeof(RectTransform))]
public class CanvasBounds : MonoBehaviour {

    void Reset()        => SetSize();
    void OnValidate()   => SetSize();

    void SetSize() {
        
        var rt = GetComponent<RectTransform>();  
        var cs = GetComponentInParent<CanvasScaler>();

        if (cs != null){
            // Access the reference resolution
            rt.sizeDelta = cs.referenceResolution;
        }else{
            Debug.LogError("No CanvasScaler in Parent, CanvasBounds self disabling");
            gameObject.SetActive(false);
        }
    }
}
}