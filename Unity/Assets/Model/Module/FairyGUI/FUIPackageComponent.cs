﻿using System.Collections.Generic;
using FairyGUI;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace ETModel
{
    [ObjectSystem]
    public class FUIPackageAwakeSystem : AwakeSystem<FUIPackageComponent>
    {
        public override void Awake(FUIPackageComponent self)
        {
            //self.AddPackageAsync(UIType.Common).GetAwaiter();
        }
    }

    /// <summary>
	/// 管理所有UI Package
	/// </summary>
	public class FUIPackageComponent: Component
	{
		public const string FUI_PACKAGE_DIR = "Assets/Res/FairyGUI";
		
		private readonly Dictionary<string, UIPackage> packages = new Dictionary<string, UIPackage>();
		
		
		public void AddPackage(string type)
		{
		    if (this.packages.ContainsKey(type))
		        return;
#if UNITY_EDITOR
			UIPackage uiPackage = UIPackage.AddPackage($"{FUI_PACKAGE_DIR}/{type}");
#else
            string uiBundleName = type.StringToAB();
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle(uiBundleName);
	        
            AssetBundle assetBundle = resourcesComponent.GetAssetBundle(uiBundleName);
            UIPackage uiPackage = UIPackage.AddPackage(assetBundle);
#endif
			this.packages.Add(type, uiPackage);
		}
        
		public async ETTask AddPackageAsync(string type)
		{
		    if (this.packages.ContainsKey(type))
		        return;
#if UNITY_EDITOR
            await ETTask.CompletedTask;
            
			UIPackage uiPackage = UIPackage.AddPackage($"{FUI_PACKAGE_DIR}/{type}");
#else
            string uiBundleName = type.StringToAB();
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            await resourcesComponent.LoadBundleAsync(uiBundleName);
	        
            AssetBundle assetBundle = resourcesComponent.GetAssetBundle(uiBundleName);
            UIPackage uiPackage = UIPackage.AddPackage(assetBundle);
#endif
			this.packages.Add(type, uiPackage);
		}

		public void RemovePackage(string type)
		{
			this.packages.TryGetValue(type, out UIPackage package);

            if (package == null)
                return;

			UIPackage.RemovePackage(package.name);
			this.packages.Remove(package.name);
#if !UNITY_EDITOR
			ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(type.StringToAB());
#endif
		}

        /// <summary>
        /// 创建基本常驻FUI包
        /// </summary>
        /// <returns></returns>
        public async ETTask CreateConstPackage()
        {
            await this.AddPackageAsync(UIType.Common);
            await this.AddPackageAsync(UIType.RootLayer);
            await this.AddPackageAsync(UIType.PopMessage);
        }

        
	}
}