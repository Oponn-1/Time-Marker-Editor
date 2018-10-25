using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPlaybackMarker {

    public Rect box;

    public float time;

    public float width = 10;
    public float height = 100;

    public bool moved;

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
            case EventType.mouseDrag:
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
                    moved = true;
                }
                break;
        }
    }
}
