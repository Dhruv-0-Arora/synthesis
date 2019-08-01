using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using InventorRobotExporter.GUI.Editors.JointSubEditors;
using InventorRobotExporter.Managers;
using InventorRobotExporter.Utilities;
using InventorRobotExporter.Utilities.ImageFormat;

namespace InventorRobotExporter.GUI.Editors.JointEditor
{
    public sealed partial class MiniJointCard : UserControl
    {
        private readonly JointFormSimple jointForm;
        private readonly RigidNode_Base node;

        private bool isHighlighted;

        public MiniJointCard(RigidNode_Base node, JointFormSimple jointForm, RobotDataManager robotDataManager)
        {
            this.robotDataManager = robotDataManager;
            this.jointForm = jointForm;
            this.node = node;

            InitializeComponent();
            AnalyticsUtils.LogEvent("Joint Editor", "System", "Init");
            WinFormsUtils.DisableScrollSelection(this);

            AddHighlightAction(this);
        }

        public void LoadValues()
        {
            var joint = node.GetSkeletalJoint();
            var typeOptions = JointDriver.GetAllowedDrivers(joint); // TODO: This doesn't protect multi-edit
            var textInfo = new CultureInfo("en-US", true).TextInfo;
            foreach (var type in typeOptions)
            {
                var name = Enum.GetName(typeof(JointDriverType), type);
                if (name != null) // TODO: Get rid of this mess
                    driverTypeComboBox.Items.Add(textInfo.ToTitleCase(name.Replace('_', ' ').ToLowerInvariant()));
            }
            
            jointName.Text = ToStringUtils.NodeNameString(node);
            
            // Defaults when switched:
            weightInput.Value = 0;
            dtSideComboBox.SelectedIndex = 0;
            wheelTypeComboBox.SelectedIndex = 0;
            driverTypeComboBox.SelectedIndex = 0;

            var jointDriver = joint.cDriver;
        
            if (jointDriver == null)
            {
                jointTypeComboBox.SelectedIndex = 0;
            }
            else
            {
                driverTypeComboBox.Items.Clear();

                if (jointDriver.port1 <= 2)
                {
                    // Drivetrain wheel
                    jointTypeComboBox.SelectedIndex = 1;
                    dtSideComboBox.SelectedIndex = jointDriver.port1;
                    var wheelDriverMeta = jointDriver.GetInfo<WheelDriverMeta>();
                    if (wheelDriverMeta != null)
                        wheelTypeComboBox.SelectedIndex = (int) wheelDriverMeta.type - 1;
                }
                else
                {
                    // Mechanism joint
                    jointTypeComboBox.SelectedIndex = 2;
                    weightInput.Value = (decimal) (Math.Max(joint.weight, 0) * 2.20462f); // TODO: Re-use existing weight code
                    driverTypeComboBox.SelectedIndex = Array.IndexOf(typeOptions, joint.cDriver.GetDriveType());
                }
            }
        }

        private void HighlightSelf()
        {
            AnalyticsUtils.LogEvent("Joint Editor", "System", "Highlight");
            if (isHighlighted)
            {
                return;
            }

            jointForm.ResetAllHighlight();
            isHighlighted = true;

            InventorUtils.FocusAndHighlightNode(node, RobotExporterAddInServer.Instance.Application.ActiveView.Camera, 0.8);
        }

        public void LoadPreviewIcon()
        {
            var iconCamera = RobotExporterAddInServer.Instance.Application.TransientObjects.CreateCamera();
            iconCamera.SceneObject = RobotExporterAddInServer.Instance.OpenAssemblyDocument.ComponentDefinition;

            const double zoom = 0.6; // Zoom, where a zoom of 1 makes the camera the size of the whole robot
            
            const int widthConst = 3; // The image needs to be wide to hide the XYZ coordinate labels in the bottom left corner

            var occurrences = InventorUtils.GetComponentOccurrencesFromNodes(new List<RigidNode_Base> {node});
            iconCamera.Fit();
            iconCamera.GetExtents(out _, out var height);

            InventorUtils.SetCameraView(InventorUtils.GetOccurrencesCenter(occurrences), 15, height * zoom * widthConst, height * zoom, iconCamera);


            pictureBox1.Image = AxHostConverter.PictureDispToImage(
                iconCamera.CreateImage(pictureBox1.Height * widthConst, pictureBox1.Height,
                    RobotExporterAddInServer.Instance.Application.TransientObjects.CreateColor(210, 222, 239),
                    RobotExporterAddInServer.Instance.Application.TransientObjects.CreateColor(175, 189, 209)));
        }

        public void ResetHighlight()
        {
            isHighlighted = false;
        }

        private void AddHighlightAction(Control baseControl)
        {
            baseControl.MouseHover += (sender, args) => HighlightSelf();

            foreach (Control control in baseControl.Controls)
                AddHighlightAction(control);
        }
        
        private void JointTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoLayout();
        }

        private void DoLayout()
        {
            switch (jointTypeComboBox.SelectedIndex)
            {
                case 1:
                    InsertDriveTrainControls();
                    return;
                case 2:
                    InsertTableLayoutControls();
                    return;
                default:
                    RemoveMechControls();
                    RemoveDrivetrainControls();
                    return;
            }
        }

        private void InsertTableLayoutControls()
        {

            RemoveDrivetrainControls();
            tableLayoutPanel2.Controls.Add(weightInput, 1, 1);
            tableLayoutPanel2.Controls.Add(driverTypeComboBox, 1, 2);
            tableLayoutPanel2.Controls.Add(weightLabel, 0, 1);
            tableLayoutPanel2.Controls.Add(jointDriverLabel, 0, 2);
        }

        private void RemoveDrivetrainControls()
        {
            tableLayoutPanel2.Controls.Remove(dtSideComboBox);
            tableLayoutPanel2.Controls.Remove(wheelTypeComboBox);
            tableLayoutPanel2.Controls.Remove(sideLabel);
            tableLayoutPanel2.Controls.Remove(wheelTypeLabel);
        }

        private void InsertDriveTrainControls()
        {
            RemoveMechControls();
            tableLayoutPanel2.Controls.Add(dtSideComboBox, 1, 1);
            tableLayoutPanel2.Controls.Add(wheelTypeComboBox, 1, 2);
            tableLayoutPanel2.Controls.Add(sideLabel, 0, 1);
            tableLayoutPanel2.Controls.Add(wheelTypeLabel, 0, 2);
        }

        private void RemoveMechControls()
        {
            tableLayoutPanel2.Controls.Remove(weightInput);
            tableLayoutPanel2.Controls.Remove(driverTypeComboBox);
            tableLayoutPanel2.Controls.Remove(weightLabel);
            tableLayoutPanel2.Controls.Remove(jointDriverLabel);
        }
    }
}