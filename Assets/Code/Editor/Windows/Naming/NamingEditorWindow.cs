using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
    public class NamingEditorWindow : EditorWindow
    {
        private const string LABEL_TO_SKIP = "CorrectlyNamed";
        
        [MenuItem("ByteSize/Naming")]
        private static void Open()
        {
            var window = CreateWindow<NamingEditorWindow>("Naming helper");
            window.minSize = new Vector2(EditorGUIUtility.labelWidth * 3.0f, EditorGUIUtility.singleLineHeight * 10.0f);
            window.Show();
        }

        private INamingRule[] _namingRules = new INamingRule[]
        {
            new PrefixNamingRule(),
            new ReplacementNamingRule(),
            new AlphaNumericalSeparationNamingRule(),
            new CapitalFirstCharacterNamingRule()
        };

        private void OnGUI()
        {
            foreach (var namingRule in _namingRules)
                namingRule.Draw();

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"{Selection.assetGUIDs.Length} asset selected.");
            using var validationScope = new EditorGUILayout.HorizontalScope();

            if (GUILayout.Button("Execute"))
            {
                foreach (var GUID in Selection.assetGUIDs)
                {
                    if (!TryProcessAsset(GUID, out var path, out var asset, out var correctedName))
                        continue;

                    AssetDatabase.RenameAsset(path, correctedName);
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
                
            if (GUILayout.Button("Preview"))
            {
                var builder = new StringBuilder();
                    
                foreach (var GUID in Selection.assetGUIDs)
                {
                    if (!TryProcessAsset(GUID, out _, out var asset, out var correctedName))
                    {
                        builder.AppendLine($"- `{asset.name}` WAS SKIPPED");
                        continue;
                    }
                    
                    builder.AppendLine($"- FROM=`{asset.name}` TO=`{correctedName}`");
                }
                    
                Debug.Log(builder);
            }
        }

        private bool TryProcessAsset(string GUID, out string path, out Object asset, out string correctedName)
        {
            correctedName = null;
            
            path = AssetDatabase.GUIDToAssetPath(GUID);
            asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            var labels = AssetDatabase.GetLabels(asset);
            if (labels.Contains(LABEL_TO_SKIP))
                return false;

            correctedName = asset.name;
            foreach (var namingRule in _namingRules)
            {
                if (!namingRule.IsActive)
                    continue;
                
                correctedName = namingRule.Process(correctedName);
            }

            return true;
        }

        private void OnSelectionChange() =>
            Repaint();
    }
}
