using System;
using NWH.VehiclePhysics2.Modules.ABS;
using NWH.VehiclePhysics2.Modules.Aerodynamics;
using NWH.VehiclePhysics2.Modules.ESC;
using NWH.VehiclePhysics2.Modules.FlipOver;
using NWH.VehiclePhysics2.Modules.TCS;
using NWH.VehiclePhysics2.Modules.Trailer;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    /// Written only for demo purposes.
    /// Messy code ahead - you have been warned!
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class DemoGUIController : MonoBehaviour
    {
        public static Color disabledColor = new Color32(66, 66, 66, 255);
        public static Color enabledColor = new Color32(76,175,80, 255);

        public Text promptText;

        public GameObject helpWindow;
        public GameObject settingsWindow;
        public GameObject telemetryWindow;

        public Button absButton;
        public Button tcsButton;
        public Button escButton;
        public Button aeroButton;
        public Button damageButton;
        public Button repairButton;
        
        public Button resetButton;
        public Button helpButton;
        public Button settingsButton;
        public Button telemetryButton;
        
        public Slider throttleSlider;
        public Slider brakeSlider;
        public Slider clutchSlider;
        public Slider handbrakeSlider;
        public Slider horizontalLeftSlider;
        public Slider horizontalRightSlider;

        public Slider damageSlider;

        private VehicleController _vc;
        private VehicleController _prevVc;

        private TrailerHitchModule _trailerHitchModule;
        private FlipOverModule _flipOverModule;
        private ABSModule _absModule;
        private TCSModule _tcsModule;
        private ESCModule _escModule;
        private AerodynamicsModule _aeroModule;

        private ColorBlock _colorBlock;
        private Canvas _canvas;

        private void Start()
        {
            absButton.onClick.AddListener(ToggleABS);
            tcsButton.onClick.AddListener(ToggleTCS);
            escButton.onClick.AddListener(ToggleESC);
            aeroButton.onClick.AddListener(ToggleAero);
            damageButton.onClick.AddListener(ToggleDamage);
            repairButton.onClick.AddListener(RepairDamage);
            
            resetButton.onClick.AddListener(ResetScene);
            helpButton.onClick.AddListener(ToggleHelpWindow);
            telemetryButton.onClick.AddListener(ToggleTelemetryWindow);
            settingsButton.onClick.AddListener(ToggleSettingsWindow);

            _canvas = GetComponent<Canvas>();
        }

        void Update()
        {
            _vc = VehicleChanger.ActiveVehicleController;

            promptText.text = "";

            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                _canvas.enabled = !_canvas.enabled;
            }
            
            if(CharacterVehicleChanger.Instance != null && CharacterVehicleChanger.Instance.nearVehicle)
            {
                promptText.text = "Press V to enter the vehicle.";
            }
            
            if (_vc == null)
            {
                return;
            }
            
            if (_vc != _prevVc)
            {
                _trailerHitchModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<TrailerHitchModule>();
                _flipOverModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<FlipOverModule>();
                _absModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<ABSModule>();
                _tcsModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<TCSModule>();
                _escModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<ESCModule>();
                _aeroModule = VehicleChanger.ActiveVehicleController.moduleManager.GetModule<AerodynamicsModule>();
            }

            throttleSlider.value = Mathf.Clamp01(_vc.input.states.vertical);
            brakeSlider.value = Mathf.Clamp01(-_vc.input.states.vertical);
            clutchSlider.value = Mathf.Clamp01(_vc.powertrain.clutch.clutchEngagement);
            handbrakeSlider.value = Mathf.Clamp01(_vc.input.states.handbrake);
            horizontalLeftSlider.value = Mathf.Clamp01(-_vc.input.Horizontal);
            horizontalRightSlider.value = Mathf.Clamp01(_vc.input.Horizontal);
            
            if(_trailerHitchModule != null && _trailerHitchModule.trailerInRange && !_trailerHitchModule.attached)
            {
                promptText.text = "Press T to attach the trailer.";
            }

            if(_flipOverModule != null && _flipOverModule.manual && _flipOverModule.flippedOver)
            {
                promptText.text = "Press P to recover the vehicle.";
            }

            if(_absModule != null)
            {
                absButton.targetGraphic.color = _absModule.IsEnabled ? enabledColor : disabledColor;
            }

            if(_tcsModule != null)
            {
                tcsButton.targetGraphic.color = _tcsModule.IsEnabled ? enabledColor : disabledColor;
            }

            if(_escModule != null)
            {
                escButton.targetGraphic.color = _escModule.IsEnabled ? enabledColor : disabledColor;
            }

            if (_aeroModule != null)
            {
                aeroButton.targetGraphic.color = _aeroModule.IsEnabled ? enabledColor : disabledColor;
            }

            damageButton.targetGraphic.color = _vc.damageHandler.Active ? enabledColor : disabledColor;
            damageSlider.value = _vc.damageHandler.Damage;

            _prevVc = _vc;
        }

        public void ToggleDamage()
        {
            _vc.damageHandler.LodIndex = -1;
            _vc.damageHandler.ToggleState();
        }

        public void RepairDamage()
        {
            if (_vc != null && _vc.damageHandler.Active)
            {
                _vc.damageHandler.Repair();
            }
        }

        public void ToggleAero()
        {
            if (_aeroModule != null)
            {
                _aeroModule.LodIndex = -1;
                _aeroModule.ToggleState();
            }
        }
        
        public void ToggleABS()
        {
            if (_absModule != null)
            {
                _absModule.LodIndex = -1;
                _absModule.ToggleState();
            }
        }

        public void ToggleTCS()
        {
            if (_tcsModule != null)
            {
                _tcsModule.LodIndex = -1;
                _tcsModule.ToggleState();
            }
        }

        public void ToggleESC()
        {
            if (_escModule != null)
            {
                _escModule.LodIndex = -1;
                _escModule.ToggleState();
            }
        }

        public void ToggleHelpWindow()
        {
            helpWindow.SetActive(!helpWindow.activeInHierarchy);
            settingsWindow.SetActive(false);
            telemetryWindow.SetActive(false);
        }

        public void ToggleSettingsWindow()
        {
            settingsWindow.SetActive(!settingsWindow.activeInHierarchy);
            helpWindow.SetActive(false);
            telemetryWindow.SetActive(false);
        }

        public void ToggleTelemetryWindow()
        {
            telemetryWindow.SetActive(!telemetryWindow.activeInHierarchy);
            settingsWindow.SetActive(false);
            helpWindow.SetActive(false);
        }

        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

