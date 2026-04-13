using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ArrowTrapScript : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;

    private float cooldownTimer;

    private void Attack()
    {
        cooldownTimer = 0;
        int arrowIndex = findArrow();
        arrows[arrowIndex].transform.position = firePoint.position;
        arrows[arrowIndex].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int findArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        
        if (cooldownTimer >= attackCooldown)
        {
            Attack();
        }
    }
}
