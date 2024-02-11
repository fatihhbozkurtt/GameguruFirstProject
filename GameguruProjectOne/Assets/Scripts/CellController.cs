using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")]
    public GameObject crossImage;

    [Header("Debug")]
    [SerializeField] Vector2 _coordinates = Vector2.zero;

    public void SetCrossImage(bool activate)
    {
        crossImage.SetActive(activate);
    }

    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }

    public void SetCoordinates(float x, float y)
    {
        _coordinates.x = x;
        _coordinates.y = y;
    }
}
