using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private float moveSpeed = 50f;

    [SerializeField] private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        float distanceBefore = Vector3.Distance(transform.position, targetPosition);

        Vector3 moveDir = (targetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        if (distanceBefore < distanceAfter)
        {
            Instantiate(vfxHitRed, targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
}
