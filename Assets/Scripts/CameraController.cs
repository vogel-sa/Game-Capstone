using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private enum Direction
    {
        FRONT, BACK, LEFT, RIGHT
    }

    [SerializeField]
    private float dist;

    [SerializeField]
    private float cameraAngle;
    [SerializeField]
    private float rotationSpeed;

    private bool isRotating;

    [SerializeField]
    private Direction direction;



    // Use this for initialization
    void Start()
    {
        direction = Direction.FRONT;
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            HandleRightKey();
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            HandleLeftKey();
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            HandleDownKey();
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            HandleUpKey();
        }

        //Rotations
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            StartCoroutine(RotateMe(Vector3.up * -90, rotationSpeed, -90f));
            rotateCounterClockwise();
        }
        if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            StartCoroutine(RotateMe(Vector3.up * 90, rotationSpeed, 90f));
            rotateClockwise();
        }
    }

    IEnumerator RotateMe(Vector3 byAngles, float inTime, float degrees)
    {
        isRotating = true;
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t <= 1f; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }

        transform.rotation = fromAngle;
        transform.Rotate(0, degrees, 0);
        isRotating = false;
    }



    private void HandleRightKey()
    {
        switch (direction)
        {
            case Direction.FRONT:
                IncreaseX();
                break;
            case Direction.LEFT:
                DecreaseZ();
                break;
            case Direction.BACK:
                DecreaseX();
                break;
            case Direction.RIGHT:
                IncreaseZ();
                break;
        }

    }
    private void HandleLeftKey()
    {
        switch (direction)
        {
            case Direction.FRONT:
                DecreaseX();
                break;
            case Direction.LEFT:
                IncreaseZ();
                break;
            case Direction.BACK:
                IncreaseX();
                break;
            case Direction.RIGHT:
                DecreaseZ();
                break;
        }
    }
    private void HandleUpKey()
    {
        switch (direction)
        {
            case Direction.FRONT:
                IncreaseZ();
                break;
            case Direction.LEFT:
                IncreaseX();
                break;
            case Direction.BACK:
                DecreaseZ();
                break;
            case Direction.RIGHT:
                DecreaseX();
                break;
        }
    }
    private void HandleDownKey()
    {
        switch (direction)
        {
            case Direction.FRONT:
                DecreaseZ();
                break;
            case Direction.LEFT:
                DecreaseX();
                break;
            case Direction.BACK:
                IncreaseZ();
                break;
            case Direction.RIGHT:
                IncreaseX();
                break;
        }
    }

    private void IncreaseX()
    {
        transform.position = new Vector3(transform.position.x + dist, transform.position.y, transform.position.z);

    }
    private void DecreaseX()
    {
        transform.position = new Vector3(transform.position.x - dist, transform.position.y, transform.position.z);
    }
    private void IncreaseZ()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + dist);
    }
    private void DecreaseZ()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - dist);
    }


    private void rotateCounterClockwise()
    {
        switch (direction)
        {
            case Direction.FRONT:
                direction = Direction.RIGHT;
                break;
            case Direction.LEFT:
                direction = Direction.FRONT;
                break;
            case Direction.BACK:
                direction = Direction.LEFT;
                break;
            case Direction.RIGHT:
                direction = Direction.BACK;
                break;
        }

    }

    private void rotateClockwise()
    {
        switch (direction)
        {
            case Direction.FRONT:
                direction = Direction.LEFT;
                break;
            case Direction.LEFT:
                direction = Direction.BACK;
                break;
            case Direction.BACK:
                direction = Direction.RIGHT;
                break;
            case Direction.RIGHT:
                direction = Direction.FRONT;
                break;
        }

    }
}
