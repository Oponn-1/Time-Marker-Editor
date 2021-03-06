using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//******************************************* Time Editor Unity Window **********************************************//
//
//      Author:         Andres De La Fuente Duran
//
//      Use:            This script can simply be included in your Editor file. It adds a new editor window
//                      accessible from the Windows drop down list.
//
//                      The point of this is to allow for the generation of timing information with respect
//                      to some corresponding audio file, like a piece of music. The window is intended to make it
//                      easy for you to mark specific times in the audio as you listen to it.
//                      Editing functions are built in to make this process more productive as well.




//****************************************** Class: Time Editor ******************************************************//
// 
//      Use:        The editor window extension for the above functionality.
//
//      Fields:     
//                  Audio Playback ..............................................................................
//
//                  musicFile:              The filepath for the audio to be loaded
//                  
//                  musicFileInProject:     The filename with extension for use with FileUtil
//
//                  musicFileNoExtension:   The filename for use with Resources.load()
//
//                  musicPlayer:            The game object to playback the audio selected
//
//                  Playback Visuals ...........................................................................
//                  
//                  musicPreview:           The image preview of the selected audio generated by Unity
//                  
//                  previewImageLength:     Width of the music preview
//
//                  playbackMarker:         The object that represents current playback time
//
//                  playbackMarkerColor:    The color for the playback marker in the GUI
//
//                  oldColor:               Color to return to after rendering GUI elements with unique colors
//
//                  Timing Marks ..............................................................................
//
//                  marks:                  List of current timing marks in this Time Editor window
//
//                  selectedMarks:          List of selected timing marks
//
//                  loadedDoc:              The timing marks loaded from JSON
//
//                  timingDoc:              The timing marks of the current Time Editor window to save
//
//                  Control Flow ..............................................................................
//
//                  musicLoaded:            Whether there is an audio file loaded 
//          
//                  moveMarks:              Whether the movement of timing marks is enabled
//
//                  externalMusic:          Whether the music to be loaded is not in the project
//
//                  Other .....................................................................................
//                  
//                  indexPosition:          The current position of the playback marker
//      
//                  yPosition:              The y coordinate at which to render the audio visualization and marks
//
//                  musicLength:            Duration in seconds of the loaded audio
//
//                  buttonHeight:           The height of all the buttons in the window
//
//      Methods:
//                  ShowWindow():           Show this window in the Windows menu
//
//                  Update():               Keep track of current playback timing information
//
//                  OnGUI():                Render the contents of the Time Editor
//
//                  HandleEvents(Event):    Have each object in the Time Editor handle user events 


public class TimeEditor : EditorWindow
{
    string musicFile = "";
    string musicFileInProject = "/TE_music.mp3";
    string musicFileNoExtension = "TE_music";
    GameObject musicPlayer;

    Texture musicPreview;
    float previewImageLength = 1000;
    EditorPlaybackMarker playbackMarker;
    Color playbackMarkerColor = new Color(0, 0, 100, 50f);
    Color oldColor;

    bool musicLoaded = false;
    bool moveMarks = false;
    bool externalMusic = false;
    
    float indexPosition = 0;
    float yPosition = 20;
    float musicLength = 0;

    float buttonHeight = 30;

    List<EditorTimeMarker> marks = new List<EditorTimeMarker>();
    List<EditorTimeMarker> selectedMarks = new List<EditorTimeMarker>();

