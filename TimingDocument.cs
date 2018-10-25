using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
