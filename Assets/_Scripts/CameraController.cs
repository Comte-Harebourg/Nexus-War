using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [Header("Move Settings")]
    public float moveSpeed = 20f;
    public float dragSpeed = 1f;
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);
    private float zoom;
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    private Camera cam;
    private Vector3 dragOrigin;
    private float baseMoveSpeed = 20f;

     private void UpdateMoveSpeed()
    {
        zoom = Camera.main.orthographicSize;
        moveSpeed = baseMoveSpeed * zoom / 10f;
    } 

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
        zoom = Camera.main.orthographicSize;
        baseMoveSpeed = moveSpeed;
        UpdateMoveSpeed();
    }

    void Update()
    {
        HandleKeyboardMove();
        HandleMouseDrag();
        HandleMouseMove();
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
    [Header("Mouse Movement Settings")]
    public float insideScreenThreshold = 20f; // seuil à l'intérieur du bord de l'écran pour que la souris affecte la caméra (avant nommé EdgeSize)
    public float outsideScreenThreshold = 10f; // seuil à l'extérieur du bord de l'écran pour que la souris affecte la caméra. Lorsqu'elle vaut -1, on autorise le mouvement de la caméra quand la souris est dehors
    void HandleMouseMove()
    {
        Vector3 pos = transform.position;
        Vector3 mousePos = Input.mousePosition;
        bool allowMovement = false;

        if (outsideScreenThreshold == -1)
        {
        // Lorsque outsideScreenThreshold vaut -1, on autorise le mouvement de la caméra quand la souris est dehors
            allowMovement = true;
        }
            else
        {
            // On autorise le mouvement si la souris est dans l'écran ou dans le seuil extérieur
            allowMovement =
                mousePos.x >= -outsideScreenThreshold &&
                mousePos.x <= Screen.width + outsideScreenThreshold &&
                mousePos.y >= -outsideScreenThreshold &&
                mousePos.y <= Screen.height + outsideScreenThreshold;
        }

        if (allowMovement)
        {
        if (mousePos.x < insideScreenThreshold)
            pos.x -= moveSpeed * Time.deltaTime;
        else if (mousePos.x > Screen.width - insideScreenThreshold)
            pos.x += moveSpeed * Time.deltaTime;

        if (mousePos.y < insideScreenThreshold)
            pos.y -= moveSpeed * Time.deltaTime;
        else if (mousePos.y > Screen.height - insideScreenThreshold)
            pos.y += moveSpeed * Time.deltaTime;
        transform.position = pos;
        }
    }

    //Zoom caméra
    [Header("Zoom Settings")]
    public bool zoomToMouse = true;

    void HandleZoom()
    {
        // On accepte le scroll sur les axes X ou Y (important pour Shift+Scroll sur Mac/Linux qui renvoie un scroll horizontal)
        Vector2 scrollDelta = Input.mouseScrollDelta;
        float scroll = scrollDelta.y != 0 ? scrollDelta.y : scrollDelta.x;

        if (scroll == 0f)
            return;
        // Lorsqu'on appuie sur shift, la caméra ne se centre plus sur la souris mais sur le centre de l'écran (appuyer sur shift ignore zoomToMouse)
        bool effectiveZoomToMouse = zoomToMouse && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift);

        // La position de la souris AVANT le zoom
        Vector3 preZoomWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // on applique le zoom
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        // On zoome normalement si shift est appuié ou si zoomToMouse est désactivé
        if (!effectiveZoomToMouse)
        {
            UpdateMoveSpeed();
            return;
        }

        // La position de la souris APRES le zoom
        Vector3 postZoomWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // On calcule le décalage de la position d'avant et après le zoom pour garder la souris au meme endroit
        Vector3 offset = preZoomWorldPos - postZoomWorldPos;

        // On ajoute le décalage à la position de la caméra
        transform.position += offset;

        UpdateMoveSpeed();
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