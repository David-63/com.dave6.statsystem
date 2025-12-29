using Dave6.StatSystem;
using Dave6.StatSystem.Stat;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dave6.StatSystemEditor
{
    [CustomEditor(typeof(StatDatabase))]
    public class StatDatabaseEditor : Editor
    {
        StatDatabase database;

        SerializedProperty attributeStatTags;
        SerializedProperty secondaryStatTags;
        SerializedProperty resourceStatTags;
        ReorderableList attributeList;
        ReorderableList secondaryList;
        ReorderableList resourceList;

        void OnEnable()
        {
            database = (StatDatabase)target;
            attributeStatTags = serializedObject.FindProperty("attributeStatTags");
            attributeList = CreateReorderableList(attributeStatTags, "Attribute Stats");

            secondaryStatTags = serializedObject.FindProperty("secondaryStatTags");
            secondaryList = CreateReorderableList(secondaryStatTags, "Secondary Stats");

            resourceStatTags = serializedObject.FindProperty("resourceStatTags");
            resourceList = CreateReorderableList(resourceStatTags, "Resource Stats");
        }

        ReorderableList CreateReorderableList(SerializedProperty property, string headerLabel)
        {
            ReorderableList list = new ReorderableList(serializedObject, property, true, true, true, true);

            // 리스트 상단 헤더 그리기
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, headerLabel);
            };

            // 각 리스트 요소를 어떻게 그릴지 정의
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // 현재 인덱스의 요소 가져오기
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                // 각 요소의 필드에 접근
                SerializedProperty statTag = element.FindPropertyRelative("statTag");
                SerializedProperty statDefinition = element.FindPropertyRelative("statDefinition");

                rect.height = EditorGUI.GetPropertyHeight(element, true);

                string label = "None";
                if (statTag.objectReferenceValue != null)
                {
                    var tag = statTag.objectReferenceValue as StatTag;
                    label = tag.tagName;
                }
                if (statDefinition.objectReferenceValue != null)
                {
                    var def = statDefinition.objectReferenceValue as StatDefinition;
                    label += $" | O";
                }
                EditorGUI.PropertyField(rect, element, new GUIContent(label), true);
            };

            list.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true);
            };

            return list;
        }

        void DrawStatBindTag(ReorderableList list, SerializedProperty property, string propertyName,string headerLabel)
        {
            // List를 Inspector UI로 그리기 위한 설정
            property = serializedObject.FindProperty(propertyName);

            // ReorderableList 생성
            // 편집대상, 리스트 대상, 드래그 허용, 헤더 표시, 추가버튼 표시, 제거버튼 표시
            list = new ReorderableList(serializedObject, property, true, true, true, true);

            // 리스트 상단 헤더 그리기
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, headerLabel);
            };

            // 각 리스트 요소를 어떻게 그릴지 정의
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // 현재 인덱스의 요소 가져오기
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                // 각 요소의 필드에 접근
                SerializedProperty statTag = element.FindPropertyRelative("statTag");
                SerializedProperty statDefinition = element.FindPropertyRelative("statDefinition");

                // -4는 패딩
                float halfWidth = rect.width / 2 - 4f;
                // statTag 필드 표시
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                    statTag, GUIContent.none);

                // statDefinition 필드 표시
                EditorGUI.PropertyField(
                    new Rect(rect.x + halfWidth, rect.y, halfWidth, EditorGUIUtility.singleLineHeight),
                    statDefinition, GUIContent.none);
            };
            // 각 요소의 높이 계산
            list.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 4f; // 각 요소의 높이
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            attributeList.DoLayoutList();
            secondaryList.DoLayoutList();
            resourceList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
    // [CustomPropertyDrawer(typeof(StatBindTag))]
    // public class StatBindTagDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         var statTag = property.FindPropertyRelative("statTag");

    //         string summary = GetSummary(statTag);

    //         EditorGUI.PropertyField(position, property, new GUIContent(summary));
    //     }
    //     string GetSummary(SerializedProperty tag)
    //     {
    //         var statTag = tag.objectReferenceValue as StatTag;
    //         var name = statTag != null ? statTag.tagName : "None";            

    //         return $"{name}";
    //     }
    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         return EditorGUI.GetPropertyHeight(property, label, true);
    //     }
    // }
}