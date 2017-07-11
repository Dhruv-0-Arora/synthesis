﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletSharp;
using BulletSharp.Math;
using System.Collections;
using BulletUnity.Debugging;
namespace BulletUnity { 
    [AddComponentMenu("Physics Bullet/RigidBody")]
    /// <summary>
    /// This is a class that handles everything associated with the driver practice mode.
    /// It 'cheats physics' to overcome the limitations that our current simulation has to create a beter environment for drivers to practice and interact with game objects.
    /// 
    /// </summary>
    public class DriverPractice : MonoBehaviour {

        private UnityEngine.Vector3 positionOffset; //position offset vector for gamepiece while its being held
        private UnityEngine.Vector3 releaseVelocity; //velocity vector for gamepiece when it is released

        private GameObject intakeNode; //node that is identified for intaking gamepieces
        private GameObject releaseNode; //node that is identified for holding/releasing gamepieces
        private Interactor intakeInteractor;

        private int holdingLimit; //the maximum number of game objects that this robot can hold at any given time.

        private List<GameObject> objectsHeld; //list of gamepieces this robot is currently holding
        private List<string> gamepieces = new List<string>(); //list of the identifiers of gamepieces in the current field

        public bool modeEnabled = false;
        /// <summary>
        /// If configuration file exists, loads information and auto-configures robot.
        /// If coniguration file doesn't exist, initializes variables for users to configure.
        /// 
        /// Also loads gamepiece list from MainState.cs.
        /// 
        /// *NOTE: Because gamepiece identification in the new field format doesn't exist yet, we are using a predetermined gamepiece list. This must be changed as soon as support for gamepieces is added in the field exporter.*
        /// </summary>
        private void Start()
        {
            objectsHeld = new List<GameObject>();
            gamepieces = new List<string>();

            gamepieces.Add("GRAY_TOTE"); //Replace
            holdingLimit = 0; //Replace

            positionOffset = new UnityEngine.Vector3(0, -.4f, -.1f); //Replace

            intakeNode = GameObject.Find("node_0.bxda"); //Replace
            intakeInteractor = intakeNode.AddComponent<Interactor>();
            releaseNode = GameObject.Find("node_0.bxda"); //Replace

            releaseVelocity = new UnityEngine.Vector3(0, 7.8f, -3.9f); //Replace
            intakeInteractor.SetKeyword(gamepieces[0]); //Replace
        }
	
	    // Update is called once per frame
	    void Update () {
            if (modeEnabled)
            {
                if (Input.GetKey(Controls.ControlKey[(int)Controls.Control.Pickup]))
                {
                    Intake();
                }
                if (Input.GetKey(Controls.ControlKey[(int)Controls.Control.Release]))
                {
                    ReleaseGamepiece();
                }
                else
                {
                    HoldGamepiece();
                }

                //This should be replaced to be used buttons in a sort of configuration menu
                if (Input.GetKey(KeyCode.Alpha3)) changeOffsetY(0.1f);
                if (Input.GetKey(KeyCode.Alpha4)) changeOffsetY(-0.1f);
                if (Input.GetKey(KeyCode.Alpha5)) changeOffsetZ(0.1f);
                if (Input.GetKey(KeyCode.Alpha6)) changeOffsetZ(-0.1f);

                if (Input.GetKey(KeyCode.Alpha7)) changeReleaseY(0.1f);
                if (Input.GetKey(KeyCode.Alpha8)) changeReleaseY(-0.1f);
                if (Input.GetKey(KeyCode.Alpha9)) changeReleaseZ(0.1f);
                if (Input.GetKey(KeyCode.Alpha0)) changeReleaseZ(-0.1f);

                if (Input.GetMouseButtonDown(0))
                {
                    DefineGamepiece();
                }



                if (Input.GetKeyDown(KeyCode.T)) SpawnGamepiece();
            }
	    }

        private void OnGUI()
        {
            UserMessageManager.Render();
        }

        /// <summary>
        /// If the robot's intake node is touching an gamepiece, make the robot 'intake' it by adding it to the list of held objects and cheats physics by disabling collisions on the gamepiece.
        /// </summary>
        void Intake()
        {
            if (objectsHeld.Count < holdingLimit && intakeInteractor.GetDetected())
            {
                for (int i = 0; i < objectsHeld.Count; i++) if (objectsHeld[i].Equals(intakeInteractor.GetObject())) return; //This makes sure the object the robot is touching isn't an object already being held.
                GameObject newObject = intakeInteractor.GetObject();
                objectsHeld.Add(newObject);
                newObject.GetComponent<BRigidBody>().velocity = UnityEngine.Vector3.zero;
                newObject.GetComponent<BRigidBody>().angularVelocity = UnityEngine.Vector3.zero;
                newObject.GetComponentInChildren<Renderer>().material.color = Color.blue;


                foreach (BRigidBody rb in this.GetComponentsInChildren<BRigidBody>())
                {
                    newObject.GetComponent<BRigidBody>().GetCollisionObject().SetIgnoreCollisionCheck(rb.GetCollisionObject(), true);
                }
            }
        }

