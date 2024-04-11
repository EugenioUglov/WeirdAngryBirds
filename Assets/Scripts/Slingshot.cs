using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private LineRenderer[] _lineRenderers; 
    [SerializeField] private Transform[] _stripPositions;
    [SerializeField] private Transform _center;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Vector3 _currentPosition;
    [SerializeField] private float _maxLength;
    [SerializeField] private float _bottomBoundary;
    [SerializeField] private GameObject _birdPrefab;
    [SerializeField] private float _birdPossitionOffset;
    [SerializeField] private float _force;

    private Rigidbody2D _bird;
    private Collider2D _birdCollider;
    private bool _isMousedDown = false;


    void Start()
    {
        _lineRenderers[0].positionCount = 2;
        _lineRenderers[1].positionCount = 2;
        _lineRenderers[0].SetPosition(0, _stripPositions[0].position);
        _lineRenderers[1].SetPosition(0, _stripPositions[1].position);

        CreateBird();
    }

    void Update()
    {
        if (_isMousedDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            _currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _currentPosition = _center.position + Vector3.ClampMagnitude(_currentPosition - _center.position, _maxLength);
            _currentPosition = ClampBoundary(_currentPosition);
            SetStrips(_currentPosition);

            if (_birdCollider)
            {
                _birdCollider.enabled = true;
            }
        }
        else
        {
            ResetStrips();
        }
    }


    private void CreateBird()
    {
        _bird = Instantiate(_birdPrefab).GetComponent<Rigidbody2D>();
        _birdCollider = _bird.GetComponent<Collider2D>();
        _birdCollider.enabled = false;

        _bird.isKinematic = true;

        ResetStrips();
    }

    private Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, _bottomBoundary, 1000);

        return vector;
    }

    void OnMouseDown()
    {
        _isMousedDown = true;
    }

    void OnMouseUp()
    {
        _isMousedDown = false;
        Shoot();
    }

    void ResetStrips()
    {
        _currentPosition = _idlePosition.position;
        SetStrips(_idlePosition.position);
    }

    void Shoot()
    {
        _bird.isKinematic = false;
        Vector3 birdForce = (_currentPosition - _center.position) * _force * -1;
        _bird.velocity = birdForce;

        _bird = null;
        _birdCollider = null;
        Invoke("CreateBird", 2);

    }

    void SetStrips(Vector3 position)
    {
        _lineRenderers[0].SetPosition(1, position);
        _lineRenderers[1].SetPosition(1, position);

        if (_bird)
        {
            Vector3 dir = position - _center.position;
            _bird.transform.position = position + dir.normalized * _birdPossitionOffset;
            _bird.transform.right = -dir.normalized;
        }
    }
}
