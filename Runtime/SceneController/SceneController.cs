/// <summary>
/// Script:  SceneController.cs
/// Created: 10/13/2022 5:28:37 PM
/// Author:  abuffie
/// 
/// Simple scene loading with nice fade in/out
/// This does not destroy, only one instance is needed
/// </summary>
    
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoSingleton<SceneController>{
    
    [SerializeField] PanelController panel;

    private void Awake(){
        base.Awake(true);
    }

    public void GoToScene(Scene scene) => GoToScene(scene.name);
    public void GoToScene(int scene)    => GoToScene(SceneManager.GetSceneByBuildIndex(scene).name);

    public void GoToScene(string scene){

        SceneManager.sceneLoaded += Close;
        panel.Open(false, ()=>{
            SceneManager.LoadScene(scene);
        });
    }

    public void Close(Scene scene, LoadSceneMode loadSceneMode){
        SceneManager.sceneLoaded -= Close;
        panel.Close();
    }
}
   