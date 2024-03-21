using UnityEngine;

public interface IDraggable
{
    void OnDragStart(Vector2 screenPos);
    void OnDrag(Vector2 screenPos);
    void OnDragEnd(Vector2 screenPos);
}
