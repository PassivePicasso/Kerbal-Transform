using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.Instantly, false)]
public class TransformWidget : MonoBehaviour
{
    private static Vector3 GLOBAL_SNAP = Vector3.one * 0.5f;

    // Snap position by using round to spec inc alg.  Given a number X, and increment M,  RoundToWholeNumber(X/M) * M
    public static Vector3 Snap(Vector3 vector, Vector3 snapValues)
    {
        return new Vector3(Mathf.RoundToInt(vector.x / snapValues.x) * snapValues.x,
                           Mathf.RoundToInt(vector.y / snapValues.y) * snapValues.y,
                           Mathf.RoundToInt(vector.z / snapValues.z) * snapValues.z);
    }

    public static Vector3 Snap(Vector3 vector)
    {
        return new Vector3(Mathf.RoundToInt(vector.x / GLOBAL_SNAP.x) * GLOBAL_SNAP.x,
                           Mathf.RoundToInt(vector.y / GLOBAL_SNAP.y) * GLOBAL_SNAP.y,
                           Mathf.RoundToInt(vector.z / GLOBAL_SNAP.z) * GLOBAL_SNAP.z);
    }
    public enum TransformMode
    {
        Translate,
        Rotate,
        Scale
    }

    public TransformMode mode;

    private GameObject inputCollider;
    public Transform InputCollider { get { return inputCollider.transform; } }

    private Camera refCamera;
    public Camera ReferenceCamera { get { return refCamera; } }

    private Transform currentTarget;
    public Transform Target
    {
        get { return currentTarget; }
        set
        {
            currentTarget = value;
            transform.position = value.position;
        }
    }

    public void Start()
    {
        if (inputCollider == null)
            inputCollider = new GameObject("translateCollider", typeof(SphereCollider));

        if (refCamera == null)
            refCamera = EditorLogic.fetch.editorCamera;
    }

    public void Update()
    {
        if (Target)
            Target.position = transform.position;
    }

    public void Destroy()
    {
        GameObject.Destroy(inputCollider);
    }
}
