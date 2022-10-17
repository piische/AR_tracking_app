using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using ApplicationVariables;

/**
 * State variable -> describes game object current state
 **/
public class ApplicationModel {
    private static ApplicationModel instance;
    private ApplicationState _applicationState = ApplicationState.NONE;
    private ApplicationMode _applicationMode = ApplicationMode.NONE;
    private SopModel _sopModel = null;
    private StepModel _stepModel = null;
    private Dictionary<int, string> _steps = new Dictionary<int, string>();
    private List<MaterialModel> _materials = null;
    private CycleModel _cycleModel = null;
    private Dictionary<string, CycleComponentModel> _cycleModelComponents = new Dictionary<string, CycleComponentModel>();
    private MedicalModel _medicalModel = null;

    public event EventHandler MaterialListChanged;
    public event EventHandler SopChanged;
    public event EventHandler ArContentObjectChanged;
    public event EventHandler StepChanged;
    public event EventHandler ApplicationStateChanged;
    public event EventHandler ApplicationModeChanged;
    public event EventHandler CycleModelChanged;
    public event EventHandler MedicalChanged;

    private ApplicationModel() { }
    public static ApplicationModel Instance
    {
        get {
            if (instance == null) {
                instance = new ApplicationModel();
            }
            return instance;
        }
    }


    public ApplicationState ApplicationState {
        get {
            return _applicationState;
        }
        set {
            if (_applicationState != value) {
                _applicationState = value;
                OnApplicationStateChanged();
            }
        }
    }

    public ApplicationMode ApplicationMode {
        get {
            return _applicationMode;
        }
        set {
            if (_applicationMode != value) {
                _applicationMode = value;
                OnApplicationModeChanged();
            }
        }
    }

    public SopModel SopModel {
        get {
            return _sopModel;
        }
        set {
            if (_sopModel != value) {
                _sopModel = value;
                OnSopChanged();
            }
        }
    }

    public StepModel StepModel {
        get {
            return _stepModel;
        }
        set {
            if (_stepModel != value) {
                _stepModel = value;
                OnStepChanged();
            }
        }
    }

    public Dictionary<int, string> Steps {
        get {
            return _steps;
        }
        set {
            if (_steps != value) {
                _steps = value;

            }
        }
    }

    public List<MaterialModel> MaterialList {
        get {
            return _materials;
        }
        set {
            if (_materials != value) {
                _materials = value;
                OnMaterialListChanged();
            }
        }
    }

    public CycleModel CycleModel {
        get {
            return _cycleModel;
        }
        set {
            if (_cycleModel != value) {
                _cycleModel = value;
                OnCycleModelChanged();
            }
        }
    }

    public Dictionary<string, CycleComponentModel> CycleModelComponents {
        get {
            return _cycleModelComponents;
        }
        set {
            if (_cycleModelComponents != value) {
                _cycleModelComponents = value;
            }
        }
    }

    public MedicalModel MedicalModel {
        get {
            return _medicalModel;
        }
        set {
            if (_medicalModel != value) {
                _medicalModel = value;
                OnMedicalModelChanged();
            }
        }
    }

    protected void OnApplicationStateChanged() {
        ApplicationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnSopChanged() {
        SopChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnArContentObjectChanged()
    {
        ArContentObjectChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnStepChanged() {
        StepChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnMaterialListChanged() {
        MaterialListChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnApplicationModeChanged() {
        ApplicationModeChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnCycleModelChanged() {
        CycleModelChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void OnMedicalModelChanged() {
        MedicalChanged?.Invoke(this, EventArgs.Empty);
    }
}

