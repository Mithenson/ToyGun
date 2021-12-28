using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;

namespace ByteSize.Editor
{
	public static class MenuItems
	{
		[MenuItem("Assets/Build AssetBundles")]
		private static void BuildAllAssetBundles()
		{
			var directory = "Assets/StreamingAssets";
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		}
	}
}