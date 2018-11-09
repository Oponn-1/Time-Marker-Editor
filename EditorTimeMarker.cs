using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//      This contains a class which is used in my Time Editor, a custom Unity editor window to create
//      timing information with respect to an audio file.
//
//      Author:     Andres De La Fuente Duran


//******************************************* Class: EditorTimeMarker *********************************************//
//      
//      Use:        A class to represent the time markings in the corresponding Time Editor custom 
//                  Unity window. 
//
//      Fields:
//                  Public ......................................................................................
//                  
//                  box:        The rectangle of the marker's GUI representation
//                  
//                  time:       The time in seconds that the marker represents
//
//                  selected:   Whether this box is currently selected in the Time Editor
//
//                  Private ....................................................................................
//
//                  markWidth:  The width for 'box'
//
//                  markHeight: The height for 'box'
//
//      Methods:
//                  Public ......................................................................................
//
//                  HandleEvents:
//                      Parameters:
//                          e:              event to be processed
//                          imageLength:    width of the audio preview image in the Time Editor
//                          musicLength:    duration in seconds of the music loaded in the Time Editor
//                          moveMarks:      whether the movement of marks is enabled in the Time Editor
//                          selectedMarks:  list of all marks currently selected in the Time Editor
//                      Use:
//                          This function handles user events on the time markers in the Time Editor:
//                          -Movement, which is limited horizontally and only within the preview image
//                          -Selection
[Serializable]
public class EditorTimeMarker {

    public Rect box;
    public float time;
    public bool selected;

    private static float markWidth = 5;
    private static float markHeight = 100;

    public EditorTimeMarker(float x, float y, float markTime)
    {
        box = new Rect(x, y, markWidth, markHeight);
        time = markTime;
        selected = false;
    }

    public void HandleEvents(Event e, float imageLength, float musicLength, bool moveMarks, List<EditorTimeMarker> selectedMarks)
    {
        switch(e.type)
        {
            // Move the marker
            case EventType.mouseDrag:
                if (moveMarks)
                {
                    if (box.Contains(e.mousePosition))
                    {
                        box.position += e.delta.x * Vector2.right;
                        // bound to the dimensions of the preview image in the Time Editor
                        if (box.position.x > imageLength)
                        {
                            box.position = new Vector2(imageLength, box.position.y);
                        }
                        if (box.position.x < 0)
                        {
                            box.position = new Vector2(0, box.position.y);
                        }
                        // update time according to new position
                        time = (box.x / imageLength) * musicLength;
                    }
                }
                break;
            // Toggle selection status
            case EventType.ContextClick:
                if (box.Contains(e.mousePosition))
                {
                    selected = !selected;
                    if (selected)
                    {
                        selectedMarks.Add(this);
                    }
                    else
                    {
                        selectedMarks.Remove(this);
                    }
                }
                break;
        }
    }
	
}
