using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class EditorTimeMarker {

    public Rect box;
    public float time;

    static float markWidth = 5;
    static float markHeight = 100;

    public bool selected;

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
            case EventType.mouseDrag:
                if (moveMarks)
                {
                    if (box.Contains(e.mousePosition))
                    {
                        box.position += e.delta.x * Vector2.right;
                        if (box.position.x > imageLength)
                        {
                            box.position = new Vector2(imageLength, box.position.y);
                        }
                        if (box.position.x < 0)
                        {
                            box.position = new Vector2(0, box.position.y);
                        }
                        time = (box.x / imageLength) * musicLength;
                    }
                }
                break;
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
