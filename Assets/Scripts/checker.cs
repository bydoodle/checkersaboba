using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

public class checker : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera;
    private bool checkQueen = false;
    public bool isActive = false;
    Vector3 checkPos;
    Vector3 checkEnemyPos;
    int clickCount = 0;

    private void Start()
    {
        gameObject.tag = "red";
    }

    private void OnMouseDown()
    {
        isActive = true;
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        clickCount++;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit) && isActive)
        {
            if (Input.GetMouseButtonDown(0)) clickCount++;

            if (raycastHit.point.x < transform.position.x - .5 && raycastHit.point.x > transform.position.x - 1.5 && raycastHit.point.z > transform.position.z + .5 && raycastHit.point.z < transform.position.z + 1.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1);
                if (clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    transform.position = new Vector3(transform.position.x - 1, 0f, transform.position.z + 1);
                    isActive = false;
                    clickCount = 0;
                }
                else if (Physics.CheckSphere(checkPos, .9f) && clickCount > 2)
                {
                    StartPositionRPC();
                }
            }
            else if (raycastHit.point.x > transform.position.x + .5 && raycastHit.point.x < transform.position.x + 1.5 && raycastHit.point.z > transform.position.z + .5 && raycastHit.point.z < transform.position.z + 1.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
                if (clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    transform.position = new Vector3(transform.position.x + 1, 0f, transform.position.z + 1);
                    isActive = false;
                    clickCount = 0;
                }
                else if (Physics.CheckSphere(checkPos, .9f) && clickCount > 2)
                {
                    StartPositionRPC();
                }
            }
            else if (raycastHit.point.x > transform.position.x + 1.5 && raycastHit.point.x < transform.position.x + 2.5 && raycastHit.point.z > transform.position.z + 1.5 && raycastHit.point.z < transform.position.z + 2.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z + 2);
                checkEnemyPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0 && clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            transform.position = new Vector3(transform.position.x + 2, 0f, transform.position.z + 2);
                            isActive = false;
                            clickCount = 0;
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                else if (clickCount > 2)
                {
                    StartPositionRPC();
                }
            }
            else if (raycastHit.point.x < transform.position.x - 1.5 && raycastHit.point.x > transform.position.x - 2.5 && raycastHit.point.z > transform.position.z + 1.5 && raycastHit.point.z < transform.position.z + 2.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z + 2);
                checkEnemyPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0 && clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            transform.position = new Vector3(transform.position.x - 2, 0f, transform.position.z + 2);
                            isActive = false;
                            clickCount = 0;
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                else if (clickCount > 2)
                {
                    StartPositionRPC();
                }
            }
            else if (clickCount > 2 && checkQueen)
            {
                QueenMovementRPC(raycastHit.point.x, raycastHit.point.z);
            }
            else if (clickCount > 2)
            {
                StartPositionRPC();
            }
        }

        if (transform.position.z > 4)
        {
            checkQueen = true;
            Debug.Log("queen");
        }
    }

    Collider[] EnemyInRange(Vector3 enemyPosition, float range, int layer)
    {
        int layerMask = 3 << layer;
        Collider[] hitColliders = Physics.OverlapSphere(enemyPosition, range, layerMask);
        return hitColliders;
    }

    [Rpc(SendTo.Server)]

    private void QueenMovementRPC(float xClick, float zClick)
    {
        int moveCount = 0;
        int enemyCount = 0;

        xClick = Mathf.Floor(xClick) + .5f;
        zClick = Mathf.Floor(zClick) + .5f;

        Debug.Log(xClick + ", " + zClick);

        checkPos = new Vector3(xClick, transform.position.y, zClick);
        if (Physics.CheckSphere(checkPos, .9f)) StartPositionRPC();

        if (transform.position.x > xClick && transform.position.z < zClick && clickCount > 2)
        {
            for (int i = 0; i < transform.position.x - xClick; i++)
            {
                checkEnemyPos = new Vector3(transform.position.x - i, transform.position.y, transform.position.z + i);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int j = 0; j < enemyInRange.Length; j++)
                    {
                        if (enemyInRange[j].tag == "black")
                        {
                            enemyCount++;
                        }
                        else if (enemyInRange[j].tag == "red") StartPositionRPC();
                    }
                }
            }

            if (enemyCount > 1) StartPositionRPC();

            while (transform.position.x - moveCount > xClick && enemyCount < 2)
            {
                checkEnemyPos = new Vector3(transform.position.x - moveCount, transform.position.y, transform.position.z + moveCount);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                moveCount++;
            }
            clickCount = 0;
            isActive = false;
            if (-(transform.position.x - xClick) == (transform.position.z - zClick) && enemyCount < 2)
            {
                transform.position = new Vector3(xClick, 0, zClick);
            }

            enemyCount = 0;
        }
        else if (transform.position.x < xClick && transform.position.z < zClick && clickCount > 2)
        {
            for (int i = 0; i < xClick - transform.position.x; i++)
            {
                checkEnemyPos = new Vector3(transform.position.x + i, transform.position.y, transform.position.z + i);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int j = 0; j < enemyInRange.Length; j++)
                    {
                        if (enemyInRange[j].tag == "black")
                        {
                            enemyCount++;
                        }
                        else if (enemyInRange[j].tag == "red") StartPositionRPC();
                    }
                }
            }

            if (enemyCount > 1) StartPositionRPC();

            while (xClick - moveCount > transform.position.x && enemyCount < 2)
            {
                checkEnemyPos = new Vector3(transform.position.x + moveCount, transform.position.y, transform.position.z + moveCount);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                moveCount++;
            }
            clickCount = 0;
            isActive = false;
            if ((transform.position.x - xClick) == (transform.position.z - zClick) && enemyCount < 2)
            {
                transform.position = new Vector3(xClick, 0, zClick);
            }

            enemyCount = 0;
        }
        else if (transform.position.x > xClick && transform.position.z > zClick && clickCount > 2)
        {
            for (int i = 0; i < transform.position.x - xClick; i++)
            {
                checkEnemyPos = new Vector3(transform.position.x - i, transform.position.y, transform.position.z - i);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int j = 0; j < enemyInRange.Length; j++)
                    {
                        if (enemyInRange[j].tag == "black")
                        {
                            enemyCount++;
                        }
                        else if (enemyInRange[j].tag == "red") StartPositionRPC();
                    }
                }
            }

            if (enemyCount > 1) StartPositionRPC();

            while (transform.position.x - moveCount > xClick && enemyCount < 2)
            {
                checkEnemyPos = new Vector3(transform.position.x - moveCount, transform.position.y, transform.position.z - moveCount);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                moveCount++;
            }
            clickCount = 0;
            isActive = false;
            if ((transform.position.x - xClick) == (transform.position.z - zClick) && enemyCount < 2)
            {
                transform.position = new Vector3(xClick, 0, zClick);
            }

            enemyCount = 0;
        }
        else if (transform.position.x < xClick && transform.position.z > zClick && clickCount > 2)
        {
            for (int i = 0; i < xClick - transform.position.x; i++)
            {
                checkEnemyPos = new Vector3(transform.position.x + i, transform.position.y, transform.position.z - i);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int j = 0; j < enemyInRange.Length; j++)
                    {
                        if (enemyInRange[j].tag == "black")
                        {
                            enemyCount++;
                        }
                        else if (enemyInRange[j].tag == "red") StartPositionRPC();
                    }
                }
            }

            if (enemyCount > 1) StartPositionRPC();

            while (xClick - moveCount > transform.position.x && enemyCount < 2)
            {
                checkEnemyPos = new Vector3(transform.position.x + moveCount, transform.position.y, transform.position.z - moveCount);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 3);
                if (enemyInRange.Length > 0)
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "black")
                        {
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                moveCount++;
            }
            clickCount = 0;
            isActive = false;
            if (-(transform.position.x - xClick) == (transform.position.z - zClick) && enemyCount < 2)
            {
                transform.position = new Vector3(xClick, 0, zClick);
            }

            enemyCount = 0;
        }
        else if (clickCount > 2)
        {
            StartPositionRPC();
        }


    }

    [Rpc(target: SendTo.ClientsAndHost)]

    private void StartPositionRPC()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isActive = false;
        clickCount = 0;
    }
}