using UnityEngine;
using UnityEditor;

public class MinMax : PropertyAttribute
{
    public float minLimit, maxLimit;

    public MinMax(float minLimit, float maxLimit)
    {
        this.minLimit = minLimit;
        this.maxLimit = maxLimit;
    }
}


[System.Serializable]
public class MinMaxRange
{
    public float min, max;

    public MinMaxRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }


    /// <summary>
    /// Returns a random value from the range.
    /// </summary>
    /// <returns></returns>
    public float GetRandomValue()
    {
        return Random.Range(min, max);
    }


    /// <summary>
    /// Checks if a value is included in the range (inclusive)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsValueInRange(float value)
    {
        return value >= min && value <= max;
    }


    /// <summary>
    /// Returns a value that is within the bounds of the range.
    /// If the value is too low, it is set to the minimum of the range.
    /// If the value is too high, it is set to the maximum of the range.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float Clamp(float value)
    {
        if (value < min) { value = min; }
        else if (value > max) { value = max; }
        return value;
    }

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        // draw as a slider or intslider based on whether it is a float or integer
        if (property.type != "MinMaxRange") { Debug.LogError("ERROR: Can't use MinMaxRangeDrawer without MinMaxRange type!"); }
        else
        {
            MinMax range = attribute as MinMax;
            var minValue = property.FindPropertyRelative("min");
            var maxValue = property.FindPropertyRelative("max");
            var newMin = minValue.floatValue;
            var newMax = maxValue.floatValue;

            var xDivision = position.width * 0.33f;
            var yDivision = position.height * 0.5f;
            EditorGUI.LabelField(new Rect(position.x, position.y, xDivision, yDivision), label);

            EditorGUI.LabelField(new Rect(position.x, position.y + yDivision, position.width, yDivision), range.minLimit.ToString("0.##"));
            EditorGUI.LabelField(new Rect(position.x + position.width - 28f, position.y + yDivision, position.width, yDivision), range.maxLimit.ToString("0.##"));
            EditorGUI.MinMaxSlider(new Rect(position.x + 24f, position.y + yDivision, position.width - 48f, yDivision), ref newMin, ref newMax, range.minLimit, range.maxLimit);

            EditorGUI.LabelField(new Rect(position.x + xDivision, position.y, xDivision, yDivision), "From: ");
            newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision + 40f, position.y, xDivision - 40f, yDivision), newMin), range.minLimit, newMax);
            EditorGUI.LabelField(new Rect(position.x + xDivision * 2f + 5f, position.y, xDivision, yDivision), "To: ");
            newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision * 2f + 28f, position.y, xDivision - 28f, yDivision), newMax), newMin, range.maxLimit);

            minValue.floatValue = newMin;
            maxValue.floatValue = newMax;

        }
    }

}
#endif

