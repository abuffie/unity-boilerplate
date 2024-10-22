namespace Aarware.Core.Editor{
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Script:  TextManager.cs
/// Created: 6/12/2024 11:12:34 PM
/// Author:  abuffie
/// </summary>
public class TextManager :EditorWindow {

        public string rootFolder = "LocaleStrings";
        // Tracking
        public int scaned        = 0;
        public int textMeshFound = 0;
        public int uiTextFound   = 0;
        public bool searchedComplete=false;
        LocalCollection locals;

        // Found Referances
        public Dictionary<GameObject, Text> uiTextObjs       = new Dictionary<GameObject, Text>();
        public Dictionary<GameObject, TextMesh> textMeshObjs = new Dictionary<GameObject, TextMesh>();
        
        // Editor/GUI Stuff
        public static EditorWindow  window;
        public bool init=false;
        public Vector2 scrollPos1,scrollPos2;	
        public MonoScript textUiScript, textMeshScript;

        // All the found strings as a big string
        public string allTheText="";

        // Regex Pattern filter
        public string regPat = @"(\d\/\d)|(\d\%)|(\d\d:\d\d\.\d)|(version?)|(build?)|(demo?)|(debug?)|(English)|(French)|(Italian)|(German)|(Spanish)|(Japanese)";
                
                
            
        public static TextManager i;
		public TextManager() { title = "TextManager"; i=this; EditorSceneManager.sceneClosing += delegate(Scene scene, bool removed){Reset();};}
        
        [MenuItem("Aarware/Text Manager")]
        public static void ShowExample() {
            i = GetWindow<TextManager>();
            i.titleContent = new GUIContent("MyEditorWindow");
        }
        
        // Opens Window
     /*   [MenuItem("GameSmart/Text Sniffer/Open")]
        public static void ShowWindow(){
            System.Type inspectorType = System.Type.GetType("UnityEditor.SceneView,UnityEditor.dll");
            window = EditorWindow.GetWindow<TextManager>(new System.Type[] {inspectorType});
            
        }
        [MenuItem("GameSmart/Text Sniffer/Close")]
        public static void CloseWindow(){
            window.Close();
        }
        [MenuItem("GameSmart/Text Sniffer/Search All", false, 8000)]
		static void TextFinder(){			
			var v = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.UI.Text));
			List<UnityEngine.UI.Text> texts = new List<UnityEngine.UI.Text>();
			foreach(var t in v){			
				texts.Add(t as UnityEngine.UI.Text);
			} 
			texts = texts.OrderBy( (text) => -text.fontSize ).ToList();
			foreach(var s in texts){
				Debug.Log(s.name + " : " + s.fontSize, s.gameObject);
			}
		}
*/
        // Clear all found refs
        private void Reset(){        
            searchedComplete=false;
            locals = new LocalCollection();
            textMeshObjs.Clear();
            uiTextObjs.Clear();        
            allTheText     = "";
            scaned         = 0;
            textMeshFound  = 0;
            uiTextFound    = 0;
        }
        
        public void CreateGUI(){
            var infoText = "Text Sniffer Will find all objects with UI Text or TextMeshes within the loaded Scene.\n\n";
            infoText += "All stings found will be writen to a kickass JSON file located @ "+rootFolder+"/{scene-name}/strings.json.\n\n";
            infoText += "It will also allow you to add the LocalizedText component or any script you choose to all objects found.";
           
            GUILayout.BeginHorizontal();
                GUILayout.Label("Text Sniffer",EditorStyles.boldLabel);
               // if (GUILayout.Button("X", GUILayout.Width(30))){ CloseWindow();}
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(infoText,MessageType.Info);          
                
            
                GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("Use this button to merge all Scenes' json files into one master",MessageType.Warning);
                    if(GUILayout.Button("Consolidate Locale Files", GUILayout.Width(200))){
                        ConsolidateLocales();
                    }
                GUILayout.EndVertical();
                
            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(20);

            int width = 280;

        // REGEX FIELD
            GUILayout.Label("Regex Filter",EditorStyles.whiteLabel);
            regPat = @GUILayout.TextField(regPat,GUILayout.Width(width));
            
        // SEARCH BUTTON 
            if (GUILayout.Button("Find All Text in scene\n"+SceneManager.GetActiveScene().name, GUILayout.Width(width))){
                FindInScene();
            }

        // Draw Found objs with found scripts
            if(searchedComplete){
                if (GUILayout.Button("Clear Search", GUILayout.Width(width))){
                    Reset();
                }

                GUILayout.Space(20);
                GUILayout.Label(string.Format("Scaned {0} GameObjects", scaned));			
                EditorGUILayout.HelpBox("Strings saved @ "+ rootFolder +"/"+SceneManager.GetActiveScene().name+"/strings.json", MessageType.Info);
                if (GUILayout.Button("Write to json", GUILayout.Width(width))){
                    WriteSceneLocales();
                }
                
                GUILayout.BeginHorizontal();
                var lwidth = EditorGUIUtility.labelWidth;            
                EditorGUIUtility.labelWidth=40;

                if(uiTextObjs.Count>0){
                    EditorGUILayout.BeginVertical();
                    DrawUiTextObjs();
                    EditorGUILayout.EndVertical();
                
                }
                GUILayout.Space(10);
                if(textMeshObjs.Count>0){
                    EditorGUILayout.BeginVertical();
                    DrawTextMeshObjs();
                    EditorGUILayout.EndVertical();
                
                }
                EditorGUIUtility.labelWidth = lwidth; 
                
            GUILayout.EndHorizontal();
            }
        }

    // TEXT UI FOUND
        private  void DrawUiTextObjs(){
            GUILayout.Label(string.Format("UI TEXT -> found {0}", uiTextFound),EditorStyles.boldLabel);
            
            // Add Script
            GUILayout.Label("Add Script:");
            GUILayout.BeginHorizontal();
            textUiScript = EditorGUILayout.ObjectField(textUiScript, typeof(MonoScript), true, GUILayout.Width(200)) as MonoScript;
            if( GUILayout.Button("Apply", GUILayout.Width(45)) ) {
                foreach(KeyValuePair<GameObject, Text> t in uiTextObjs){
                    AttachComponent(textUiScript, t.Key);
                }
            }
            GUILayout.EndHorizontal();

            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1, GUILayout.Width(300), GUILayout.Height(200));	

            List<GameObject> removeItems = new List<GameObject>();
            // List Found
            foreach(KeyValuePair<GameObject, Text> t in uiTextObjs){
                if(t.Key==null){Reset(); break;} // hack for scene change
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(t.Key.name + "\n"+ t.Value.text, GUILayout.Width(300))){
                    EditorGUIUtility.PingObject(t.Key ); // Ping in tree
                    Selection.activeGameObject=t.Key;// open inspector
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(t.Value, true); // expand componenet in inspector
                }
                if (GUILayout.Button("X", GUILayout.Width(30))){
                    removeItems.Add(t.Key);
                }
                GUILayout.EndHorizontal();			
            }
            if(removeItems.Count>0){
                foreach(var i in removeItems){
                    uiTextObjs.Remove(i);
                }
            }

            EditorGUILayout.EndScrollView();
        }
    // TEXT MESHES FOUND
        private  void DrawTextMeshObjs(){       
            GUILayout.Label(string.Format("TEXT MESH -> found {0}",textMeshFound),EditorStyles.boldLabel);

            // Add Script
            GUILayout.Label("Add Script:");
            GUILayout.BeginHorizontal();
            textMeshScript = EditorGUILayout.ObjectField(textMeshScript, typeof(MonoScript), true, GUILayout.Width(200)) as MonoScript;
            if( GUILayout.Button("Apply", GUILayout.Width(45)) ) {
                foreach(KeyValuePair<GameObject, TextMesh> t in textMeshObjs){
                    AttachComponent(textMeshScript, t.Key);
                }
            }
            GUILayout.EndHorizontal();

            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.Width(200), GUILayout.Height(300));
            // List Found
            foreach(KeyValuePair<GameObject, TextMesh> t in textMeshObjs){
                    
                if (GUILayout.Button(t.Key.name + "\n"+ t.Value.text, GUILayout.Width(200))){
                    EditorGUIUtility.PingObject(t.Key ); // Ping in tree
                    Selection.activeGameObject=t.Key;// open inspector
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(t.Value, true); // expand componenet in inspector
                }
            }	

            EditorGUILayout.EndScrollView();
        }
    
    // SEARCH FUNCTION
        private void FindInScene(){
            Reset(); 
            // only pull root active/disabled objects in scene 
            //will loop through this so we can find *disabled* children along with the active
            GameObject[] go = SceneManager.GetActiveScene().GetRootGameObjects(); 
        
            foreach (GameObject g in go){
                FindInGO(g);	
            }
          /*  Directory.CreateDirectory(rootFolder);
            Directory.CreateDirectory(rootFolder+"/"+SceneManager.GetActiveScene().name);
            //var json = JsonUtility.ToJson(locals,true);
            // Write found text to a txt
            File.WriteAllText(rootFolder+"/"+SceneManager.GetActiveScene().name+"/strings.txt", allTheText);
            // write found text to json           
            File.WriteAllText(rootFolder+"/"+SceneManager.GetActiveScene().name+"/strings.json", JsonUtility.ToJson(locals, true));
*/
            /*for(int i=1; i<= LocalManager.supported; i++){            
                File.WriteAllText("Assets/StreamingAssets/Locals/"+LocalManager.GetLocalName(i)+".json", json);
            }*/
            searchedComplete=true;
        }
    // SEARCH OBJECT 	
        private void FindInGO(GameObject g){
            // Pull Text Components		
            TextMesh[] textMesh = g.GetComponents<TextMesh>();
            Text[]     textUi   = g.GetComponents<Text>();
            // Track counts
            textMeshFound += textMesh.Length;
            uiTextFound   += textUi.Length;
            scaned++;
            
            // get location path in scene
            string hp = GetHierarchyPath(g);

        // Run Through TextMesh Component
            for (int i = 0; i < textMesh.Length; i++)	{
                // SKIP FILTERED FIELDS i.e NUMBERS ARE NOT LOCALIZED...
                if(IsFiltered(textMesh[i].text, regPat) ){
                    Debug.Log(hp + " SKIPPING NUMBER: "  + textMesh[i].text, g);
                    continue;
                }
                // ADD TO OUR OBJECT RESULT DICT
                textMeshObjs.Add(g,textMesh[i]);             
                // TEXT HAS VALUE, ADD TO JSON
                AddLocalItem(textMesh[i].text);
                Debug.Log (hp + " has text mesh:\n" + textMesh[i].text, g);
            }

        // Run Through UI Text Component
            for (int i = 0; i < textUi.Length; i++){
                // SKIP FILTERED FIELDS i.e NUMBERS ARE NOT LOCALIZED...
                if(IsFiltered(textUi[i].text, regPat) ){
                    Debug.Log(hp + " SKIPPING NUMBER: "  + textUi[i].text, g);
                    continue;
                }
                // ADD TO OUR OBJECT RESULT DICT 
                uiTextObjs.Add(g,textUi[i]);
                // TEXT HAS VALUE, ADD TO JSON
                AddLocalItem(textUi[i].text);
                Debug.Log (hp + " has ui text:\n" + textUi[i].text, g);			
            }
                    

            // this will get children in objects including disabled children
            foreach (Transform childT in g.transform) {
            FindInGO(childT.gameObject);
            }
        }

        
        private void AttachComponent(MonoScript script, GameObject g){
            if(script!=null ){
                var has = g.GetComponent(script.GetClass());
                if(has==null){
                    Debug.Log(" Adding component " + script.name + " to " + g.name);
                    g.AddComponent(script.GetClass());
                    EditorUtility.SetDirty(g);
                    /* This is still beging work out
                    var prefab = PrefabUtility.GetPrefabParent(g);
                    if(prefab !=null){
                        Debug.Log("applying to prefab "+ prefab.name);
                        PrefabUtility.ReplacePrefab(g, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    }else{
                        Debug.Log(g.name + " Has no Prefab");
                    }
                    */
                }else{
                    Debug.Log(g.name + " has component " + script.name + " already");
                }
            } 
        }

        private string GetHierarchyPath(GameObject g){
            string s = g.name;
            Transform t = g.transform;
            while (t.parent != null){
                s = t.parent.name +"/"+s;
                t = t.parent;
            }
            return s;
        }

        private bool IsFiltered(string text, string regex){
            var rx = new Regex(regex,RegexOptions.IgnoreCase);
            int number;
            return (int.TryParse(text, out number) || rx.IsMatch(text) );
        }

        // adds text to json, skips dups
        private bool AddLocalItem(string text){
            if(text.Length>0){
                var has = locals.localItem.Find(l => l.key == text);
                if(has==null){
                    var item = new LocalItem();
                    item.key = text;
                    item.text = "LOCAL!";
                    locals.localItem.Add(item); 
                    allTheText += text +"\n~\n";
                    /*
                    var sb = new System.Text.StringBuilder(text.Length);

                    foreach (char i in text)
                        if (i == '\n'){
                            sb.Append("\\n");
                        }else{
                            sb.Append(i);
                        }
    */
                    //allTheText += sb.ToString() + "~";
                    //allTheText += text.Replace("\n", "\\n") + "~";               
                    return true;
                }
                return false;
            }
            return false;
        }

        private void WriteSceneLocales(){
            Directory.CreateDirectory(rootFolder);
            Directory.CreateDirectory(rootFolder+"/"+SceneManager.GetActiveScene().name);
            //var json = JsonUtility.ToJson(locals,true);
            // Write found text to a txt
            File.WriteAllText(rootFolder+"/"+SceneManager.GetActiveScene().name+"/strings.txt", allTheText);
            // write found text to json           
            File.WriteAllText(rootFolder+"/"+SceneManager.GetActiveScene().name+"/strings.json", JsonUtility.ToJson(locals, true));
        }

        private void ConsolidateLocales(){
            string[] dirs = Directory.GetDirectories(rootFolder);
            LocalCollection master = new LocalCollection();
            // go through the dirs
            foreach(string dir in dirs){
                // read each json file in dir
                foreach (string file in Directory.GetFiles(dir, "*.json")){
                    var locales = JsonUtility.FromJson<LocalCollection>(File.ReadAllText(file));
                    // if locale not in master add it
                    foreach(var locale in locales.localItem){
                        if(master.localItem.Find(l => l.key == locale.key) != null){
                            continue;
                        }
                        var item = new LocalItem();
                        item.key = locale.key;
                        item.text = "translation ok key goes here";
                        master.localItem.Add(item); 
                    }

                }
            }
            if(master.localItem.Count>0){
                File.WriteAllText(rootFolder+"/master.json", JsonUtility.ToJson(master, true));
            }
            
        }

    // FOR BUILDING JSON LOCAL FILES
        [System.Serializable]
        public class LocalCollection{
            public List<LocalItem> localItem = new List<LocalItem>();
        }
        [System.Serializable]
        public class LocalItem{
            public string key;
            public string text;
        }
    }
}


