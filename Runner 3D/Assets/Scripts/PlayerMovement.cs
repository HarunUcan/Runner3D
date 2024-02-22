using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private bool _isJumping;
    private bool _isCrouching;

    private float _startPosZ;

    [SerializeField] private float _slopeForce;
    [SerializeField] private float _slopeForceRayLength;

    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private float _startSpeed = 5f;
    [HideInInspector] public static float currentSpeed;

    [SerializeField] private AnimationCurve _jumpFallOff;
    [SerializeField] private float _jumpMultiplier;

    private int _oldLane = 0;
    private int _currentLane = 0; // -1: left, 0: middle, 1: right

    private CharacterController _charController;

    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _charController.enableOverlapRecovery = true;
        _startPosZ = transform.position.z;
        currentSpeed = _startSpeed;
    }

    private void Update()
    {
        CharacterMovement();
        if (currentSpeed < _maxSpeed)
            currentSpeed += Time.deltaTime / 10f;
    }
    private void LateUpdate()
    {
        // Obstacles are moving the player, so we need to keep the player in the same position on the z axis
        transform.position = transform.position.z != _startPosZ ? new Vector3(transform.position.x, transform.position.y, _startPosZ) : transform.position;

        if (_currentLane == -1 && transform.position.x < -2.5f)
            transform.position = new Vector3(-2.5f, transform.position.y, transform.position.z);

        else if (_currentLane == 1 && transform.position.x > 2.5f)
            transform.position = new Vector3(2.5f, transform.position.y, transform.position.z);

        else if (_currentLane == 0 && _oldLane == -1 && transform.position.x > 0)
            transform.position = new Vector3(0, transform.position.y, transform.position.z);

        else if (_currentLane == 0 && _oldLane == 1 && transform.position.x < 0)
            transform.position = new Vector3(0, transform.position.y, transform.position.z);

    }

    void CharacterMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) && _currentLane > -1)
        {
            _oldLane = _currentLane;
            _currentLane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) && _currentLane < 1)
        {
            _oldLane = _currentLane;
            _currentLane++;
        }
        if (_currentLane < -1)
            _currentLane = -1;

        else if (_currentLane > 1)
            _currentLane = 1;

        if (_currentLane == -1 && transform.position.x > -2.5f) // Left
            _charController.Move(Vector3.left * Time.deltaTime * 10);
        else if (_currentLane == 1 && transform.position.x < 2.5f) // Right
            _charController.Move(Vector3.right * Time.deltaTime * 10);
        else if (_currentLane == 0 && _oldLane == -1 && transform.position.x < 0) // Left to Middle
            _charController.Move(Vector3.right * Time.deltaTime * 10);
        else if (_currentLane == 0 && _oldLane == 1 && transform.position.x > 0) // Right to Middle
            _charController.Move(Vector3.left * Time.deltaTime * 10);


        if (_isJumping && !_charController.isGrounded && _isCrouching)
            _charController.Move(Vector3.down * Time.deltaTime * 12); // Dash to the ground


        if (OnSlope())
            _charController.Move(Vector3.down * _charController.height / 2 * _slopeForce * Time.deltaTime);

        if (_isCrouching)
        {
            _charController.height = 1.0f;
            _charController.center = new Vector3(0, 0.5f, 0);
            GetComponent<CapsuleCollider>().height = 1.0f;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);
        }
        else
        {
            _charController.height = 2.0f;
            _charController.center = new Vector3(0, 1.0f, 0);
            GetComponent<CapsuleCollider>().height = 1.2f;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 1.4f, 0);
        }

        JumpInput();
        CrouchingCheck();
    }
    private bool OnSlope()
    {
        if (_isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, _charController.height / 2 * _slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
                return true;
            //Debug.DrawRay(hit.point, hit.normal, Color.red);
        }

        return false;
    }

    private void JumpInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !_isJumping)
        {
            _isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        _charController.slopeLimit = 90.0f;
        GetComponent<Animator>().SetInteger("AnimState", (int)Animations.Jump);
        StartCoroutine(AnimReset());
        float timeInAir = 0.0f;
        do
        {
            float jumpForce = _jumpFallOff.Evaluate(timeInAir);
            _charController.Move(Vector3.up * jumpForce * _jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!_charController.isGrounded);

        _charController.slopeLimit = 45.0f;
        _isJumping = false;
    }

    void CrouchingCheck()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !_isCrouching)
        {
            _isCrouching = true;

            GetComponent<Animator>().SetInteger("AnimState", (int)Animations.Slide);
            StartCoroutine(AnimReset());
            StartCoroutine(StandUp());
        }
    }

    IEnumerator StandUp()
    {
        yield return new WaitForSeconds(0.8f);
        _isCrouching = false;
    }

    IEnumerator AnimReset()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<Animator>().SetInteger("AnimState", (int)Animations.Run);
    }


}
