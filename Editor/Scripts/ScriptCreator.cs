

namespace Aarware.Core.Editor {
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ScriptCreator : EditorWindow{

        const string packageRoot = "Packages/com.aarware.core";

        static string tplPath           = $"{packageRoot}/Editor/ScriptTemplates";
        static string androidStingsTpl  = $"{packageRoot}/Editor/Android/strings.xml.txt";
        static string androidStings     = "Assets/Plugins/Android/res/values/strings.xml";
        const string menuRoot           = "Assets/Scripts/";
        const string toolRoot           = "Aarware/Scripts/";

        [MenuItem(menuRoot + " Create Directories", false, 1)]
        [MenuItem(toolRoot + "Create Directories", false, 0)]
        public static void CreateDirStructure(){

            string[] toplevel = new string[]{"Art", "Audio", "Editor", "Fonts", "Scenes", "Scripts", "Prefabs", "External"};
            string[] artlevel = new string[]{"Sprites", "Shaders", "Animations", "Textures", "Materials"};

            // Create all toplevels
            for(int i =0; i<toplevel.Length;i++){
                CreateDir(toplevel[i]);
            }
            // Create all artLevels
            for(int i =0; i<artlevel.Length;i++){
                CreateDir(artlevel[i], "/Art");
            }
        }

        /// <summary>
        ///  Create Dirs if they do not exsits
        /// </summary>
        static void CreateDir(string dir, string path=""){
            //Debug.Log("Assets"+path+dir);
            //return;
            if(!AssetDatabase.IsValidFolder("Assets"+path+"/"+dir )){
                string guid = AssetDatabase.CreateFolder("Assets"+path, dir);
                string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            }
            
        }
        static void CreateScriptAsset(string templatePath, string destName) {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, destName);
        }
   
        

        
    // BLANK
        [MenuItem(menuRoot + "Blank/Mono", false, 1)]
        [MenuItem(toolRoot + "Blank/Mono", false, 1)]
        private static void BlankMono(){			
            CreateScriptAsset($"{tplPath}/BlankMono.cs.txt", "NewBlankMono.cs");
        }
        [MenuItem(menuRoot + "Blank/Class", false, 1)]
        [MenuItem(toolRoot + "Blank/Class", false, 1)]
        private static void BlankClass(){			
            CreateScriptAsset($"{tplPath}/BlankClass.cs.txt", "NewBlankClass.cs");
        }
        [MenuItem(menuRoot + "Blank/Static", false, 1)]
        [MenuItem(toolRoot + "Blank/Static", false, 1)]
        private static void BlankStatic(){			
            CreateScriptAsset($"{tplPath}/BlankStatic.cs.txt", "NewBlankStatic.cs");
        }

    //SINGLETONS
        [MenuItem(menuRoot + "Singleton/GameObject Destroy", false, 0)]
        [MenuItem(toolRoot + "Singleton/GameObject Destroy", false, 0)]
        private static void SingletonGameObject(){			
            CreateScriptAsset($"{tplPath}/SingletonGameObject.cs.txt", "NewSingletonScript.cs");
        }
        [MenuItem(menuRoot + "Singleton/Component Destroy", false, 0)]
        [MenuItem(toolRoot + "Singleton/Component Destroy", false, 0)]
        private static void SingletonComponent(){			
            CreateScriptAsset($"{tplPath}/SingletonComponent.cs.txt", "NewSingletonScript.cs");
        }
        [MenuItem(menuRoot + "Singleton/NO DESTROY", false, 0)]
        [MenuItem(toolRoot + "Singleton/NO DESTROY", false, 0)]
        private static void SingletonLive(){			
            CreateScriptAsset($"{tplPath}/SingletonDontDestroy.cs.txt", "NewNDSingletonScript.cs");
        }
    // MONO LOADED
        [MenuItem(menuRoot + "Loaded/2D", false, 0)]
        [MenuItem(toolRoot + "Loaded/2D", false, 0)]
        private static void Loaded2D(){			
            CreateScriptAsset($"{tplPath}/Mono2D.cs.txt", "New2DScript.cs");
        }

        [MenuItem(menuRoot + "Loaded/3D", false, 0)]
        [MenuItem(toolRoot + "Loaded/3D", false, 0)]
        private static void Loaded3D(){			
            CreateScriptAsset($"{tplPath}/Mono3D.cs.txt", "New3DScript.cs");
        }

     
        private static void PingConfig(){
            // Load object
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("", typeof(UnityEngine.Object)); 
            // Select the object in the project folder
            Selection.activeObject = obj; 
            // Also flash the folder yellow to highlight it
            EditorGUIUtility.PingObject(obj);
        }

    }

    // custom script placeholder replacement
    public class KeywordReplace : UnityEditor.AssetModificationProcessor {
    
    
    public static void OnWillCreateAsset (string path) {
        
            path = path.Replace( ".meta", "" );
            int index = path.LastIndexOf( "." );
            if (index < 0){
                return;
            }
    
            string ext = path.Substring( index );
            if (ext != ".cs"){
                return;
            }
    
            index = Application.dataPath.LastIndexOf( "Assets" );
            path = Application.dataPath.Substring( 0, index ) + path;
            if (!System.IO.File.Exists( path )){
                Debug.Log("no - " + path);
                return;
            }
            string fileContent = System.IO.File.ReadAllText( path );
            
            fileContent = fileContent.Replace( "#HEADER#", 
@"/// <summary>
/// Script:  "+ System.IO.Path.GetFileName(path) + @"
/// Created: "+ System.DateTime.Now + @"
/// Author:  "+ System.Environment.UserName + @"
/// </summary>" );
            System.IO.File.WriteAllText( path, fileContent );
            AssetDatabase.Refresh();
        }
    }
}