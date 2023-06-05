using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public float ShowDistance = 20f;
    [Tooltip("Defines your own update rate for the alignment. For moving objects, use delta time or a number lower than fixed update for smooth results (.02f).")]
    public float UpdateRate = .02f;
    [Tooltip("If enabled, will ignore UpdateRate and default to deltaTime.")]
    public bool UseDeltaTime = false;
    [Tooltip("Use 0 in the X rotation for alignment. Might be useful for different camera placements.")]
    public bool ZeroLookRotX = true;
    [Tooltip("Use 0 in the Z rotation for alignment.Might be useful for different camera placements.")]
    public bool ZeroLookRotY = true;

    public float ZOffset = 0;
    public float XOffset = 0;
    public float YOffset = 1;

    protected Camera mainCamera = null;
    protected bool hasdmg = false;

    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;    
    }

    // Update is called once per frame
    void Update()
    {
        AlignToCamera();
    }

    protected virtual void AlignToCamera()
    {
        if (mainCamera == null) return;//can't align without camera
        transform.localPosition = new Vector3(XOffset, YOffset, ZOffset);
        Transform camXform = mainCamera.transform;
        Vector3 forward = transform.position - camXform.position;


        if (ZeroLookRotX)
        {
            forward.x = 0;
        }
        if (ZeroLookRotY)
        {
            forward.y = 0;

        }
        forward.Normalize();
        Vector3 up = Vector3.Cross(forward, camXform.right);
        transform.rotation = Quaternion.LookRotation(forward, up);

    }
}
