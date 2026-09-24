using UnityEngine;

[CreateAssetMenu(fileName = "TypewriterConfig", menuName = "Config/Typewriter")]

public class TypewriterConfig : ScriptableObject
{
    public bool typingEnabled = true;
    public float charPerSecond = 30f;
    private static TypewriterConfig instance;

    public static TypewriterConfig Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<TypewriterConfig>("TypewriterConfig");
            if (instance == null)
                instance = CreateInstance<TypewriterConfig>();

            return instance;

        }
    }
}
