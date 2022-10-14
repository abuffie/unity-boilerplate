using UnityEngine;
using UnityEngine.UI;
using Aarware.Core;

public class SampleAvatar : MonoBehaviour{

    [SerializeField] RawImage remoteImage;
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] string image_url = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50";
    
    void Awake() {
        
        // load into RawImage - this is the most effient
        RemoteImage.LoadImage(remoteImage, image_url);

        // Load into Image
        RemoteImage.LoadImage(image, image_url);

        // Load into Sprite - uses CreateSprite
        RemoteImage.LoadImage(spriteRenderer, image_url, 100F);

    }
        
}

