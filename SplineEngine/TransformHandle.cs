using UnityEngine;
using System.Collections.Generic;
using System;


[KSPAddon(KSPAddon.Startup.Instantly, false)]
public class TransformHandle : MonoBehaviour
{
    public delegate void OnReleaseDelegate();

    public delegate void OnDraggingDelegate();

    public delegate bool CanDrag(Vector3 GizmoPosition, Vector3 DragDeltaDelta);

    public Vector3 MoveAxis;
    public Vector3 SecondaryMoveAxis;
    public OnReleaseDelegate OnRelease;
    public OnDraggingDelegate OnDragging;
    public CanDrag CanDragPredicate;
    private Vector3 initialPosition;
    public bool snap = false;

    private TransformWidget RootWidget { get { return transform.parent.gameObject.GetComponent<TransformWidget>(); } }

    private RaycastHit lastClickHitInfo;
    private bool dragging = false;

    void Update()
    {
        if (!dragging && Input.GetMouseButtonDown(0))
        {
            RootWidget.InputCollider.gameObject.SetActive(true);
            if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out lastClickHitInfo, 1000.0f))
            {
                dragging = true;
                initialPosition = RootWidget.transform.position;
            }
        }
        else if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
            RootWidget.InputCollider.gameObject.SetActive(false);

            if (OnRelease != null)
                OnRelease.Invoke();
        }

        if (dragging)
        {

            var offsetData = RootWidget.ReferenceCamera.WorldToScreenPoint(transform.position);

            RootWidget.InputCollider.transform.position = RootWidget.ReferenceCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, offsetData.z));

            RaycastHit newHitInfo;

            RootWidget.InputCollider.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out newHitInfo, 1000.0f);

            Vector3 dragDelta = initialPosition;

            if (MoveAxis.magnitude > 0.000f)
                MoveOnAxis(initialPosition, newHitInfo.point, MoveAxis);

            if (SecondaryMoveAxis.magnitude > 0.000f)
            {
                MoveOnAxis(RootWidget.transform.position, newHitInfo.point, SecondaryMoveAxis);
            }
        }
    }

    private void MoveOnAxis(Vector3 initialPosition, Vector3 currentPosition, Vector3 moveAxis)
    {
        var dragDelta = initialPosition + Vector3.Project(currentPosition - lastClickHitInfo.point, moveAxis);

        if (CanDragPredicate == null || CanDragPredicate.Invoke(transform.position, Vector3.Project(currentPosition - lastClickHitInfo.point, moveAxis)))
        {
            if (snap)
                dragDelta = TransformWidget.Snap(dragDelta);

            RootWidget.transform.position = dragDelta;

            if (OnDragging != null)
                OnDragging.Invoke();
        }
    }
}