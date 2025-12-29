using Dave6.StatSystem.Effect;
using UnityEditor;
using UnityEngine;

namespace Dave6.StatSystemEditor
{
    [CustomEditor(typeof(EffectDefinition))]
    public class EffectDefinitionEditor : Editor
    {
        #region SerializedPropertys
        EffectDefinition definition;

        bool showSourceType = true;
        SerializedProperty sourceType;
        SerializedProperty flatValue;
        SerializedProperty sources;

        bool applyModeFoldout = true;
        SerializedProperty applyMode;
        SerializedProperty instant;
        SerializedProperty periodic;
        SerializedProperty sustained;

        SerializedProperty duration;
        SerializedProperty outputMultiplier;
        #endregion

        void OnEnable()
        {
            definition = (EffectDefinition)target;

            sourceType = serializedObject.FindProperty("sourceType");
            sources = serializedObject.FindProperty("sources");
            flatValue = serializedObject.FindProperty("flatValue");

            applyMode = serializedObject.FindProperty("applyMode");
            instant = serializedObject.FindProperty("instant");
            sustained = serializedObject.FindProperty("sustained");
            periodic = serializedObject.FindProperty("periodic");

            duration = serializedObject.FindProperty("duration");
            outputMultiplier = serializedObject.FindProperty("outputMultiplier");

            EnsurePayloadsExist();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 이거 이쁨
            showSourceType = EditorGUILayout.InspectorTitlebar(showSourceType, definition);
            if (showSourceType)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Definition Source", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(sourceType);
                DrawEffectValue();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(16);

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Payload Setting", EditorStyles.boldLabel);
                EditorGUILayout.Space(8);
                EditorGUILayout.PropertyField(applyMode);
                DrawPayload();            
                EditorGUILayout.PropertyField(outputMultiplier);
                EditorGUILayout.EndVertical();
            }

            

            serializedObject.ApplyModifiedProperties();
        }

        void EnsurePayloadsExist()
        {
            bool dirty = false;

            if (definition.instant == null)
            {
                definition.instant = new InstantPayload();
                dirty = true;
            }

            if (definition.sustained == null)
            {
                definition.sustained = new SustainedPayload();
                dirty = true;
            }

            if (definition.periodic == null)
            {
                definition.periodic = new PeriodicPayload();
                dirty = true;
            }

            if (dirty)
            {
                EditorUtility.SetDirty(definition);
                //AssetDatabase.SaveAssets();
            }
        }

        void DrawEffectValue()
        {
            var source = (SourceType)sourceType.enumValueIndex;

            switch (source)
            {
                case SourceType.Owner:
                EditorGUILayout.PropertyField(sources, true);
                break;
                case SourceType.Static:
                EditorGUILayout.PropertyField(flatValue);
                break;
            }
        }

        void DrawPayload()
        {
            EditorGUILayout.Space(4);
            var mode = (EffectApplyMode)applyMode.enumValueIndex;
            
            switch (mode)
            {
                case EffectApplyMode.Instant:
                {
                    var operationType = instant.FindPropertyRelative("operationType");
                    EditorGUILayout.PropertyField(operationType);
                    break;
                }
                case EffectApplyMode.Periodic:
                {
                    var operationType = periodic.FindPropertyRelative("operationType");
                    var tickInterval = periodic.FindPropertyRelative("tickInterval");
                    EditorGUILayout.PropertyField(operationType);
                    EditorGUILayout.PropertyField(duration);
                    EditorGUILayout.PropertyField(tickInterval);
                    break;
                }
                case EffectApplyMode.Sustained:
                {
                    var valueType = sustained.FindPropertyRelative("valueType");
                    EditorGUILayout.PropertyField(valueType);
                    EditorGUILayout.PropertyField(duration);
                    break;
                }
            }
        }
    }
}
