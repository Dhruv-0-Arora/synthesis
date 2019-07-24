﻿using System;
using System.Drawing;
using System.Windows.Forms;
using BxDRobotExporter.Exporter;
using BxDRobotExporter.Managers;
using BxDRobotExporter.Properties;

namespace BxDRobotExporter.GUI.Editors
{
    public delegate void SettingsEvent(ExporterSettingsForm.PluginSettingsValues values);

    public partial class ExporterSettingsForm : Form
    {
        /// <summary>
        /// The local copy of the setting values
        /// </summary>
        private static PluginSettingsValues Values = new PluginSettingsValues();

        public ExporterSettingsForm()
        {
            InitializeComponent();

            LoadValues();

            buttonOK.Click += delegate(object sender, EventArgs e)
            {
                SaveValues();
                Close();
            };

            buttonCancel.Click += delegate(object sender, EventArgs e) { Close(); };
        }

        /// <summary>
        /// Load values into the form
        /// </summary>
        private void LoadValues()
        {
            Values = RobotDataManager.PluginSettings;

            ChildHighlight.BackColor = Values.InventorChildColor;
            checkBox1.Checked = Values.UseAnalytics;
        }

        /// <summary>
        /// Save the form's values in a <see cref="PluginSettingsValues"/> structure
        /// </summary>
        private void SaveValues()
        {
            Values.InventorChildColor = ChildHighlight.BackColor;
            Values.UseAnalytics = checkBox1.Checked;
            Values.OnSettingsChanged();
        }

        /// <summary>
        /// Get the default values for the <see cref="PluginSettingsValues"/> structure
        /// </summary>
        /// <returns>Default values for the <see cref="Exporter"/></returns>
        public static PluginSettingsValues GetDefaultSettings()
        {
            return new PluginSettingsValues()
            {
                InventorChildColor = Color.FromArgb(255, 0, 125, 255),
                GeneralUseFancyColors = true,
                GeneralSaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Synthesis\Robots",
                UseAnalytics = true
            };
        }

        /// <summary>
        /// The struct that stores settings for the <see cref="Exporter"/>
        /// </summary>
        public class PluginSettingsValues
        {
            public PluginSettingsValues()
            {
                LoadSettings();
            }

            public event SettingsEvent SettingsChanged;

            internal void OnSettingsChanged()
            {
                SaveSettings();
                if (SettingsChanged != null) SettingsChanged.Invoke(this);
            }

            private void SaveSettings()
            {
                Settings.Default.ExportToField = OpenSynthesis;
                Settings.Default.SelectedField = FieldName;
                Settings.Default.ChildColor = InventorChildColor;
                Settings.Default.FancyColors = GeneralUseFancyColors;
                Settings.Default.SaveLocation = GeneralSaveLocation;
                Settings.Default.DefaultRobotCompetition = DefaultRobotCompetition;
                Settings.Default.UseAnalytics = UseAnalytics;
                Settings.Default.ConfigVersion = 3; // Update this config version number when changes are made to the exporter which require settings to be reset or changed when the exporter starts
                Settings.Default.Save();
            }

            /// <summary>
            /// Initializes all of the <see cref="Managers.RobotDataManager"/> settings to the proper values. Should be called once in the Activate class
            /// </summary>
            public void LoadSettings()
            {
                // Old configurations get overriden (version numbers below 1)
                if (Settings.Default.SaveLocation == "" || Settings.Default.SaveLocation == "firstRun" || Settings.Default.ConfigVersion < 2)
                    Settings.Default.SaveLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Synthesis\Robots";

                if (Settings.Default.ConfigVersion < 3)
                {
                    InventorChildColor = Settings.Default.ChildColor;
                    GeneralSaveLocation = Settings.Default.SaveLocation;
                    GeneralUseFancyColors = Settings.Default.FancyColors;
                    OpenSynthesis = Settings.Default.ExportToField;
                    FieldName = Settings.Default.SelectedField;
                    DefaultRobotCompetition = "GENERIC";
                    UseAnalytics = true;
                }
                else
                {
                    InventorChildColor = Settings.Default.ChildColor;
                    GeneralSaveLocation = Settings.Default.SaveLocation;
                    GeneralUseFancyColors = Settings.Default.FancyColors;
                    OpenSynthesis = Settings.Default.ExportToField;
                    FieldName = Settings.Default.SelectedField;
                    DefaultRobotCompetition = Settings.Default.DefaultRobotCompetition;
                    UseAnalytics = Settings.Default.UseAnalytics;
                }
            }

            //General
            public string GeneralSaveLocation;
            public bool GeneralUseFancyColors;
            public string FieldName;
            public string DefaultRobotCompetition;
            public bool OpenSynthesis;

            public bool UseAnalytics;

            //Inventor
            public Color InventorChildColor;
        }

        /// <summary>
        /// Sets the <see cref="Color"/> of the <see cref="Button"/> and by extension the <see cref="PluginSettingsValues"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildHighlight_Click(object sender, EventArgs e)
        {
            ColorDialog colorChoose = new ColorDialog();
            if (colorChoose.ShowDialog() == DialogResult.OK)
            {
                ChildHighlight.BackColor = colorChoose.Color;
            }
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            if (folderBrowser.SelectedPath != null)
            {
                Values.GeneralSaveLocation = folderBrowser.SelectedPath;
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Values.UseAnalytics = checkBox1.Checked;
        }
    }
}