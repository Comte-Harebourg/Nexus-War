using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [Header("Move Settings")]
    public float moveSpeed = 20f;
    public float dragSpeed = 1f;
    public float edgeSize = 20f; // pixels du bord écran
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);
    private float zoom;
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    private Camera cam;
    private Vector3 dragOrigin;

     private void UpdateMoveSpeed()
    {
        //MoveSpeed * zoom / 10
    } 

    void Start()
    {
        Instance = this;
        cam = Camera.main;
        zoom = Camera.main.orthographicSize;
        UpdateMoveSpeed();
    }

    void Update()
    {
        HandleKeyboardMove();
        HandleMouseDrag();
        HandleEdgeMove();
        HandleZoom();
        ClampPosition();
    }

    //Déplacement ZQSD
    void HandleKeyboardMove()
    {
        float h = Input.GetAxisRaw("Horizontal"); // Q/D ou flèches
        float v = Input.GetAxisRaw("Vertical");   // Z/S ou flèches
        Vector3 dir = new Vector3(h, v, 0).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    //Déplacement par Drag
    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += diff * dragSpeed;
        }
    }

    //Déplacement aux bords de l’écran
    void HandleEdgeMove()
    {
        Vector3 pos = transform.position;
        if (Input.mousePosition.x < edgeSize)
            pos.x -= moveSpeed * Time.deltaTime;
        else if (Input.mousePosition.x > Screen.width - edgeSize)
            pos.x += moveSpeed * Time.deltaTime;
        if (Input.mousePosition.y < edgeSize)
            pos.y -= moveSpeed * Time.deltaTime;
        else if (Input.mousePosition.y > Screen.height - edgeSize)
            pos.y += moveSpeed * Time.deltaTime;
        transform.position = pos;
    }

    //Zoom caméra
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            UpdateMoveSpeed();
        }
    }

    //Empêcher la caméra de sortir de la map
    void ClampPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
            transform.position.z
        );
    }

    public void SetCameraBounds(ScriptableLevel level)
    {
        int minX = level.GroundTiles.Min(t => t.Position.x);
        int maxX = level.GroundTiles.Max(t => t.Position.x);
        int minY = level.GroundTiles.Min(t => t.Position.y);
        int maxY = level.GroundTiles.Max(t => t.Position.y);
        minBounds = new Vector2(minX, minY);
        maxBounds = new Vector2(maxX+1, maxY+1);
    }
}