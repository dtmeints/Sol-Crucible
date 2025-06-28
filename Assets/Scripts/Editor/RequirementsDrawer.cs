using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Requirements))]
public class RequirementsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty randomizeProp = property.FindPropertyRelative("Randomize");
            SerializedProperty totalProp = property.FindPropertyRelative("TotalToRequire");
            SerializedProperty air = property.FindPropertyRelative("airRequired");
            SerializedProperty fire = property.FindPropertyRelative("fireRequired");
            SerializedProperty earth = property.FindPropertyRelative("earthRequired");
            SerializedProperty water = property.FindPropertyRelative("waterRequired");

            Rect lineRect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(lineRect, randomizeProp);
            y += lineHeight + spacing;

            if (randomizeProp.boolValue)
            {
                // Show "TotalToRequire"
                lineRect = new Rect(position.x, y, position.width, lineHeight);
                EditorGUI.PropertyField(lineRect, totalProp);
            }
            else
            {
                // Show element requirements
                lineRect = new Rect(position.x, y, position.width, lineHeight); EditorGUI.PropertyField(lineRect, air);
                y += lineHeight + spacing;
                lineRect = new Rect(position.x, y, position.width, lineHeight); EditorGUI.PropertyField(lineRect, fire);
                y += lineHeight + spacing;
                lineRect = new Rect(position.x, y, position.width, lineHeight); EditorGUI.PropertyField(lineRect, earth);
                y += lineHeight + spacing;
                lineRect = new Rect(position.x, y, position.width, lineHeight); EditorGUI.PropertyField(lineRect, water);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 1; // foldout

        if (property.isExpanded)
        {
            lines++; // Randomize

            SerializedProperty randomizeProp = property.FindPropertyRelative("Randomize");

            if (randomizeProp != null && randomizeProp.boolValue)
                lines += 1; // TotalToRequire
            else
                lines += 4; // air/fire/earth/water
        }

        return lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}