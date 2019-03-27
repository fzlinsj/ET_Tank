﻿using System;
using NPOI.OpenXmlFormats.Wordprocessing;
using PF;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using ETHotfix;

namespace ETModel
{
    [ObjectSystem]
    public class TurretComponentAwakeSystem : AwakeSystem<TurretComponent>
    {
        public override void Awake(TurretComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TurretComponentLateUpdateSystem : LateUpdateSystem<TurretComponent>
    {
        public override void LateUpdate(TurretComponent self)
        {
            self.LateUpdate();
        }
    }


    public static class StaticClass
    {
        public static int a;
    }

    /// <summary>
    /// 炮塔旋转组件
    /// </summary>
    public class TurretComponent : Component
    {
        public static Action<float, float> UpdatePos;

        private Transform turretTransform;

        // 炮塔旋转速度
        private float rotSpeed = 0.5f;

        // 炮塔目标角度
        private float rotTarget = 0f;
        private float rollTarget = 0f;

        //炮管
        private Transform gunTransform;

        //炮管旋转角度范围
        private float maxRoll = 60f - 40f;
        private float minRoll = 10f - 40f;

        public void Awake()
        {
            this.turretTransform = this.GetParent<Tank>().GameObject.FindChildObjectByPath("turret").transform;
            this.gunTransform = this.GetParent<Tank>().GameObject.FindChildObjectByPath("turret/gun").transform;
        }

        public void LateUpdate()
        {
            this.TargetSignPos();
            this.TurretRotate();
            this.GunRotate();
            this.UpdateBoomIconPos();
        }

        private void UpdateBoomIconPos()
        {
            Vector3 hitPoint = Vector3.zero;

            RaycastHit raycastHit;

            Vector3 pos = this.gunTransform.position ;

            Ray ray = new Ray(pos,this.gunTransform.forward);

            hitPoint = Physics.Raycast(ray, out raycastHit, 100f)? raycastHit.point : ray.GetPoint(100f);

            Debug.DrawLine(ray.origin, hitPoint,Color.red);//划出射线，在scene视图中能看到由摄像机发射出的射线

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(hitPoint);


            UpdatePos?.Invoke(screenPoint.x, screenPoint.y);
        }


        /// <summary>
        /// 屏幕中间准心
        /// </summary>
        private void TargetSignPos()
        {
            Vector3 hitPoint = Vector3.zero;
            RaycastHit raycastHit;
            Vector3 centerVec = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
            Ray ray = Camera.main.ScreenPointToRay(centerVec);

            LayerMask layerMask = ~(1 << 9);


            hitPoint = Physics.Raycast(ray, out raycastHit, 100f, layerMask)? raycastHit.point : ray.GetPoint(100f);

            Debug.DrawLine(ray.origin, hitPoint, Color.green);//划出射线，在scene视图中能看到由摄像机发射出的射线

            Vector3 dir = hitPoint - this.gunTransform.position;

            //Log.Info($"dir = {dir}");

            Quaternion angle = Quaternion.LookRotation(dir);

            this.rotTarget = angle.eulerAngles.y;

            this.rollTarget = angle.eulerAngles.x;

            Log.Info($"rotTarget = {this.rotTarget},rollTarget = {this.rollTarget}");

            //GameObject.Find("TargetCube").transform.position = hitPoint;
        }

        /// <summary>
        /// 炮塔左右旋转
        /// </summary>
        private void TurretRotate()
        {
            // 炮塔角度
            //rotTarget = Camera.main.transform.eulerAngles.y;
            
            float angle = this.turretTransform.eulerAngles.y - this.rotTarget;
            if (angle < 0)
                angle += 360;
            if (angle > this.rotSpeed && angle < 180)
                this.turretTransform.Rotate(0f, -this.rotSpeed, 0f);
            else if (angle > 180 && angle < 360 - this.rotSpeed)
                this.turretTransform.Rotate(0f, this.rotSpeed, 0f);

        }


        /// <summary>
        /// 炮管上下旋转
        /// </summary>
        private void GunRotate()
        {
            //this.rollTarget = Camera.main.transform.eulerAngles.x;
            //this.rollTarget= this.rollTarget * 2f / 3f - 10f;

            //Log.Info($"rollTarget = {this.rollTarget}");

            

            //获取角度
            Vector3 worldEuler = this.gunTransform.eulerAngles;
            Vector3 localEuler = this.gunTransform.localEulerAngles;

            //世界坐标系角度计算
            worldEuler.x = this.rollTarget;
            this.gunTransform.eulerAngles = worldEuler;
            //本地坐标系角度限制
            Vector3 euler = this.gunTransform.localEulerAngles;
            if (euler.x > 180)
                euler.x -= 360;

            //euler.x = PF.Mathf.Clamp(euler.x, this.minRoll, this.maxRoll);

            this.gunTransform.localEulerAngles = new Vector3(euler.x,localEuler.y,localEuler.z);

            //Log.Info($"localEulerAngles = {this.gunTransform.localEulerAngles}");
        }

    }
}