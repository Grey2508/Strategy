using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Camera RaycastCamera;

    public float MinZoom = -12;
    public float MaxZoom = 7;

    private Vector3 _startPoint;
    private Vector3 _cameraStartPosition;
    private Plane _plane;
    private float _currentZoom;
    private float _oldZoom;

    void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition);

        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 point = ray.GetPoint(distance);

        if (Input.GetMouseButtonDown(2))
        {
            _startPoint = point;
            _cameraStartPosition = transform.position;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 offset = point - _startPoint;
            transform.position = _cameraStartPosition - offset;
        }

        if(Input.GetMouseButtonUp(2))
            RaycastCamera.transform.position = transform.position;

        Zoom();
    }

    private void Zoom()
    {
        _currentZoom = Mathf.Clamp(_currentZoom + Input.mouseScrollDelta.y, MinZoom, MaxZoom);
        float delta = _currentZoom - _oldZoom;
        _oldZoom = _currentZoom;

        transform.Translate(0, 0, delta);
        RaycastCamera.transform.Translate(0, 0, delta);
    }
}
