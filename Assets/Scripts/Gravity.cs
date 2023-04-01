using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
    private const float GravityConstant = 1.0f;
    private static readonly List<Gravity> Others = new();

    [SerializeField]
    private Vector3 InitialVelocity = Vector3.zero;
    [SerializeField]
    private Vector3 InitialRotation = Vector3.zero;
    [SerializeField]
    private Vector3 OrbitUp = Vector3.up;
    [SerializeField]
    private Gravity OrbitObject = null;
    [SerializeField]
    private bool OrbitClockwise = false;
    [SerializeField]
    private bool CalculateVelocity = false;
    private Rigidbody _rb = null;
    private Rigidbody rb
    {
        get
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();
            return _rb;
        }
    }
    private bool OrbitComputed = false;

    private void Awake()
    {
        rb.useGravity = false;
        rb.velocity = InitialVelocity;
        rb.angularVelocity = InitialRotation;
        OrbitComputed = OrbitObject == null;
    }

    private void OnEnable()
    {
        Others.Add(this);
    }

    private void OnDisable()
    {
        Others.Remove(this);
    }

    private void FixedUpdate()
    {
        if (CalculateVelocity && !OrbitComputed && OrbitObject != null && OrbitObject.OrbitComputed)
        {
            rb.velocity += OrbitObject.rb.velocity;

            Vector3 thisToOrbitObject = rb.position - OrbitObject.rb.position;
            Vector3 orbitDirection = Vector3.Cross(thisToOrbitObject.normalized, OrbitUp.normalized);
            if (OrbitClockwise)
                orbitDirection = -orbitDirection;
            rb.velocity += orbitDirection * Mathf.Sqrt(GravityConstant * OrbitObject.rb.mass / thisToOrbitObject.magnitude);

            OrbitComputed = true;
        }

        foreach (var other in Others)
        {
            if (this != other)
            {
                Vector3 thisToOther = rb.position - other.rb.position;
                float gravityforce = GravityConstant * other.rb.mass / thisToOther.sqrMagnitude;
                rb.AddForce(-thisToOther.normalized * gravityforce, ForceMode.Acceleration);
            }
        }
    }
}
