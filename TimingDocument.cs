using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//      This contains a class which is used in my Time Editor, a custom Unity editor window to create
//      timing information with respect to an audio file.
//
//      Author:     Andres De La Fuente Duran


//******************************************* Class: TimingDocument  ***********************************************//
//      
//      Use:        A class to represent the timing information for an entire piece of music. This is 
//                  used simply to represent the data in an object which can then be converted to JSON
//                  for saving and loading.
//
//      Fields:
//                  Public ......................................................................................
//                  
//                  timeMarks:  The list of time markers from the Time Editor window
//
//                  length:     The duration of the corresponding piece of music
//


[Serializable]
public class TimingDocument {

    public List<EditorTimeMarker> timeMarks;
    public float length;

    public TimingDocument(List<EditorTimeMarker> marks, float musicLength)
    {
        timeMarks = marks;
        length = musicLength;
    }
	
}
