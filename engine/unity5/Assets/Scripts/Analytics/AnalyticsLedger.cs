﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsLedger
{

    public static int getMilliseconds()
    {
        return (int)(Time.unscaledTime * 1000);
    }

    public class EventCatagory {
        public const string

            // Main Menu scene and events
            MainSimMenu = "simMenu",
            MixAndMatchMenu = "mixMenu",
            SettingsMenu = "settingsMenu",
            MultiplayerMenu = "multiplayerMenu",
            MixAndMatchSimulator = "mixSimulator",

            // Activated scenes 
            MainSimulator = "mainSimulator",
            Tutorials = "tutorialLink",
            //MultiplayerSimulator = "multiplayerSimulator", // network multiplayer temporarily disabled.

            // Toolbar tabs
            ExitTab = "exitTab",
            HomeTab = "homeTab",
            DPMTab = "driverPracticeTab",
            ScoringTab = "scoringTab",
            SensorTab = "sensorTab",
            EmulationTab = "emulationTab",

            // Main toolbar scene events
            AddRobot = "addRobot",
            ChangeRobot = "changeRobot",
            MaMRobot = "mixandMatchRobot",  // changed to MaM robot
            ExportedRobot = "exportedRobot", // changed to pre-exported robot
            ChangeField = "changeField",

            ResetDropdown = "resetDropdown",
            ResetRobot = "resetRobot",
            ResetSpawnpoint = "resetSpawnpoint",
            ResetField = "resetField",

            CameraDropdown = "camDropdown",
            DriverView = "driverStationView",
            OrbitView = "orbitView",
            FreeroamView = "freeRoamView",
            Overview = "overviewView",

            ReplayMode = "replayMode",
            Multiplayer = "multiplayer",
            Stopwatch = "stopWatch",
            Ruler = "ruler",
            ControlPanel = "controlPanel",
            MainHelp = "mainToolbarHelp",

            // DPM toolbar events
            GamepieceDropdown = "dpm_changeGamepiece",
            DefineIntake = "dpm_defineIntake",
            DefineRelease = "dpm_defineRelease",
            EditTrajectory = "dpm_editTrajectory",
            SetSpawnpoint = "dpm_setSpawnpoint",
            SpawnGamepiece = "dpm_spawnGamepiece",
            ClearGamepiece = "dpm_clearGamepiece",
            DPMHelp = "dpm_Help",

            // Scoring toolbar events
            ScoreZones = "scoreZone",
            ScoreBoard = "scoreBoard",
            ScoreHelp = "scoreHelp",

            // Sensor toolbar events
            RobotCamera = "robotCamera",

            UltrasonicDropdown = "ultraSensorDropdown",
            AddUltrasonic = "ultraSensorAdd",
            EditUltrasonic = "ultraSensorEdit",

            BeamBreakDropdown = "beamBreakDropdown",
            AddBeam = "beamBreakSensorAdd",
            EditBeam = "beamBreakSensorEdit",

            GyroDropdown = "gyroDropdown",
            AddGyro = "gyroSensorAdd",
            EditGyro = "gyroSensorEdit",

            SensorNode = "sensorNode",
            SensorAngle = "sensorAngle",
            SensorRange = "sensorRange",
            SensorPosition = "sensorPostion",
            SensorHide = "sensorHide",
            DeleteSensor = "sensorDelete",

            HideOutputs = "sensorHideOutputs",
            SensorHelp = "sensorHelp",

            // Emulation toolbar events
            SelectCode = "emulationSelectCode",
            DriverStation = "emulationDriverStation",
            RunCode = "emulationRunCode",
            EmulationHelp = "emulationHelp";

    }

    public class EventAction
    {
        public const string
            Start = "start",
            TutorialRequest = "requestedTutorial",
            Saved = "saved",
            BackedOut = "backedOut",
            Continued = "continued",
            Clicked = "buttonClicked",
            Viewed = "viewed",
            Changed = "changed";
    }

    public class PageView
    {
        public const string
            MainSimMenu = "simMenu",
            MixAndMatchMenu = "mixMenu",
            MultiplayerMenu = "multiplayerMenu",
            MainSimulator = "mainSimulator",
            MixAndMatchSimulator = "mixSimulator",
            MultiplayerSimulator = "multiplayerSimulator";
    }

    public class TimingCatagory
    {
        public const string
            Main = "main",
            MixMatch = "mixAndMatch",
            Multiplater = "multiplayer",

            MainSimulator = "inSimulator",
            HomeTab = "homeTab",
            DPMTab = "dpmTab",
            ScoringTab = "scoringTab",
            SensorTab = "sensorTab",
            EmulationTab = "emulationTab",
            Tab = "toolbarTab";
    }

    public class TimingVarible
    {
        public const string
            Loading = "loading",
            Playing = "playing",
            Customizing = "customizing",
            Viewing = "viewing",
            Starting = "starting";
    }

    public class TimingLabel
    {
        public const string
            MixAndMatchMenu = "mixMenu",
            MainSimMenu = "mainSimMenu",
            MultiplayerLobbyMenu = "multiplayerLobbyMenu",

            MainSimulator = "mainSimulator",
            ResetField = "resetField",
            ChangeField = "changedField",
            MixAndMatch = "mixAndMatchMode",
            ReplayMode = "replayMode",

            HomeTab = "homeTab",
            DPMTab = "dpmTab",
            ScoringTab = "scoringTab",
            SensorTab = "sensorTab",
            EmulationTab = "emulationTab";
    }
}
