using TMPro;
using UnityEngine;
using static StaticData;

public class CameraController : MonoBehaviour
{
    private float limitRight;
    private float limitLeft;
    private float limitTop;
    private float limitBottom;

    public float minSize;
    public float maxSize;

    public float speed;
    public float sizeSpeed;

    public float borderOffset;

    private Vector2 screenSize;
    private Camera cam;
    private Transform camTransform;

    private Vector2 delta;
    float? distance;
    float screenWidth;
    int maxDeltaMousePos = 125;
    Vector2? startMousePos;
    Vector2 newMousePos;

    private void Start()
    {
        cam = Camera.main;
        camTransform = transform;
        screenWidth = Screen.width;
        screenSize = new Vector2(cam.scaledPixelWidth, cam.scaledPixelHeight);
        delta = (cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 0)) - cam.ScreenToWorldPoint(Vector3.zero)) / 2;

        limitTop = MapManagerStatic.lastCoord.y - 1.5f;
        limitBottom = MapManagerStatic.firstCoord.y + 0.5f;
        limitRight = MapManagerStatic.lastCoord.x - 1.5f;
        limitLeft = MapManagerStatic.firstCoord.x + 0.5f;

        Vector3 pos = new Vector3(MapManagerStatic.lastCoord.x - MapManagerStatic.firstCoord.x, MapManagerStatic.lastCoord.y - MapManagerStatic.firstCoord.y, 0) / 2;
        pos += new Vector3(MapManagerStatic.firstCoord.x - 0.5f, MapManagerStatic.firstCoord.y - 0.5f, -10);
        camTransform.position = pos;

        float maxSizeX = cam.orthographicSize * (limitTop - limitBottom) / delta.x / 2;
        float maxSizeY = cam.orthographicSize * (limitRight - limitLeft) / delta.y / 2;
        maxSize = Mathf.Min(maxSizeX, maxSizeY);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            var newSize = Mathf.Clamp(cam.orthographicSize - sizeSpeed * Input.GetAxis("Mouse ScrollWheel"), minSize, maxSize);
            var isIncrement = newSize > cam.orthographicSize;
            cam.orthographicSize = newSize;
            if (isIncrement)
            {
                var v0 = camTransform.position;
                var v1 = cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 0));
                var v2 = cam.ScreenToWorldPoint(Vector3.zero);
                if (v1.y > limitTop)
                    v0 -= new Vector3(0, v1.y - limitTop);

                if (v2.y < limitBottom)
                    v0 += new Vector3(0, limitBottom - v2.y);


                if (v1.x > limitRight)
                    v0 -= new Vector3(v1.x - limitRight, 0);

                if (v2.x < limitLeft)
                    v0 += new Vector3(limitLeft - v2.x, 0);

                camTransform.position = v0;
            }

            delta = (cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 0)) - cam.ScreenToWorldPoint(Vector3.zero)) / 2;
        }

        var k = Time.deltaTime * 40;

        if (Input.GetKey(KeyCode.W))
            if (cam.ScreenToWorldPoint(new Vector3(0, screenSize.y, 0)).y < limitTop)
                cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Clamp(cam.transform.position.y + k * speed * (cam.orthographicSize / maxSize), limitBottom + delta.y, limitTop - delta.y), -10);

        if (Input.GetKey(KeyCode.S))
            if (cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y > limitBottom)
                cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Clamp(cam.transform.position.y - k * speed * (cam.orthographicSize / maxSize), limitBottom + delta.y, limitTop - delta.y), -10);

        if (Input.GetKey(KeyCode.D))
            if (cam.ScreenToWorldPoint(new Vector3(screenSize.x, 0, 0)).x < limitRight)
                cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x + k * speed * (cam.orthographicSize / maxSize), limitLeft + delta.x, limitRight - delta.x), cam.transform.position.y, -10);

        if (Input.GetKey(KeyCode.A))
            if (cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x > limitLeft)
                cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x - k * speed * (cam.orthographicSize / maxSize), limitLeft + delta.x, limitRight - delta.x), cam.transform.position.y, -10);
#endif
        if (!Cursor.IsMoveBuilding && !Cursor.IsOverUIElement())
        {
            if (Input.touches.Length >= 1)
            {
                newMousePos = Input.mousePosition;
                if (startMousePos != null && maxDeltaMousePos > Vector2.Distance(newMousePos, (Vector2)startMousePos))
                {
                    var newCamPos = camTransform.position + cam.ScreenToWorldPoint((Vector3)startMousePos) - cam.ScreenToWorldPoint(newMousePos);
                    float x = Mathf.Clamp(newCamPos.x, limitLeft + delta.x, limitRight - delta.x);
                    float y = Mathf.Clamp(newCamPos.y, limitBottom + delta.y, limitTop - delta.y);
                    camTransform.position = new Vector3(x, y, camTransform.position.z);
                    if (newMousePos != startMousePos)
                        Cursor.IsCameraMoving = true;
                }

                startMousePos = newMousePos;
            }
        }

        if (startMousePos != null && Input.touches.Length == 0)
        {
            startMousePos = null;
            Cursor.IsCameraMoving = false;
        }

        if (Input.touches.Length == 2)
        {
            Vector2 touch0, touch1;
            float newDistance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            newDistance = Vector2.Distance(touch0, touch1);
            if (distance != null)
            {
                var newSize = cam.orthographicSize - (maxSize - minSize) * (newDistance - (float)distance) / screenWidth;
                newSize = Mathf.Clamp(newSize, minSize, maxSize);
                cam.orthographicSize = newSize;
                if (newDistance < distance)
                {
                    var v0 = camTransform.position;
                    var v1 = cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 0));
                    var v2 = cam.ScreenToWorldPoint(Vector3.zero);
                    if (v1.y > limitTop)
                        v0 -= new Vector3(0, v1.y - limitTop);

                    if (v2.y < limitBottom)
                        v0 += new Vector3(0, limitBottom - v2.y);


                    if (v1.x > limitRight)
                        v0 -= new Vector3(v1.x - limitRight, 0);

                    if (v2.x < limitLeft)
                        v0 += new Vector3(limitLeft - v2.x, 0);

                    camTransform.position = v0;
                    Cursor.IsCameraMoving = true;
                }
                delta = (cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, 0)) - cam.ScreenToWorldPoint(Vector3.zero)) / 2;
            }

            distance = newDistance;
        }
        else if (distance != null)
        {
            distance = null;
            Cursor.IsCameraMoving = false;
        }
    }
}
