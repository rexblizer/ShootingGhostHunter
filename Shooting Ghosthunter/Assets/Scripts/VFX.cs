using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private Vector3 VFXDir;

    public void Setup(Vector3 VfxDir)
    {
        this.VFXDir = VfxDir;
    }
    void Update()
    {
        transform.LookAt(VFXDir);
        Destroy(gameObject, destroyTime);
    }
}