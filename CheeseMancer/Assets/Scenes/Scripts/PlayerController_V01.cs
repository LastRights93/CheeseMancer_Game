using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_V01 : MonoBehaviour
{

    //
    public KeyCode Forward;
    public KeyCode Backward;
    public KeyCode Left;
    public KeyCode Right;

    //
    public KeyCode RefocusKey;

    //
    public float MovementSpeed;

    //
    public float BaseCameraDistance;
    public float MaximumCameraDistanceToFloor;
    public Camera MainCamera;
    public Vector3 CameraOffset;

    //
    public bool InvertY;
    public AnimationCurve MouseSensitivy; 

    // Use this for initialization
    void Start ()
    {

        MainCamera = Camera.main;
        MainCamera.transform.position = this.transform.position - CameraOffset;
        CameraOffset = MainCamera.transform.position - this.transform.position;

    }

    void Update()
    {


        MovePlayer();


    }

    public void MovePlayer()
    {

        //
        float LeftAnalogX;
        float LeftAnalogY;

        //
        LeftAnalogX = Input.GetAxisRaw("Horizontal");
        LeftAnalogY = Input.GetAxisRaw("Vertical");

        // Detect if any key from keyboard is pressed
        if (Input.anyKey)
        {

            // Movement
            if (Input.GetKey(Left))
            {

                this.transform.position += Vector3.left * MovementSpeed * Time.deltaTime;

            }
            if (Input.GetKey(Right))
            {

                this.transform.position += Vector3.right * MovementSpeed * Time.deltaTime;

            }
            if (Input.GetKey(Forward))
            {

                this.transform.position += Vector3.forward * MovementSpeed * Time.deltaTime;

            }
            if (Input.GetKey(Backward))
            {

                this.transform.position += Vector3.back * MovementSpeed * Time.deltaTime;

            }

            Quaternion NewPlayerRotation = new Quaternion();
            // Preserve Current Rotation if no Rotation is necessary
            NewPlayerRotation = this.transform.rotation;

            // Rotation to Direction Combos
            if (Input.GetKey(Left) && Input.GetKey(Forward))
            {

                NewPlayerRotation.eulerAngles = new Vector3(0, 135.0f, 0);

            }

            if (Input.GetKey(Right) && Input.GetKey(Forward))
            {

                NewPlayerRotation.eulerAngles = new Vector3(0, 45.0f, 0);

            }

            if (Input.GetKey(Left) && Input.GetKey(Backward))
            {

                NewPlayerRotation.eulerAngles = new Vector3(0, 225.0f, 0);

            }

            if (Input.GetKey(Right) && Input.GetKey(Backward))
            {

                NewPlayerRotation.eulerAngles = new Vector3(0, 315.0f, 0);

            }

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, NewPlayerRotation, 0.1f);

        }
        //else if ()

    }

    // Update is called once per frame
    void LateUpdate ()
    {

        AdjustCamera();
        
    }

    public void AdjustCamera()
    {

        // Analog Data
        float RightAnalogX;
        float RightAnalogY;

        // Mouse Data
        float MouseX;
        float MouseY;

        //
        RightAnalogX = Input.GetAxisRaw("HorizontalRot");
        RightAnalogY = Input.GetAxisRaw("VerticalRot");

        //
        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");

        //if (Input.GetKey(RefocusKey))
        //{

        //    MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, this.transform.rotation, 0.75f);

        //}

        // Lock Cursor in place to help normalize mouse scrolling
        // and clamp to screen space
        if (Input.mousePosition.x < Screen.width ||
            Input.mousePosition.x > 0 ||
            Input.mousePosition.y < Screen.height ||
            Input.mousePosition.y > 0)
        {

            Cursor.lockState = CursorLockMode.Locked;

        }

        // If Mouse is moved do this
        if (MouseX > 0 || MouseY > 0)
        {

            Vector2 MouseMovement = new Vector2(MouseX * (MouseX < 0 ? 1 : -1), MouseY * (InvertY ? 1 : -1));
            var MouseSensitivityFactor = MouseSensitivy.Evaluate(MouseMovement.magnitude);

            Quaternion XTurnAngle = Quaternion.AngleAxis(MouseX * BaseCameraDistance, new Vector3(0, MouseMovement.x * MouseSensitivityFactor, 0));
            Quaternion YTurnAngle = Quaternion.AngleAxis(MouseY * BaseCameraDistance, Vector3.left);
            CameraOffset = XTurnAngle * YTurnAngle * CameraOffset;

        }
        // If Right analog stick is moved do this
        if (RightAnalogX > 0.1f || RightAnalogY > 0.1f)
        {

            Quaternion XTurnAngle = Quaternion.AngleAxis(RightAnalogY * BaseCameraDistance, RightAnalogY < 0 ? Vector3.up : Vector3.down);
            Quaternion YTurnAngle = Quaternion.AngleAxis(RightAnalogX * BaseCameraDistance, RightAnalogX < 0 ? Vector3.right: Vector3.left);
            CameraOffset = XTurnAngle * YTurnAngle * CameraOffset;

        }

        Vector3 CameraAdjustHeight = new Vector3();
        Ray CheckFloor = new Ray(MainCamera.transform.position, Vector3.down);
        RaycastHit FloorHit;

        if (Physics.Raycast(CheckFloor, out FloorHit, 5.0f))
        {

            Debug.Log(FloorHit.distance);
            if (FloorHit.distance < MaximumCameraDistanceToFloor)
            {

                CameraAdjustHeight = new Vector3(0, FloorHit.distance, 0);
                MainCamera.transform.position += CameraAdjustHeight;

            }

        }

        // Put all offsets and add position of orbit point
        Vector3 NewCameraPosition = this.transform.position + CameraOffset + CameraAdjustHeight;

        // Reset Camera to Player Position
        MainCamera.transform.position = Vector3.Slerp(MainCamera.transform.position, NewCameraPosition, 0.5f);
        MainCamera.transform.LookAt(this.transform);

    }

}
