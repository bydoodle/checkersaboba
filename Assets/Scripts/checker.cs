using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Unity.Netcode;

public class checker : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera;
    private bool checkQueen = false;
    public bool isActive = false;
    Vector3 checkPos;
    int clickCount = 0;

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

            if (raycastHit.point.x < transform.position.x - .5 && raycastHit.point.x > transform.position.x - 1.5 && raycastHit.point.z > transform.position.z + .5 && raycastHit.point.z < transform.position.z + 1.5)
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
                    StartPosition();
                }
            } else if (raycastHit.point.x > transform.position.x + .5 && raycastHit.point.x < transform.position.x + 1.5 && raycastHit.point.z > transform.position.z + .5 && raycastHit.point.z < transform.position.z + 1.5)
            {
                checkPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
                if (clickCount > 2 && !Physics.CheckSphere(checkPos, .9f))
                {
                    transform.position = new Vector3(transform.position.x + 1, 0f, transform.position.z + 1);
                    isActive = false;
                    clickCount = 0;
                } else if (Physics.CheckSphere(checkPos, .9f) && clickCount > 2)
                {
                    StartPosition();
                }
            } else if (clickCount > 2)
            {
                StartPosition();
            }
        }

        if (transform.position.z > 4)
        {
            checkQueen = true;
        }
    }

    private void StartPosition()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isActive = false;
        clickCount = 0;
    }
}
