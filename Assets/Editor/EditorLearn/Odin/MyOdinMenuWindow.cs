using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorLearn
{
    // 带树形菜单栏的窗口
    public class MyOdinMenuWindow:OdinMenuEditorWindow
    {
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;
            tree.Add("Settings", GeneralDrawerConfig.Instance);
            tree.Add("Utilities", new TextureUtilityEditor());
            tree.Add("Utilities/MyStruct", new MyStruct());
            tree.AddAllAssetsAtPath("Odin Settings", "", typeof(ScriptableObject), true, true);
            return tree;
            
            var tree2 = new OdinMenuTree();
            tree2.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            tree2.Add("Menu Style", tree2.DefaultMenuStyle);
            var allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(x => x.StartsWith("Assets/"))
                .OrderBy(x => x);
            foreach (var path in allAssets)
            {
                tree2.AddAssetAtPath(path.Substring("Assets/".Length), path);
            }
            tree2.EnumerateTree().AddThumbnailIcons();
            return tree2;
        }
    }

    public class TextureUtilityEditor
    {
        [BoxGroup("Tool"), HideLabel, EnumToggleButtons]
        public Tool tool;
        public List<Texture> Textures;

        [Button(ButtonSizes.Large), HideIf("tool", Tool.View)]
        public void SomeAction()
        {
        }

        [Button(ButtonSizes.Large), ShowIf("tool", Tool.Move)]
        public void SomeOtherAction()
        {
            
        }
    }
}