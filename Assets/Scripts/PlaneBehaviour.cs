using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    public Camera cam;
    public GameObject brokenPlane, canvas;
    public Joystick joystick; // Reference to the Fixed Joystick
    private Rigidbody2D rb;
    public float baseSpeed = 5f, maxSpeed = 10f, acceleration = 2f; // Speed properties
    private float currentSpeed;
    private Vector2 currentDirection = Vector2.up; // Default forward direction
    private bool isGameOver;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed; // Initialize with base speed
    }

    void FixedUpdate()
    {
        if (!isGameOver)
        {
            ControlPlane(); // Control the plane based on joystick input
            MovePlane();    // Keep the plane moving
        }
    }

    void ControlPlane()
    {
        // Get joystick input
        float horizontalInput = joystick.Horizontal; // Left (-1) to Right (+1)
        float verticalInput = joystick.Vertical;     // Down (-1) to Up (+1)

        // If there is joystick input, update the movement direction
        if (horizontalInput != 0 || verticalInput != 0)
        {
            currentDirection = new Vector2(horizontalInput, verticalInput).normalized; // Update direction
        }

        // Adjust speed based on joystick magnitude
        float joystickMagnitude = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude); // Clamp to [0, 1]
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, joystickMagnitude); // Interpolate speed between base and max
    }

    void MovePlane()
    {
        // Set the velocity based on the current direction and speed
        rb.velocity = currentDirection * currentSpeed;

        // Rotate the plane to face the current movement direction
        if (currentDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90; // Adjusting to match the plane's forward direction
        }
    }

    public void gameOver(Transform missile)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        isGameOver = true;
        rb.velocity = Vector2.zero;
        cam.gameObject.GetComponent<CameraBehaviour>().gameOver = true;

        GameObject planeTemp = Instantiate(brokenPlane, transform.position, transform.rotation);
        for (int i = 0; i < planeTemp.transform.childCount; i++)
        {
            Rigidbody2D rbTemp = planeTemp.transform.GetChild(i).gameObject.GetComponent<Rigidbody2D>();
            rbTemp.AddForce(((Vector2)missile.position - rbTemp.position) * -5f, ForceMode2D.Impulse);
        }

        StartCoroutine(canvasStuff());
    }

    IEnumerator canvasStuff()
    {
        yield return new WaitForSeconds(1f);
        canvas.SetActive(true);
        for (int i = 0; i <= 10; i++)
        {
            float k = (float)i / 10;
            canvas.transform.localScale = new Vector3(k, k, k);
            yield return new WaitForSeconds(.01f);
        }
        Destroy(gameObject);
    }
}
