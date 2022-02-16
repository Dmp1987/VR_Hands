using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //Animation Variables
    [SerializeField]private float animationSpeed;
    Animator _animator;
    private SkinnedMeshRenderer _mesh;
    private float _triggerTarget;
    private float _gripTarget;
    private float _triggerCurrent;
    private float _gripCurrent;    
    private const string animatorGripParam = "Grip";
    private const string animatorTriggerParam = "Trigger";
    private static readonly int Grip = Animator.StringToHash(animatorGripParam);
    private static readonly int Trigger = Animator.StringToHash(animatorTriggerParam);


    //PHysics Move
    [SerializeField] private GameObject followObject;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    private Transform _followTarget;
    private Rigidbody _body;

    // Start is called before the first frame update
    void Start()
    {
        //Animation
        _animator = GetComponent<Animator>();
        _mesh = GetComponent<SkinnedMeshRenderer>();

        //Physics
        _followTarget = followObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;

        //teleport hands
        _body.position = _followTarget.position;
        _body.rotation = _followTarget.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        animateHand();
        //
        physicsMove();
    }

    private void physicsMove()
    {
        //position
        var positionWithOffset = _followTarget.position + positionOffset;
        var distance = Vector3.Distance(_followTarget.position, transform.position);
        _body.velocity = (_followTarget.position - transform.position).normalized * (followSpeed * distance);

        //rotation
        var rotationWithOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(_body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        _body.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }

    internal void setGrip(float v)
    {
        _gripTarget = v;
    }

    internal void setTrigger(float v)
    {
        _triggerTarget = v;
    }

    void animateHand() 
    {
        if (_gripCurrent != _gripTarget)
        {
            _gripCurrent = Mathf.MoveTowards(_gripCurrent, _gripTarget, Time.deltaTime * animationSpeed);
            _animator.SetFloat(animatorGripParam, _gripCurrent);
        }
        
        if (_triggerCurrent != _triggerTarget)
        {
            _triggerCurrent = Mathf.MoveTowards(_triggerCurrent, _triggerTarget, Time.deltaTime * animationSpeed);
            _animator.SetFloat(animatorTriggerParam, _triggerCurrent);
        }
    }
}
