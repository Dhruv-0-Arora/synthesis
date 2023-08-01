using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimObjects.MixAndMatch;
using Synthesis.Import;
using Synthesis.UI;
using Synthesis.UI.Dynamic;
using SynthesisAPI.Utilities;
using UI.Dynamic.Modals.MixAndMatch;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.ColorManager;
using Logger = SynthesisAPI.Utilities.Logger;
using Object  = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace UI.Dynamic.Panels.MixAndMatch {
    public class RobotEditorPanel : PanelDynamic {
        private const float PANEL_WIDTH  = 400f;
        private const float PANEL_HEIGHT = 400f;

        private const float PART_ROTATION_SPEED = 10f;

        private static readonly int _connectionLayer     = LayerMask.NameToLayer("ConnectionPoint");
        private static readonly int _connectionLayerMask = 1 << _connectionLayer;

        private readonly MixAndMatchRobotData _robotData;

        private GameObject _robotGameObject;
        private readonly List<(GameObject gameObject, MixAndMatchPartData partData)> _partGameObjects = new();

        private float _scrollViewWidth;
        private float _entryWidth;

        private ScrollView _scrollView;

        private Button _removeButton;

        private (GameObject gameObject, MixAndMatchPartData partData)? _selectedPart;

        public RobotEditorPanel(MixAndMatchRobotData robotData) : base(new Vector2(PANEL_WIDTH, PANEL_HEIGHT)) {
            _robotData = robotData;
        }

        private bool _creationFailed;
        public override bool Create() {
            Title.SetText("Robot Editor");

            AcceptButton.StepIntoLabel(l => l.SetText("Save"))
                .AddOnClickedEvent(
                    _ => {
                        SaveRobotData();
                        DynamicUIManager.ClosePanel<RobotEditorPanel>();
                    });
            CancelButton.RootGameObject.SetActive(false);

            _scrollView = MainContent.CreateScrollView().SetStretch<ScrollView>(bottomPadding: 60f);

            CreateAddRemoveButtons();

            _robotGameObject = new GameObject(_robotData.Name);

            InstantiatePartGameObjects();
            if (_creationFailed)
                return false;
            
            PopulateScrollView();

            return true;
        }

        private void CreateAddRemoveButtons() {
            (Content left, Content right) = MainContent.CreateSubContent(new Vector2(400, 50))
                                                .SetBottomStretch<Content>()
                                                .SplitLeftRight((PANEL_WIDTH - 10f) / 2f, 10f);

            var addButton = left.CreateButton("Add").SetStretch<Button>().AddOnClickedEvent(
                _ => {
                    DynamicUIManager.CreateModal<SelectPartModal>(
                        args: new Action<MixAndMatchPartData>(AddAdditionalPart));
                });

            _removeButton = right.CreateButton("Remove").SetStretch<Button>().AddOnClickedEvent(
                _ => {
                    if (_selectedPart != null) {
                        _partGameObjects.Remove(_selectedPart.Value);
                        Object.Destroy(_selectedPart.Value.gameObject);
                        _selectedPart = null;
                    }

                    PopulateScrollView();

                    UpdateRemoveButton();
                });
            UpdateRemoveButton();
        }

        private void AddAdditionalPart(MixAndMatchPartData part) {
            EnablePartColliders(_selectedPart?.gameObject);
            AddScrollViewEntry((InstantiatePartGameObject(Vector3.zero, Quaternion.identity, part), part));
            UpdateRemoveButton();
        }

        private void PopulateScrollView() {
            _scrollView.Content.DeleteAllChildren();

            _partGameObjects.ForEach(x => AddScrollViewEntry(x));
        }

        private void InstantiatePartGameObjects() {
            if (_robotData.PartData == null)
                return;

            _robotData.PartData.ForEach(part => { InstantiatePartGameObject(part); });
        }

        private GameObject InstantiatePartGameObject(
            (string fileName, Vector3 localPosition, Quaternion localRotation) partData) {
            return InstantiatePartGameObject(
                partData.localPosition, partData.localRotation, MixAndMatchSaveUtil.LoadPartData(partData.fileName));
        }

        private GameObject InstantiatePartGameObject(
            Vector3 localPosition, Quaternion localRotation, MixAndMatchPartData partData) {
            if (!File.Exists(partData.MirabufPartFile)) {
                Logger.Log($"Part file {partData.MirabufPartFile} not found!", LogLevel.Error);
                _creationFailed = true;
                return null;
            }
            
            MirabufLive miraLive = new MirabufLive(partData.MirabufPartFile);

            GameObject gameObject = new GameObject(partData.Name);

            miraLive.GenerateDefinitionObjects(gameObject, false);

            gameObject.transform.SetParent(_robotGameObject.transform);

            gameObject.transform.position = localPosition;
            gameObject.transform.rotation = localRotation;

            _partGameObjects.Add((gameObject, partData));

            InstantiatePartConnectionPoints(gameObject, partData);
            return gameObject;
        }

        private void InstantiatePartConnectionPoints(GameObject partGameObject, MixAndMatchPartData partData) {
            partData.ConnectionPoints.ForEachIndex((i, point) => {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var trf = obj.transform;
                
                trf.SetParent(partGameObject.transform);
                trf.localPosition = point.LocalPosition;
                trf.localRotation = point.LocalRotation;
                trf.localScale    = Vector3.one * 0.25f;
                
                obj.layer = _connectionLayer;
                obj.name = "Connection Point";

                trf.GetComponent<MeshRenderer>().material.color = ColorManager.GetColor(
                    ColorManager.SynthesisColor.HighlightHover);

                if (trf.TryGetComponent<SphereCollider>(out var collider)) {
                    collider.isTrigger    = true;
                    collider.radius       = 1f;
                }
            });
        }

        private void AddScrollViewEntry((GameObject gameObject, MixAndMatchPartData partData) part) {
            var toggle = _scrollView.Content.CreateToggle(label: part.gameObject.name, radioSelect: true)
                             .SetSize<Toggle>(new Vector2(PANEL_WIDTH, 50f))
                             .ApplyTemplate(Toggle.RadioToggleLayout)
                             .StepIntoLabel(l => l.SetFontSize(16f))
                             .SetDisabledColor(ColorManager.SynthesisColor.Background);
            toggle.AddOnStateChangedEvent((t, s) => { SelectPart(part, t, s); });
        }

        private void SelectPart((GameObject gameObject, MixAndMatchPartData partData) part, Toggle toggle, bool state) {
            if (state) {
                _scrollView.Content.ChildrenReadOnly.OfType<Toggle>().ForEach(x => { x.SetStateWithoutEvents(false); });
                toggle.SetStateWithoutEvents(true);

                _partGameObjects.ForEach(x => EnablePartColliders(x.gameObject));
                DisablePartColliders(part.gameObject);

                _selectedPart = part;
            } else
                _selectedPart = null;

            UpdateRemoveButton();
        }

        private void DeselectSelectedPart() {
            _scrollView.Content.ChildrenReadOnly.OfType<Toggle>().ForEach(x => { x.SetStateWithoutEvents(false); });
            _selectedPart = null;
        }

        private void EnablePartColliders(GameObject part) {
            if (part == null)
                return;

            part.GetComponentsInChildren<SphereCollider>(true).ForEach(c => c.enabled = true);
        }

        private void DisablePartColliders(GameObject part) {
            if (part == null)
                return;

            part.GetComponentsInChildren<SphereCollider>().ForEach(c => c.enabled = false);
        }

        private void UpdateRemoveButton() {
            _removeButton.ApplyTemplate(
                (_partGameObjects.Count > 0 && _selectedPart != null) ? Button.EnableButton : Button.DisableButton);
        }

        // TODO: store separately for each part not globally
        private float _axisRotation;

        private void PartPlacement() {
            if (EventSystem.current.IsPointerOverGameObject() || _selectedPart == null)
                return;

            if (Input.GetMouseButtonDown(0)) {
                DeselectSelectedPart();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, _connectionLayerMask)) {
                var selectedTrf      = _selectedPart.Value.gameObject.transform;
                var selectedPartData = _selectedPart.Value.partData;

                selectedTrf.position = hit.transform.position;

                selectedTrf.rotation = Quaternion.LookRotation(-hit.transform.forward, Vector3.up);
                selectedTrf.Rotate(-selectedPartData.ConnectionPoints[0].LocalRotation.eulerAngles);

                if (Input.GetKey(KeyCode.R)) {
                    _axisRotation += Time.deltaTime * PART_ROTATION_SPEED * (Input.GetKey(KeyCode.LeftShift) ? -1 : 1);
                }

                Vector3 axis = selectedTrf.localToWorldMatrix.rotation *
                               (selectedPartData.ConnectionPoints[0].LocalRotation * Vector3.forward);
                selectedTrf.RotateAround(axis, _axisRotation);

                selectedTrf.Translate(-selectedPartData.ConnectionPoints[0].LocalPosition);
            }
        }

        public override void Update() {
            PartPlacement();
        }

        public override void Delete() {
            Object.Destroy(_robotGameObject);
        }

        private void SaveRobotData() {
            List<(string fileName, Vector3 localPosition, Quaternion localRotation)> parts = new();
            _partGameObjects.ForEach(part => {
                parts.Add(
                    (part.gameObject.name, part.gameObject.transform.position, part.gameObject.transform.rotation));
            });

            _robotData.PartData = parts.ToArray();

            MixAndMatchSaveUtil.SaveRobotData(_robotData);

            RobotSimObject.SpawnRobot(_robotData);
        }
    }
}