        /// <summary>
        /// Binds every gamepiece the robot is holding to the proper node and its position.
        /// </summary>
        private void HoldGamepiece()
        {
            if (objectsHeld.Count > 0)
            {
                BRigidBody rb;
                for (int i = 0; i < objectsHeld.Count; i++)
                {
                  rb = objectsHeld[i].GetComponent<BRigidBody>();
                  rb.velocity = releaseNode.GetComponent<BRigidBody>().velocity;
                  rb.SetPosition(releaseNode.GetComponent<BRigidBody>().transform.position + releaseNode.GetComponent<BRigidBody>().transform.rotation * positionOffset);
                    rb.angularVelocity = UnityEngine.Vector3.zero;
                    rb.angularFactor = UnityEngine.Vector3.zero;

                }
            }
            Debug.DrawLine(releaseNode.transform.position + releaseNode.GetComponent<BRigidBody>().transform.rotation * positionOffset, releaseNode.transform.position+releaseNode.transform.rotation*releaseVelocity);
            DrawTrajectory(releaseNode.transform.position + releaseNode.GetComponent<BRigidBody>().transform.rotation * positionOffset, releaseNode.GetComponent<BRigidBody>().velocity+releaseNode.transform.rotation * releaseVelocity);
        }

        /// <summary>
        /// Releases the gamepiece from the robot at a set velocity
        /// </summary>
        private void ReleaseGamepiece()
        {
            if (objectsHeld.Count > 0)
            {
                objectsHeld[0].GetComponent<BRigidBody>().velocity += releaseNode.transform.rotation * releaseVelocity;
                objectsHeld[0].GetComponent<BRigidBody>().angularFactor = UnityEngine.Vector3.one;
                StartCoroutine(UnIgnoreCollision(objectsHeld[0]));
                objectsHeld.RemoveAt(0);
            }
            Debug.Log(objectsHeld.Count);
        }

        /// <summary>
        /// Waits .5 seconds before renabling collisions between the release gamepiece and the robot.
        /// </summary>
        IEnumerator UnIgnoreCollision(GameObject obj)
        {
            yield return new WaitForSeconds(0.5f);

            foreach (BRigidBody rb in this.GetComponentsInChildren<BRigidBody>())
            {
                obj.GetComponent<BRigidBody>().GetCollisionObject().SetIgnoreCollisionCheck(rb.GetCollisionObject(), false);
            }
        }

        /// <summary>
        /// Illustrates the trajectory a released gamepiece would follow.
        /// Does this by creating rendering lines bounded to several vertices positioned based on multiplying velocity, gravity, and time.
        /// </summary>
        /// <param name="position">starting position of the gamepiece</param>
        /// <param name="velocity">starting velocity of the gamepiece</param>
        void DrawTrajectory(UnityEngine.Vector3 position, UnityEngine.Vector3 velocity)
        {
            int verts = 100; //This determines how far along time the illustration goes.
            LineRenderer line = this.gameObject.GetComponent<LineRenderer>();
            if (line == null)
            {
                line = this.gameObject.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.startColor = Color.blue;
                line.endColor = Color.red;
            }
            line.positionCount = verts;

            UnityEngine.Vector3 pos = position;
            UnityEngine.Vector3 vel = velocity;
            UnityEngine.Vector3 grav = GameObject.Find("BulletPhysicsWorld").GetComponent<BPhysicsWorld>().gravity;
            for (int i = 0; i < verts; i++)
            {
                line.SetPosition(i, pos);
                vel = vel + grav * Time.fixedDeltaTime;
                pos = pos + vel * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// Spawns a gamepiece
        /// </summary>
        void SpawnGamepiece()
        {
            Instantiate(AuxFunctions.FindObject(gamepieces[0]).GetComponentInParent<BRigidBody>().gameObject, new UnityEngine.Vector3(0, 3, 0), UnityEngine.Quaternion.identity);
        }

        #region Configuration

        /// <summary>
        /// Allows the user to select a dynamic object with their mouse and add it to the list of gamepieces.
        /// </summary>
        public void DefineGamepiece()
        {
            //Casts a ray from the camera in the direction the mouse is in and returns the closest object hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            BulletSharp.Math.Vector3 start = ray.origin.ToBullet();
            BulletSharp.Math.Vector3 end = ray.GetPoint(200).ToBullet();
            ClosestRayResultCallback rayResult = new ClosestRayResultCallback(ref start, ref end);

            BPhysicsWorld world = BPhysicsWorld.Get();
            world.world.RayTest(start, end, rayResult);

            //If there is a collision object and it is dynamic, add it to the list of gamepieces
            if (rayResult.CollisionObject != null)
            { 
                if (rayResult.CollisionObject.CollisionFlags != BulletSharp.CollisionFlags.StaticObject)
                {
                    string name = (rayResult.CollisionObject.UserObject.ToString().Replace(" (BulletUnity.BRigidBody)", ""));
                    gamepieces[0] = name;
                    intakeInteractor.SetKeyword(gamepieces[0]);
                    GameObject gamepiece = GameObject.Find(name);
                    gamepiece.GetComponentInChildren<Renderer>().material.color = Color.blue;

                    UserMessageManager.Dispatch(name + " has been selected as the gamepiece", 2);
                }
                else
                {
                    UserMessageManager.Dispatch("The gamepiece must be a dynamic object!", 2);
                }
            }
            else
            {
                
            }
        }

        public void changeOffsetX(float amount)
        {
            positionOffset.x += amount;
        }
        public void changeOffsetY(float amount)
        {
            positionOffset.y += amount;
            Debug.Log(positionOffset);
        }
        public void changeOffsetZ(float amount)
        {
            positionOffset.z += amount;
        }
        public void changeReleaseX(float amount)
        {
            releaseVelocity.x += amount;
        }
        public void changeReleaseY(float amount)
        {
            releaseVelocity.y += amount;
            Debug.Log(releaseVelocity);
        }
        public void changeReleaseZ(float amount)
        {
            releaseVelocity.z += amount;
        }
        #endregion

    }
}