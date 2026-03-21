using UnityEngine;

[CreateAssetMenu(fileName = "LensBase", menuName = "Scriptable Objects/LensBase")]
public class LensBase : ScriptableObject
{
    public bool isFixedLength = false;
    [Range(10f, 300f)]
    public float focalLength = 0.1f;
    [Range(10f, 300f)]
    public float focalLengthMax = 0.1f;

    [Range(1.2f, 22f)]
    public float aperture = 0.1f;
    [Range(1.2f, 22f)]
    public float apertureMax = 0.1f;

    [Range(5f, 15f)]
    public int bladeCount = 0;
}