    TimingDocument loadedDoc;
    TimingDocument timingDoc;


    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Time Editor")]

    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TimeEditor));
        AssetDatabase.Refresh();
    }

    private void Update()
    {
        if (musicLoaded && playbackMarker != null)
        {
            // Update timing information according to playback
            float musicTime = musicPlayer.GetComponent<AudioSource>().time;
            indexPosition = (musicTime / musicLength) * previewImageLength;
            playbackMarker.box.x = indexPosition;
            playbackMarker.time = musicTime;
            Repaint();
        }
    }

    void OnGUI()
    {
        playbackMarkerColor = new Color(0, 1, 1, 0.5f);
        oldColor = GUI.color;

        //**************** MUSIC VISUALIZATION ****************************
        GUILayout.BeginVertical(GUILayout.Height(150));
        GUILayout.Label("");
        

        if (musicLoaded)
        {
            // Audio Preview
            GUI.DrawTexture(new Rect(0, yPosition, previewImageLength, 100), musicPreview);
            // Time Markers
            if (marks.Count > 0)
            {
                foreach (EditorTimeMarker m in marks)
                {
                    if (m.selected)
                    {
                        GUI.color = Color.red;
                        GUI.Box(m.box, "");
                        GUI.color = oldColor;
                    }
                    else
                    {
                        GUI.Box(m.box, "");
                    }
                }
            }
            if (playbackMarker == null)
            {
                playbackMarker = new EditorPlaybackMarker(indexPosition, yPosition, 0);
            }
            else
            {
                GUI.color = playbackMarkerColor;
                GUI.Box(playbackMarker.box, "");
                GUI.color = oldColor;
            }
        }
        
        GUILayout.EndVertical();
        

        //***************** PLAYBACK CONTROLS *****************************

        GUILayout.Label("Audio Controls", EditorStyles.largeLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play", GUILayout.Height(buttonHeight)))
        {
            musicPlayer.GetComponent<AudioSource>().Play();
        }
        if (GUILayout.Button("Pause", GUILayout.Height(buttonHeight)))
        {
            musicPlayer.GetComponent<AudioSource>().Pause();
        }
        if (GUILayout.Button("Stop", GUILayout.Height(buttonHeight)))
        {
            musicPlayer.GetComponent<AudioSource>().Pause();
            musicPlayer.GetComponent<AudioSource>().time = 0;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        //****************** MARKERS CONTROLS *****************************

        GUILayout.Label("Marker Controls", EditorStyles.largeLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add Marker", GUILayout.Height(buttonHeight)))
        {
            marks.Add(new EditorTimeMarker(indexPosition, yPosition, musicPlayer.GetComponent<AudioSource>().time));
        }
        if (GUILayout.Button("Delete Selection", GUILayout.Height(buttonHeight)))
        {
            foreach (EditorTimeMarker m in selectedMarks)
            {
                if (m.selected)
                {
                    marks.Remove(m);
                }
            }
            selectedMarks.Clear();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Save", GUILayout.Height(buttonHeight)))
        {
            timingDoc = new TimingDocument(marks, musicLength);
            Debug.Log("timing doc array length: " + timingDoc.timeMarks.Count);
            string timingDocJson = JsonUtility.ToJson(timingDoc);
            Debug.Log(timingDocJson);
            string path = EditorUtility.SaveFilePanel(
                "Save timing information as txt",
                Application.dataPath + "/Resources",
                musicFileNoExtension + "Timing.txt",
                "txt");
            File.WriteAllText(path, timingDocJson);
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Load Time Marks", GUILayout.Height(buttonHeight)))
        {
            string path = EditorUtility.OpenFilePanel(
                "Load timing marks file",
                Application.dataPath,
                "txt");
            string json = File.ReadAllText(path);
            loadedDoc = JsonUtility.FromJson<TimingDocument>(json);
            marks = loadedDoc.timeMarks;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        moveMarks = GUILayout.Toggle(moveMarks, "Move Marks");

        GUILayout.Space(20);


        //*************** AUDIO SOURCE SETUP ******************************

        GUILayout.Label("Setup", EditorStyles.largeLabel);

        if (GUILayout.Button("Spawn Music Player", GUILayout.Height(buttonHeight)))
        {
            musicPlayer = new GameObject("Time Editor Music Player");
        }

        EditorGUILayout.BeginHorizontal();        

        externalMusic = GUILayout.Toggle(externalMusic, "Load External Music File");

        if (GUILayout.Button("Select Music", GUILayout.Height(buttonHeight)))
        {
            if (externalMusic)
            {
                musicFile = EditorUtility.OpenFilePanel("Load mp3 file", "", "mp3");

                if (musicFile != "")
                {
                    FileUtil.CopyFileOrDirectory(musicFile, Application.dataPath + "/Resources" + musicFileInProject);
                    AssetDatabase.Refresh();
                }
            } else
            {
                musicFile = EditorUtility.OpenFilePanel("Choose mp3 file", Application.dataPath + "/Resources", "mp3");
            }
        }
        if(GUILayout.Button("Add to Player", GUILayout.Height(buttonHeight)))
        {
            if (musicPlayer.GetComponent<AudioSource>() == null)
            {
                musicPlayer.AddComponent<AudioSource>();
            }
            musicPlayer.GetComponent<AudioSource>().clip = Resources.Load(musicFileNoExtension) as AudioClip;
            musicLength = musicPlayer.GetComponent<AudioSource>().clip.length;
            musicPreview = AssetPreview.GetAssetPreview(musicPlayer.GetComponent<AudioSource>().clip);
            musicLoaded = true;
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Label(musicFile, EditorStyles.boldLabel);

        GUILayout.Space(20);


        //************************* CLEANUP *****************************

        if (GUILayout.Button("Clean Up", GUILayout.Height(buttonHeight)))
        {
            musicLoaded = false;
            FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/" + musicFileInProject);
            GameObject.DestroyImmediate(musicPlayer);
            marks.Clear();
            AssetDatabase.Refresh();

        }

        if (musicLoaded && playbackMarker != null)
        {
            HandleEvents(Event.current);
        }
    }

    private void HandleEvents(Event e)
    {
        foreach (EditorTimeMarker m in marks)
        {
            m.HandleEvents(e, previewImageLength, musicLength, moveMarks, selectedMarks);
        }

        playbackMarker.HandleEvents(e, previewImageLength);

        if (e.type == EventType.mouseDrag)
        {
            if (playbackMarker.moved)
            {
                musicPlayer.GetComponent<AudioSource>().time = (playbackMarker.box.x / previewImageLength) * musicLength;
                playbackMarker.moved = false;
            }
        }
    }
}
