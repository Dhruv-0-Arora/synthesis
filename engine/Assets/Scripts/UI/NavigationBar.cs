using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Synthesis.UI.Tabs;
using Synthesis.UI.Dynamic;

using Image = UnityEngine.UI.Image;

namespace Synthesis.UI.Bars {
    // TODO: Needs a big rework. We'll tackle this with the rest of the UI system later
    public class NavigationBar : MonoBehaviour {
        public GameObject TopButtonContainer;
        public GameObject TopButtonPrefab;

        // TODO: Centralize colors
        public Color SelectedTopButtonColor;
        public Color UnselectedTopButtonColor;

        public TMP_Text VersionNumber;

        public TMP_FontAsset artifaktRegular;
        public TMP_FontAsset artifaktBold;

        private GameObject _currentTabButton;
        private GameObject _currentPanelButton;

        public NavigationBar navBarPrefab;

        private string lastOpenedPanel;

        public GameObject ModalTab;
        [Header("Tab Prefabs"), SerializeField]
        public GameObject TabPanelButtonPrefab;
        [SerializeField]
        public GameObject TabDividerPrefab;

        public static NavigationBar Instance { get; private set; }

        private string _currentTab = string.Empty;
        private Dictionary<string, (TopButton topButton, Tab tab)> _registeredTabs =
            new Dictionary<string, (TopButton topButton, Tab tab)>();

        private void Start() {
            Instance           = this;
            VersionNumber.text = $"v {AutoUpdater.LocalVersion}  ALPHA";

            RegisterTab("Home", new HomeTab());
            RegisterTab("Config", new ConfigTab());
            SelectTab("Home");
        }

        public void Exit() {
            if (Application.isEditor)
                Debug.Log("Would exit, but it's editor mode");
            else {
                DynamicUIManager.CreateModal<ExitSynthesisModal>();
            }
        }

        // Eh?
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                navBarPrefab.CloseAllPanels();
            }
        }

        public void OpenPanel(GameObject prefab) {
            if (prefab != null) {
                LayoutManager.OpenPanel(prefab, true);
                if (_currentPanelButton != null)
                    changePanelButton(artifaktRegular, 1f);

                // set current panel button to the button clicked
                _currentPanelButton = EventSystem.current.currentSelectedGameObject;
                changePanelButton(artifaktBold, 0.6f);

                // Analytics Stuff
                lastOpenedPanel = prefab.name; // this will need to be an array for movable panels
            }
        }

        public void CloseAllPanels() {
            LayoutManager.ClosePanel();
            if (_currentPanelButton != null)
                changePanelButton(artifaktRegular, 1f);
        }

        private void changePanelButton(TMP_FontAsset f, float opacity) {
            // set font
            TextMeshProUGUI text = _currentPanelButton.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.font = f;

            Image img = _currentPanelButton.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, opacity);
        }

        public void RegisterTab(string name, Tab t) {
            var obj       = Instantiate(TopButtonPrefab, TopButtonContainer.transform);
            var topB      = obj.GetComponent<TopButton>();
            topB.Tag.text = name;
            topB.ActualButton.onClick.AddListener(() => { SelectTab(name); });
            _registeredTabs[name] = (topB, t);
        }

        public void SelectTab(string name) {
            if (_currentTab == name)
                return;
            if (_currentTab != string.Empty) {
                var prevTopButton = _registeredTabs[_currentTab].topButton;
                prevTopButton.SetUnderlineColor(UnselectedTopButtonColor);
                prevTopButton.SetUnderlineHeight(1f);
            }
            var currentTopButton = _registeredTabs[name].topButton;
            currentTopButton.SetUnderlineColor(SelectedTopButtonColor);
            currentTopButton.SetUnderlineHeight(2f);
            LayoutManager.OpenTab(_registeredTabs[name].tab);
            _currentTab = name;
        }
    }
}
