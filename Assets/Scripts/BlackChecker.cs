using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

public class BlackChecker : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera;
    private bool checkQueen = false;
    public bool isActive = false;
    Vector3 checkPos;
    Vector3 checkEnemyPos;
    int clickCount = 0;
    int enemyID;

    private void Start()
    {
        gameObject.tag = "black";
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

            if (raycastHit.point.x < transform.position.x - .5 && raycastHit.point.x > transform.position.x - 1.5 && raycastHit.point.z < transform.position.z - .5 && raycastHit.point.z > transform.position.z - 1.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1);
                if (clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    transform.position = new Vector3(transform.position.x - 1, 0f, transform.position.z - 1);
                    isActive = false;
                    clickCount = 0;
                }
                else if (Physics.CheckSphere(checkPos, .9f) && clickCount > 2)
                {
                    StartPosition();
                }
            }
            else if (raycastHit.point.x > transform.position.x + .5 && raycastHit.point.x < transform.position.x + 1.5 && raycastHit.point.z < transform.position.z - .5 && raycastHit.point.z > transform.position.z - 1.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1);
                if (clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    transform.position = new Vector3(transform.position.x + 1, 0f, transform.position.z - 1);
                    isActive = false;
                    clickCount = 0;
                }
                else if (Physics.CheckSphere(checkPos, .9f) && clickCount > 2)
                {
                    StartPosition();
                }
            }
            else if (raycastHit.point.x > transform.position.x + 1.5 && raycastHit.point.x < transform.position.x + 2.5 && raycastHit.point.z < transform.position.z - 1.5 && raycastHit.point.z > transform.position.z - 2.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z - 2);
                checkEnemyPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 6);
                if (enemyInRange.Length > 0 && clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "red")
                        {
                            transform.position = new Vector3(transform.position.x + 2, 0f, transform.position.z - 2);
                            isActive = false;
                            clickCount = 0;
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                else if (clickCount > 2)
                {
                    StartPosition();
                }
            }
            else if (raycastHit.point.x < transform.position.x - 1.5 && raycastHit.point.x > transform.position.x - 2.5 && raycastHit.point.z < transform.position.z - 1.5 && raycastHit.point.z > transform.position.z - 2.5 && !checkQueen)
            {
                checkPos = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z - 2);
                checkEnemyPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 6);
                if (enemyInRange.Length > 0 && clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "red")
                        {
                            transform.position = new Vector3(transform.position.x - 2, 0f, transform.position.z - 2);
                            isActive = false;
                            clickCount = 0;
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                else if (clickCount > 2)
                {
                    StartPosition();
                }
            }
            else if (clickCount > 2 && !checkQueen)
            {
                StartPosition();
            }
            else if (clickCount > 2 && checkQueen)
            {
                QueenMovement(raycastHit.point.x, raycastHit.point.z);
            }
        }

        if (transform.position.z < -4)
        {
            checkQueen = true;
        }
    }

    Collider[] EnemyInRange(Vector3 enemyPosition, float range, int layer)
    {
        int layerMask = 3 << layer;
        Collider[] hitColliders = Physics.OverlapSphere(enemyPosition, range, layerMask);
        return hitColliders;
    }

    private void StartPosition()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isActive = false;
        clickCount = 0;
    }

    private void QueenMovement(float xClick, float zClick)
    {
        int moveCount = 0;

        xClick = Mathf.Floor(xClick) + .5f;
        zClick = Mathf.Floor(zClick) + .5f;

        Debug.Log(xClick + ", " + zClick);

        checkPos = new Vector3(xClick, transform.position.y, zClick);
        if (Physics.CheckSphere(checkPos, .9f)) StartPosition();

        if (transform.position.x > xClick && transform.position.z < zClick && clickCount > 2)
        {
            while (transform.position.x - moveCount > xClick)
            {
                Debug.Log("test");

                checkEnemyPos = new Vector3(transform.position.x - moveCount, transform.position.y, transform.position.z + moveCount);
                Collider[] enemyInRange = EnemyInRange(checkEnemyPos, .9f, 6);
                if (enemyInRange.Length > 0)
                {
                    for (int i = 0; i < enemyInRange.Length; i++)
                    {
                        if (enemyInRange[i].tag == "red")
                        {
                            Destroy(enemyInRange[i].gameObject);
                        }
                    }
                }
                moveCount++;
            }
            clickCount = 0;
            isActive = false;
            if (-(transform.position.x - xClick) == (transform.position.z - zClick))
            {
                transform.position = new Vector3(xClick, 0, zClick);
            }
        }
        else if (transform.position.x < xClick && transform.position.z < zClick && clickCount > 2)
        {

        }
        else if (transform.position.x > xClick && transform.position.z > zClick && clickCount > 2)
        {

        }
        else if (transform.position.x < xClick && transform.position.z > zClick && clickCount > 2)
        {

        }
        else if (clickCount > 2)
        {
            StartPosition();
        }


    }

    private void CheckPositions()
    {

    }
}
