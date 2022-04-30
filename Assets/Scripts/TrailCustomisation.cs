using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TrailCustomisation : MonoBehaviour
{
    //// Text describing the part currently being customised.
    //[SerializeField] private TextMeshProUGUI currentCustomisableTextP1;
    //[SerializeField] private TextMeshProUGUI currentCustomisableTextP2;

    ///// <summary>
    ///// Text describing the current selected round length.
    ///// </summary>
    //[SerializeField] private TextMeshProUGUI currentRoundLengthText;

    // Sprites for the customisable trail segments.
    [SerializeField] private Button[] segments;

    /// <summary>
    /// Third-party colour picker.
    /// </summary>
    [SerializeField] private ColorPicker colourPicker;

    /// <summary>
    /// Controls the alpha value of each colour on the trail.
    /// </summary>
    [SerializeField] private Slider transparency;

    [SerializeField] private TextMeshProUGUI transparencyValueLabel;

    /// <summary>
    /// Currently-selected colour scheme.
    /// </summary>
    private ColourProfile colourProfile;

    /// <summary>
    /// Gradient of each agent's TrailRenderer.
    /// </summary>
    public Gradient TrailGradient { get; private set; }

    private GradientColorKey[] colourKeys;
    private GradientAlphaKey[] alphaKeys;

    ///// <summary>
    ///// Label displaying the numeric value of the R slider.
    ///// </summary>
    //[SerializeField] private TextMeshProUGUI rSliderValueLabel;

    ///// <summary>
    ///// Label displaying the numeric value of the R slider.
    ///// </summary>
    //[SerializeField] private TextMeshProUGUI gSliderValueLabel;

    ///// <summary>
    ///// Label displaying the numeric value of the R slider.
    ///// </summary>
    //[SerializeField] private TextMeshProUGUI bSliderValueLabel;

    ///// <summary>
    ///// Slider controlling R value.
    ///// </summary>
    //[SerializeField] private Slider rSlider;

    ///// <summary>
    ///// Slider controlling G value.
    ///// </summary>
    //[SerializeField] private Slider gSlider;

    ///// <summary>
    ///// Slider controlling B value.
    ///// </summary>
    //[SerializeField] private Slider bSlider;


    ///// <summary>
    ///// List of customisable parts. Used for UI display.
    ///// </summary>
    //private Customisables[] customisables = new Customisables[] 
    //{ 
    //    Customisables.Hair, 
    //    Customisables.Shirt, 
    //    Customisables.Pants, 
    //    Customisables.Shoes
    //};

    // The current segment being customsied.
    public static TrailSegments currentSegment;

    ///// <summary>
    ///// Cycle next in the customisables array.
    ///// </summary>
    //public void NextCustomisable(int player)
    //{
    //    // Load the colour and assign it to the sliders.
    //    switch (player)
    //    {
    //        case 1:
    //            // Circular array implementation.
    //            currentCustomisableP1 = (Customisables)(
    //                ((int)currentCustomisableP1 + 1) 
    //                % customisables.Length);

    //            Color newColourP1 = ColourProfileManager.p1ColourProfile.profile[currentCustomisableP1];
    //            colourPickerP1.color = newColourP1;

    //            // Update text label.
    //            currentCustomisableTextP1.text = customisables[(int)currentCustomisableP1].ToString();
                
    //            break;
    //        case 2:
    //            // Circular array implementation.
    //            currentCustomisableP2 = (Customisables)(
    //                ((int)currentCustomisableP2 + 1)
    //                % customisables.Length);

    //            Color newColourP2 = ColourProfileManager.p2ColourProfile.profile[currentCustomisableP2];
    //            colourPickerP2.color = newColourP2;

    //            // Update text label.
    //            currentCustomisableTextP2.text = customisables[(int)currentCustomisableP2].ToString();

    //            break;
    //    }
    //}

    ///// <summary>
    ///// Cycle previous in the customisables array.
    ///// </summary>
    //public void PreviousCustomisable(int player)
    //{
    //    // Load the colour and assign it to the sliders.
    //    switch (player)
    //    {
    //        case 1:
    //            // Circular array implementation.
    //            // Account for the fact that -1 % y = -1 (ugh).
    //            if (--currentCustomisableP1 < 0) 
    //                currentCustomisableP1 = (Customisables)(customisables.Length - 1);

    //            Color newColourP1 = ColourProfileManager.p1ColourProfile.profile[currentCustomisableP1];
    //            colourPickerP1.color = newColourP1;

    //            // Update text label.
    //            currentCustomisableTextP1.text = customisables[(int)currentCustomisableP1].ToString();

    //            break;
    //        case 2:
    //            // Circular array implementation.
    //            // Account for the fact that -1 % y = -1 (ugh).
    //            if (--currentCustomisableP2 < 0)
    //                currentCustomisableP2 = (Customisables)(customisables.Length - 1);

    //            Color newColourP2 = ColourProfileManager.p2ColourProfile.profile[currentCustomisableP2];
    //            colourPickerP2.color = newColourP2;

    //            // Update text label.
    //            currentCustomisableTextP2.text = customisables[(int)currentCustomisableP2].ToString();

    //            break;
    //    }
    //}

    //public void ChangeRoundLength()
    //{
    //    switch (roundLength)
    //    {
    //        case RoundLengths.Sixty:
    //            roundLength = RoundLengths.Ninety;
    //            break;
    //        case RoundLengths.Ninety:
    //            roundLength = RoundLengths.Sixty;
    //            break;
    //    }

    //    // Update text label.
    //    currentRoundLengthText.text = ((int)roundLength).ToString() + "s";
    //}

    /// <summary>
    /// Commit colour choices to the ColourProfile for P1.
    /// </summary>
    private void SaveChanges(Color colour)
    {
        Color newColour = colour;
        newColour.a = transparency.value;
        colourProfile.profile[currentSegment] = newColour;

        // Populate the colour keys at the relative time 0 and 1 (0 and 100%).
        colourKeys[0].color = segments[0].image.color;
        colourKeys[0].time = 0.0f;
        colourKeys[1].color = segments[1].image.color;
        colourKeys[1].time = 0.5f;
        colourKeys[2].color = segments[2].image.color;
        colourKeys[2].time = 1.0f;

        // Populate the alpha keys at relative time 0 and 1 (0 and 100%)
        alphaKeys[0].alpha = segments[0].image.color.a;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = segments[1].image.color.a;
        alphaKeys[1].time = 0.5f;
        alphaKeys[2].alpha = segments[2].image.color.a;
        alphaKeys[2].time = 1.0f;

        // This is needed to update the overall gradient. Think of it like
        // filling out a D3D struct and then creating the desired component.
        TrailGradient.SetKeys(colourKeys, alphaKeys);
    }

    public void LoadColour()
    {
        Color newColour = colourProfile.profile[currentSegment];
        colourPicker.color = newColour;
        transparency.value = newColour.a;
    }

    ///// <summary>
    ///// Commit colour choices to the ColourProfile for P2.
    ///// </summary>
    //private void SaveChangesP2(Color colour)
    //{
    //    ColourProfileManager.p2ColourProfile.profile[currentCustomisableP2] = colour;
    //}

    // Start is called before the first frame update
    void Start()
    {
        colourProfile = new ColourProfile();
        currentSegment = 0;

        TrailGradient = new Gradient();
        colourKeys = new GradientColorKey[segments.Length];
        alphaKeys = new GradientAlphaKey[segments.Length];

        //currentCustomisableP1 = 0;
        //currentCustomisableP2 = 0;

        // Commit changes when colour picker and slider values are changed.
        colourPicker.onColorChanged += SaveChanges;
        transparency.onValueChanged.AddListener(delegate { SaveChanges(colourPicker.color); });
        //colourPickerP2.onColorChanged += SaveChangesP2;

        //// Load ColourProfile and display colours on the sprite.
        //for (int i = 0; i < spritesP1.Length; i++)
        //    spritesP1[i].color = ColourProfileManager.p1ColourProfile.profile[customisables[i]];

        //for (int i = 0; i < spritesP2.Length; i++)
        //    spritesP2[i].color = ColourProfileManager.p2ColourProfile.profile[customisables[i]];

        //// Initialise the slider colours to match the first item in the array.
        //rSlider.value = sprites[0].color.r;
        //gSlider.value = sprites[0].color.g;
        //bSlider.value = sprites[0].color.b;

        //// Initialise the colour picker to match the first array element's colour.
        //colourPicker.color = spritesP1[0].color;
        //colourPickerP2.color = spritesP2[0].color;

        //// Initialise text labels.
        //currentCustomisableTextP1.text = customisables[(int)currentCustomisableP1].ToString();
        //currentCustomisableTextP2.text = customisables[(int)currentCustomisableP2].ToString();
        //currentRoundLengthText.text = ((int)roundLength).ToString() + "s";
    }

    // Update is called once per frame
    void Update()
    {
        // Update label values each frame, normalising to the range [0, 255].
        transparencyValueLabel.text = (transparency.value / (transparency.maxValue - transparency.minValue) * 255).ToString("F0");

        // Update sprite colours each frame.
        Color newColour = colourPicker.color;
        newColour.a = transparency.value;
        segments[(int)currentSegment].image.color = newColour;
        //spritesP2[(int)currentCustomisableP2].color = colourPickerP2.color;
    }
}
