using System;
using System.Collections;
using System.Collections.Generic;
using Synthesis.Import;
using System.Linq;
using Synthesis.Gizmo;
using Synthesis.PreferenceManager;
using Synthesis.UI.Dynamic;
using SynthesisAPI.InputManager;
using SynthesisAPI.Simulation;
using SynthesisAPI.Utilities;
using Synthesis.Util;
using UnityEngine;

using Logger = SynthesisAPI.Utilities.Logger;
using Synthesis.UI;
using UnityEngine.SceneManagement;
using Synthesis.Physics;
using SynthesisAPI.EventBus;
using Synthesis.Replay;
using Synthesis.WS;
using SynthesisAPI.Controller;
using SynthesisAPI.RoboRIO;
using UnityEngine.Rendering;

namespace Synthesis.Runtime {
    public class SimulationRunner : MonoBehaviour {
        // clang-format off
        private static uint _simulationContext = 0x00000001;
        // clang-format on
        public static uint SimulationContext => _simulationContext;

        public const uint RUNNING_SIM_CONTEXT = 0x00000001;
        public const uint PAUSED_SIM_CONTEXT  = 0x00000002;
        public const uint REPLAY_SIM_CONTEXT  = 0x00000004;
        public const uint GIZMO_SIM_CONTEXT   = 0x00000008;

        /// <summary>
        /// Called when going to the main menu.
        /// Will be completely reset after called
        /// </summary>
        public static event Action OnSimKill;

        public static event Action OnUpdate;
        public static event Action OnGameObjectDestroyed;

        private static bool _inSim = false;
        public static bool InSim {
            get => _inSim;
            set {
                _inSim = value;
                if (!_inSim)
                    SimKill();
            }
        }

        private bool _setupSceneSwitchEvent = false;

        private void Awake() {
            Synthesis.PreferenceManager.PreferenceManager.Load();
            UnityEngine.Physics.defaultSolverIterations = 20;
        }

        private void Start() {
            InSim = true;

            SetContext(RUNNING_SIM_CONTEXT);
            MainHUD.Setup();
            ModeManager.Start();
            RobotSimObject.Setup();
            WebSocketManager.Init();
            GizmoManager.Setup();

            OnUpdate += ModeManager.Update;
            OnUpdate += () => RobotSimObject.SpawnedRobots.ForEach(r => r.UpdateMultiplayer());

            WebSocketManager.RioState.OnUnrecognizedMessage += s => Debug.Log(s);

            if (ModeManager.CurrentMode is not null)
                ModeManager.CurrentMode.Start();

            SettingsModal.LoadSettings();
            SettingsModal.ApplySettings();
        }

        private void TestColor(Color c) {
            Debug.Log($"{c.r * 255}, {c.g * 255}, {c.b * 255}, {c.a * 255}");
            var hex = c.ToHex();
            Debug.Log(hex);
            var color = hex.ColorToHex();
            Debug.Log($"{color.r * 255}, {color.g * 255}, {color.b * 255}, {color.a * 255}");
        }

        void Update() {
            InputManager.UpdateInputs(_simulationContext);
            SimulationManager.Update();
            DynamicUIManager.Update();

            if (OnUpdate != null) {
                OnUpdate();
            }
        }

        private void FixedUpdate() {
            SimulationManager.FixedUpdate();
            PhysicsManager.FixedUpdate();
        }

        void OnDestroy() {
            MainHUD.Delete();

            Synthesis.PreferenceManager.PreferenceManager.Save();
            MirabufCache.Clear();
            if (OnGameObjectDestroyed != null)
                OnGameObjectDestroyed();
        }

        /// <summary>
        /// Set current context
        /// </summary>
        /// <param name="c">Mask for context</param>
        public static void SetContext(uint c) {
            _simulationContext = c;
        }

        /// <summary>
        /// Add an additional context to the current contexts
        /// </summary>
        /// <param name="c">Mask for context</param>
        public static void AddContext(uint c) {
            _simulationContext |= c;
        }

        /// <summary>
        /// Remove a context from the current context
        /// </summary>
        /// <param name="c">Mask for context</param>
        public static void RemoveContext(uint c) {
            if (HasContext(c))
                _simulationContext ^= c;
        }

        /// <summary>
        /// Check if a context exists within the current context
        /// </summary>
        /// <param name="c">Mask for context</param>
        /// <returns></returns>
        public static bool HasContext(uint c) => (_simulationContext & c) != 0;

        /// <summary>
        /// Teardown sim for recycle
        /// </summary>
        public static void SimKill() {
            ModeManager.Teardown();

            FieldSimObject.DeleteField();
            List<string> robotIDs = new List<string>(RobotSimObject.SpawnedRobots.Count);
            RobotSimObject.SpawnedRobots.ForEach(x => robotIDs.Add(x.Name));
            robotIDs.ForEach(x => RobotSimObject.RemoveRobot(x));
            OrbitCameraMode.FocusPoint = () => Vector3.zero;

            if (OnSimKill != null)
                OnSimKill();

            OnSimKill = null;
            OnUpdate  = null;

            PhysicsManager.Reset();
            ReplayManager.Teardown();
            WebSocketManager.Teardown();
        }
    }
}
