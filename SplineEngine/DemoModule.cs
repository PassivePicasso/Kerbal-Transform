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

        CreateHandle(widgetTransform, widget.Target.up, new Color(255, 162, 63));

        return widget;
    }

    private Transform CreateHandle(Transform parent, Vector3 axis, Color color)
    {
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);

        handle.transform.parent = parent;
        handle.transform.localPosition = Vector3.zero;
        handle.transform.forward = axis;
        handle.transform.localScale = new Vector3(0.1f, 0.1f, 0.25f);

        var handleComponent = handle.AddComponent<TransformHandle>();
        handleComponent.moveAxis = axis;

        var handleRenderer = handle.GetComponent<MeshRenderer>();
        handleRenderer.material = new Material(Shader.Find("Diffuse"));
        handleRenderer.material.color = color;

        handleComponent.OnDragging += ScaleMesh;

        return handle.transform;
    }

    private void ScaleMesh() { transform.localScale = new Vector3(Vector3.Distance(transform.position, widget.transform.position), transform.localScale.y, transform.localScale.z); }
}
