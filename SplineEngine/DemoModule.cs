using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DemoModule : PartModule
{
    private TransformWidget widget;
    private Transform topEdge;
    private static Shader SelfIllum = Shader.Find("Self-Illumin/Diffuse");
    private float radius = 1.0f;
    private MeshCollider meshCollider;

    public override void OnStart(PartModule.StartState state)
    {
        if (state == StartState.Editor)
        {
            part.OnEditorAttach = new Callback(CreateTransform);
            part.OnEditorDetach = new Callback(DestroyTransform);
            meshCollider = part.gameObject.AddComponent<MeshCollider>();
        }
    }

    private void DestroyTransform()
    {
        GameObject.Destroy(widget.gameObject);
    }

    private void CreateTransform()
    {
        topEdge = new GameObject().transform;
        topEdge.localScale = new Vector3(1.0f, 0.05f, 1.0f);
        topEdge.up = -1 * transform.right;
        topEdge.parent = transform;
        widget = CreateTransformWidget(topEdge);
        widget.transform.position = transform.position + (topEdge.up * 0.25f);
    }

    private TransformWidget CreateTransformWidget(Transform target)
    {
        TransformWidget widget = new GameObject("TransformWidget", typeof(TransformWidget)).GetComponent<TransformWidget>();

        var widgetTransform = widget.transform;
        widget.Target = target;
        widgetTransform.position = target.position;

        CreateHandle(widgetTransform, widget.Target.up, new Vector3(0.15f, 0.1f, 0.45f), Color.blue);

        var handleComponent = CreateHandle(widgetTransform, widget.Target.up, new Vector3(0.25f, 0.1f, 0.25f), Color.green);
        handleComponent.SecondaryMoveAxis = Vector3.right;
        handleComponent.transform.localPosition = (handleComponent.MoveAxis * 1.1f) + (handleComponent.SecondaryMoveAxis * 1.1f);

        return widget;
    }

    private TransformHandle CreateHandle(Transform parent, Vector3 axis, Vector3 gizmoSize, Color color)
    {
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);

        handle.transform.parent = parent;
        handle.transform.localPosition = Vector3.zero;
        handle.transform.forward = axis;
        handle.transform.localScale = gizmoSize;

        var handleComponent = handle.AddComponent<TransformHandle>();
        handleComponent.MoveAxis = axis;

        var handleRenderer = handle.GetComponent<MeshRenderer>();
        handleRenderer.material = new Material(SelfIllum);
        handleRenderer.material.color = color;

        handleComponent.OnDragging += DrawMesh;

        return handleComponent;
    }

    private void ScaleMesh() { transform.localScale = new Vector3(Vector3.Distance(transform.position, widget.transform.position), transform.localScale.y, transform.localScale.z); }
}
