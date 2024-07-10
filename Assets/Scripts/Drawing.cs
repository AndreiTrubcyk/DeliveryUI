using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

public class Drawing : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineDrawer;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _rope;
    [SerializeField] private GameObject _finish;
    [SerializeField] private BoxController _boxController;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private GameObject _win;
    [SerializeField] private GameObject _loose;
    private float _deepZ = 12.87999f;
    private float _maxDistanceBetweenFinishAndBox = 0.1f;
    private bool _isPoint;
    private Vector3 _lastPointInLineRenderer;
    private bool _isDrawed = true;

    private void Awake()
    {
        _lastPointInLineRenderer = _lineDrawer.GetPosition(_lineDrawer.positionCount - 1);
    }


    private void Update()
    {
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _deepZ);
        var drawingPoint = _camera.ScreenToWorldPoint(mousePosition);
        if (Input.GetMouseButton(0))
        {
            if (!_isDrawed)
            {
                return;
            }
            if (Mathf.Abs(_lastPointInLineRenderer.x - drawingPoint.x) > 0.5f)
            {
                _lineDrawer.positionCount++;
                _lineDrawer.SetPosition(_lineDrawer.positionCount - 1, drawingPoint);
                _lastPointInLineRenderer = drawingPoint;
            }
        }

        // если isPoint = false и при этом у ящика _ropeAttachment.enabled = false это значит что ящек отцепился раньше чем доехала верёвка то можно будет вызывать экран с перезапуском
        // и делать reloar scene. (написать отдельный script для UI)!!!!!!!!!!!!!!!!!! тут проверять и сделать событие когда ящик отцеплен но не доехал то событие будет тригерить метод из UI
        // кторый будет затемнять всё и включать выбор перезапуска, а при нажатии на кнопку перезапуска будет выбываться событие по кнопке что бы reload scene. Для reload тоже отдельный скрипт.
        if (Input.GetMouseButtonUp(0) && _isDrawed)
        {
            StartCoroutine(Moving());
            _isDrawed = false;
        }
        
        if (Mathf.Abs(_finish.transform.position.x - _boxController.transform.position.x) < _maxDistanceBetweenFinishAndBox && !_isPoint)
        {
            _boxController.DropDown(_finish.transform.position);
            _isPoint = true;
        }

        if (_boxController.transform.position == _finish.transform.position)
        {
            _win.SetActive(true);
        }

        if (!_boxController.BoxState() && !_isPoint)
        {
            _loose.SetActive(true);
        }
    }

    private IEnumerator Moving()
    {
        for (int i = 0; i < _lineDrawer.positionCount - 1; i++)
        {
            var position = _lineDrawer.GetPosition(i);
            if (_isPoint)
            {
                _rope.transform.position = new Vector3(_finish.transform.position.x,_rope.transform.position.y,_rope.transform.position.z);
                yield break;
            }
            while (_rope.transform.position != position)
            {
                _rope.transform.position = Vector3.MoveTowards(_rope.transform.position, position, _speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
