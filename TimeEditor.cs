using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TimeEditor : EditorWindow
{
    string musicFile = "";

    string musicFileInProject = "/TE_music.mp3";
    string musicFileNoExtension = "TE_music";

    GameObject musicPlayer;

    Texture musicPreview;
    bool musicLoaded = false;
    bool moveMarks = false;
    bool externalMusic = false;

    float previewImageLength = 1000;
    float indexPosition = 0;
    float yPosition = 20;
    float musicLength = 0;

    float buttonHeight = 30;

    EditorPlaybackMarker playbackMarker;
    Color playbackMarkerColor = new Color(0, 0, 100, 50f);
    Color oldColor;

    List<EditorTimeMarker> marks = new List<EditorTimeMarker>();
    List<EditorTimeMarker> selectedMarks = new List<EditorTimeMarker>();

    TimingDocument loadedDoc;
    TimingDocument timingDoc;


    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Time Editor")]

    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TimeEditor));
        AssetDatabase.Refresh();
    }

    private void Update()
    {
        if (musicLoaded && playbackMarker != null)
        {
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
            GUI.DrawTexture(new Rect(0, yPosition, previewImageLength, 100), musicPreview);
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
            //musicPlaying = true;
        }
        if (GUILayout.Button("Pause", GUILayout.Height(buttonHeight)))
        {
            musicPlayer.GetComponent<AudioSource>().Pause();
            //musicPlaying = false;
        }
        if (GUILayout.Button("Stop", GUILayout.Height(buttonHeight)))
        {
            musicPlayer.GetComponent<AudioSource>().Pause();
            musicPlayer.GetComponent<AudioSource>().time = 0;
            //musicPlaying = false;
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
