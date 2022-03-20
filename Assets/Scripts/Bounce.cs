using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    Sprite sprite;
    Vector3 vel;
    public Vector3 screenPos;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        vel = new Vector3(0.01f, 0.01f, 0);
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, sprite.bounds.extents.y, 0)).y > Screen.height
            || Camera.main.WorldToScreenPoint(transform.position - new Vector3(0, sprite.bounds.extents.y, 0)).y < 0f)
            vel.y *= -1;

        if (Camera.main.WorldToScreenPoint(transform.position + new Vector3(sprite.bounds.extents.x, 0, 0)).x > Screen.width
            || Camera.main.WorldToScreenPoint(transform.position - new Vector3(sprite.bounds.extents.x, 0, 0)).x < 0f)
            vel.x *= -1;

        //if (screenPos.y > Screen.height - sprite.bounds.extents.y * sprite.pixelsPerUnit || screenPos.y < 0f + sprite.bounds.extents.y * sprite.pixelsPerUnit)
        //{
        //    vel.y *= -1;
        //}

        //if (screenPos.x > Screen.width - sprite.bounds.extents.x * sprite.pixelsPerUnit || screenPos.x < 0f + sprite.bounds.extents.x * sprite.pixelsPerUnit)
        //    vel.x *= -1;

        transform.position += vel;
    }
}
