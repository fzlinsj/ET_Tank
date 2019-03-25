﻿using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{

    [ObjectSystem]
    public class TankMoveComponentStartSystem: AwakeSystem<TankMoveComponent>
    {
        public override void Awake(TankMoveComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TankMoveComponentUpdateSystem : UpdateSystem<TankMoveComponent>
    {
        public override void Update(TankMoveComponent self)
        {
            self.Update();
        }
    }

    public class TankMoveComponent : Component
    {
        private class AxleInfo
        {
            //左轮
            public WheelCollider leftWheel;
            //右轮
            public WheelCollider rightWheel;
            //马力
            public bool montor;
            //转向
            public bool steering;
        }

        //轮轴
        private List<AxleInfo> axleInfos;
        //马力
        private float motor = 0;
        private readonly float maxMotor = 320f;
        //制动
        private float breakTorque = 0;
        private readonly float maxBreakTorque = 150f;
        //转向角
        private float steering = 0;
        private readonly float maxSteering = 40;

        //轮子
        private Transform wheels;

        //履带
        private Transform tracks;

        //音效
        private AudioSource motorAudioSource;
        private AudioClip motorClip;

        public void Awake()
        {
            Tank tank = this.GetParent<Tank>();
            wheels = tank.GameObject.FindComponentInChildren<Transform>("wheels");

            tracks = tank.GameObject.FindComponentInChildren<Transform>("tracks");

            motorAudioSource = tank.GameObject.AddComponent<AudioSource>();
            motorAudioSource.spatialBlend = 1;

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            motorClip = bundleGameObject.Get<AudioClip>("motor");

            axleInfos = new List<AxleInfo>(2);
            
            // 前轮
            AxleInfo axleInfo = new AxleInfo();
            axleInfo.leftWheel = tank.GameObject.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelL1");
            axleInfo.rightWheel = tank.GameObject.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelR1");
            axleInfo.montor = false;
            axleInfo.steering = true;
            this.axleInfos.Add(axleInfo);
            axleInfo = new AxleInfo();
            axleInfo.leftWheel = tank.GameObject.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelL2");
            axleInfo.rightWheel = tank.GameObject.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelR2");
            axleInfo.montor = true;
            axleInfo.steering = false;
            this.axleInfos.Add(axleInfo);
        }

        public void Update()
        {

            this.PlayerCtrl();

            this.Move();

            this.WheelsRolling();

            this.TrackRolling();

            this.Audio();
        }

        /// <summary>
        /// 玩家操作
        /// </summary>
        private void PlayerCtrl()
        {
            //马力和转向
            this.motor = this.maxMotor * Input.GetAxis("Vertical");
            this.steering = this.maxSteering * Input.GetAxis("Horizontal");

            this.breakTorque = 0;
            foreach (AxleInfo axleInfo in this.axleInfos)
            {
                if (axleInfo.leftWheel.rpm > 5 && this.motor < 0)
                    this.breakTorque = this.maxBreakTorque;
                else if (axleInfo.leftWheel.rpm < -5 && this.motor > 0)
                    this.breakTorque = this.maxBreakTorque;
            }
        }

        /// <summary>
        /// 坦克移动
        /// </summary>
        private void Move()
        {
            foreach (AxleInfo axleInfo in this.axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = axleInfo.rightWheel.steerAngle = this.steering;
                }

                if (axleInfo.montor)
                {
                    axleInfo.leftWheel.motorTorque = axleInfo.rightWheel.motorTorque = this.motor;
                }

                if (true)
                {
                    axleInfo.leftWheel.brakeTorque = axleInfo.rightWheel.brakeTorque = this.breakTorque;
                }
            }
        }

        /// <summary>
        /// 轮子旋转
        /// </summary>
        private void WheelsRolling()
        {
            axleInfos[1].leftWheel.GetWorldPose(out Vector3 _,out Quaternion rotation);
            foreach (Transform wheel in this.wheels)
            {
                wheel.rotation = rotation;
            }
        }

        /// <summary>
        /// 履带滚动
        /// </summary>
        private void TrackRolling()
        {
            float offset = 0;

            if (this.wheels.GetChild(0) != null)
                offset = this.wheels.GetChild(0).localEulerAngles.x;

            //Log.Info($"offset = {offset}");


            offset = offset / 90f;

            foreach (Transform track in this.tracks)
            {
                MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();

                if(mr == null)
                    continue;

                Material mtl = mr.material;
                
                mtl.mainTextureOffset = new Vector2(0,offset);
            }
        }

        /// <summary>
        /// 音效播放
        /// </summary>
        private void Audio()
        {
            if (PF.Mathf.Abs(this.motor) > 0.1f && !this.motorAudioSource.isPlaying)
            {
                this.motorAudioSource.loop = true;
                this.motorAudioSource.clip = this.motorClip;
                this.motorAudioSource.Play();
            }
            else if (PF.Mathf.Abs(this.motor) <= 0.1f)
            {
                this.motorAudioSource.Pause();
            }
        }

    }
}