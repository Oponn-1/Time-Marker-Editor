using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//      This contains a class which is used in my Time Editor, a custom Unity editor window to create
//      timing information with respect to an audio file.
//
//      Author:     Andres De La Fuente Duran


//******************************************* Class: EditorPlaybackMarker *****************************************//
//      
//      Use:        A class to represent the playback marker in the Time Editor window.
//
//      Fields:
//                  Public ......................................................................................
//                  
//                  box:        The rectangle of the marker's GUI representation
//                  
//                  time:       The time in seconds the marker represents in the Time Editor window
//
//                  moved:      Whether the marker has been moved as a result of an event
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
//
//                      Use:
//                          This function handles user events on the time markers in the Time Editor:
//                          -Movement, which is limited horizontally and only within the preview image

public class EditorPlaybackMarker {

    public Rect box;
    public float time;
    public bool moved;

    private float width = 10;
    private float height = 100;

    public EditorPlaybackMarker(float x, float y, float setTime)
    {
        box = new Rect(x, y, width, height);
        time = setTime;
        moved = false;
    }

    public void HandleEvents(Event e, float imageLength)
    {
        switch(e.type)
        {
            // Move the marker
            case EventType.mouseDrag:
                if (box.Contains(e.mousePosition))
                {
                    box.position += e.delta.x * Vector2.right;
                    // bound movement by the size of the preview image in the Time Editor window
                    if (box.position.x > imageLength)
                    {
                        box.position = new Vector2(imageLength, box.position.y);
                    }
                    if (box.position.x < 0)
                    {
                        box.position = new Vector2(0, box.position.y);
                    }
                    moved = true;
                }
                break;
        }
    }
}
