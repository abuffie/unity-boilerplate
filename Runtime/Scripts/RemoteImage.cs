/// <summary>
/// Script:  RemoteImage.cs
/// Created: 6/13/2022 10:01:00 AM
/// Author:  AaronBuffie
/// 
/// Loads images from urls into sprites and UI
/// * Right now all is done using tasks so it wont work with webgl
/// </summary>

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Aarware.Core{
//TODO: this uses task which i think might not work on web, test it and add web support if needed

public class RemoteImage {

    /// <summary>
    /// Loads image from a url into a RawImage
    /// </summary>
    /// <param name="image"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async void LoadImage(UnityEngine.UI.RawImage image, string url){
        var text = await GetRemoteTexture(url);
        if(text != null){
            image.texture = text;
        }
    }
    /// <summary>
    /// Loads image from a url into UI Image
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async void LoadImage(UnityEngine.UI.Image image, string url){
        var text = await GetRemoteTexture(url);
        if(text != null){
            image.sprite = Sprite.Create(text, new Rect(0,0,text.width,text.height), new Vector2(0.5f, 0.5f) );
        }
    }
    /// <summary>
    /// Loads image from a url into Sprite
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async void LoadImage(SpriteRenderer sprite, string url, float unitSize=100f){
        var text = await GetRemoteTexture(url);
        if(text != null){
            sprite.sprite = Sprite.Create(text, new Rect(0,0,text.width,text.height), new Vector2(0.5f, 0.5f), unitSize);
        }
    }       


    /// <summary> 
    /// Loads a texture from a url 
    /// </summary>
    /// <remarks>
    /// This is a sync function so it is blocking and you must await it.
    /// Use LoadImage() for none blocking or see code to create a custom non blocking function
    /// </remarks>
    /// <param name="url"></param>
    /// <returns>Texture2D or Null on fail</returns>
    public static async Task<Texture2D> GetRemoteTexture ( string url ){
        using( UnityWebRequest www = UnityWebRequestTexture.GetTexture(url) ) {
            //begin requenst:
            var asyncOp = www.SendWebRequest();

            //await until it's done: 
            while( asyncOp.isDone==false ){
                await Task.Delay( 30 );
            }

            //read results:
            if( www.isNetworkError || www.isHttpError ){
                Debug.Log( $"Avatar Failed to load - { www.error }, URL:{ www.url }" );

                //nothing to return on error:
                return null;
            }else{
                //return valid results:
                return DownloadHandlerTexture.GetContent( www );
            }
        }
    }
}
}