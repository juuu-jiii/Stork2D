using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds a scale factor that fits the attached sprite to the screen.
/// Make sure both backdrop sprite AND camera are centred when attaching this
/// script!
/// https://www.loekvandenouweland.com/content/stretch-unity-sprite-to-fill-the-screen.html
/// </summary>
public class FillScreen : MonoBehaviour
{
    [SerializeField] private bool keepAspectRatio;

    // Start is called before the first frame update
    void Start()
    {
        // Although the screen is measured in pixel dimensions, sprites are not,
        // instead only having world space coordinates. Therefore, any math
        // will need to be performed in terms of world space coordinates.

        // Convert top-right position from pixel to world space coordinates.
        var topRightCorner = Camera.main.ScreenToWorldPoint(
            new Vector3(
                Screen.width, 
                Screen.height, 
                Camera.main.transform.position.z));

        // In 2D games, the z-coordinate can be ignored.
        // It is equal to camera size * camera z-position.
        // Changing the camera size, therefore, also alters values returned by
        // ScreenToWorldPoint().

        // Get width and height of world space by doubling the top-right
        // corner's x- and y-coordinate, respectively.
        var worldSpaceWidth = topRightCorner.x * 2;
        var worldSpaceHeight = topRightCorner.y * 2;

        // Calculate sprite width and height in world space.
        var spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;

        // To obtain the factor used to scale the sprite horizontally, divide
        // the screen width by the sprite width.
        // The same concept applies for scaling vertically.
        var scaleFactorX = worldSpaceWidth / spriteSize.x;
        var scaleFactorY = worldSpaceHeight / spriteSize.y;

        // Maintains image aspect ratio to prevent distortion.
        if (keepAspectRatio)
        {
            if (scaleFactorX > scaleFactorY) scaleFactorY = scaleFactorX;
            else scaleFactorX = scaleFactorY;
        }

        // Use the values obtained to scale the sprite appropriately in both
        // directions.
        gameObject.transform.localScale = new Vector3(
            scaleFactorX, 
            scaleFactorY, 
            1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
