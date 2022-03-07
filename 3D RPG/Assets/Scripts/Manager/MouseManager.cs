using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseManager: MonoBehaviour
{
    public event Action<Vector3> OnMouseClickEvent;
    public event Action<GameObject> OnEnemyClickEvent;
    public Texture2D point, doorway, attack, target, arrow;

    public static MouseManager Instance;

    void Awake()
    {
        if (Instance != null) { Destroy(Instance.gameObject); }
        Instance = this;
    }

    RaycastHit hitInfo;
    void Update()
    {
        SerCorsorTexture();
        MouseControl();
    }

    void SerCorsorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            switch (hitInfo.collider.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16f, 16f), CursorMode.Auto);
                break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16f, 16f), CursorMode.Auto);
                break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("Ground"))
            {
                OnMouseClickEvent?.Invoke(hitInfo.point);
            }
            else if (hitInfo.collider.CompareTag("Enemy"))
            {
                OnEnemyClickEvent?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}
