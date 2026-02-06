using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class MVVMFeatureGeneratorWindow : EditorWindow
{
    private string _featureName = "Login";
    private string _baseNamespace = "Match3.Scripts";
    private string _selectedPath = "Assets/Scripts";

    [MenuItem("Tools/MVVM/Generate Feature")]
    public static void ShowWindow()
    {
        GetWindow<MVVMFeatureGeneratorWindow>("MVVM Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("MVVM Feature Generator", EditorStyles.boldLabel);

        GUILayout.Space(5);

        _featureName = EditorGUILayout.TextField("Feature Name", _featureName);
        _baseNamespace = EditorGUILayout.TextField("Base Namespace", _baseNamespace);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Target Folder:");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.SelectableLabel(_selectedPath, GUILayout.Height(18));

        if (GUILayout.Button("Select", GUILayout.Width(80)))
        {
            SelectFolder();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);

        if (GUILayout.Button("Generate Feature", GUILayout.Height(40)))
        {
            GenerateFeature();
        }
    }

    private void SelectFolder()
    {
        string absolutePath = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");

        if (string.IsNullOrEmpty(absolutePath))
            return;

        if (!absolutePath.StartsWith(Application.dataPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder",
                "You must select a folder inside the project's Assets folder.",
                "Ok");
            return;
        }

        _selectedPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
    }

    private void GenerateFeature()
    {
        if (string.IsNullOrEmpty(_featureName))
        {
            Debug.LogError("Feature name is empty");
            return;
        }

        string featureFolder = Path.Combine(_selectedPath, _featureName);
        Directory.CreateDirectory(featureFolder);

        CreateFile(featureFolder, $"{_featureName}Model.cs", GenerateModel());
        CreateFile(featureFolder, $"I{_featureName}UseCase.cs", GenerateUseCaseInterface());
        CreateFile(featureFolder, $"{_featureName}UseCase.cs", GenerateUseCase());
        CreateFile(featureFolder, $"{_featureName}ViewModel.cs", GenerateViewModel());
        CreateFile(featureFolder, $"{_featureName}Presenter.cs", GeneratePresenter());
        CreateFile(featureFolder, $"{_featureName}Controller.cs", GenerateController());
        CreateFile(featureFolder, $"{_featureName}View.cs", GenerateView());

        EnsureViewBaseExists();

        AssetDatabase.Refresh();
    }

    private void CreateFile(string folder, string fileName, string content)
    {
        string fullPath = Path.Combine(folder, fileName);

        if (File.Exists(fullPath))
        {
            Debug.LogWarning($"{fileName} already exists. Skipping.");
            return;
        }

        File.WriteAllText(fullPath, content, Encoding.UTF8);
    }

    private void EnsureViewBaseExists()
    {
        string viewBasePath = Path.Combine(_selectedPath, "ViewBase.cs");

        if (File.Exists(viewBasePath))
            return;

        File.WriteAllText(viewBasePath, GenerateViewBase(), Encoding.UTF8);
    }

    // ===================== TEMPLATES =====================

    private string GenerateModel() =>
$@"namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}Model
    {{
        
    }}
}}";

    private string GenerateUseCaseInterface() =>
$@"namespace {_baseNamespace}.{_featureName}
{{
    public interface I{_featureName}UseCase
    {{
        
    }}
}}";

    private string GenerateUseCase() =>
$@"namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}UseCase : I{_featureName}UseCase
    {{
        
    }}
}}";

    private string GenerateViewModel() =>
$@"using UniRx;

namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}ViewModel
    {{
        
    }}
}}";

    private string GeneratePresenter() =>
$@"using System;

namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}Presenter : IDisposable
    {{
        private readonly {_featureName}ViewModel _viewModel;

        public {_featureName}Presenter({_featureName}ViewModel viewModel)
        {{
            _viewModel = viewModel;
        }}

        public void Dispose()
        {{
        }}
    }}
}}";

    private string GenerateController() =>
$@"using System;
using System.Collections.Generic;

namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}Controller : IDisposable
    {{
        private readonly {_featureName}ViewModel _viewModel;
        private readonly I{_featureName}UseCase _useCase;
        private readonly List<IDisposable> _disposables = new();

        public {_featureName}Controller({_featureName}ViewModel viewModel, I{_featureName}UseCase useCase)
        {{
            _viewModel = viewModel;
            _useCase = useCase;
        }}

        public void Dispose()
        {{
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }}
    }}
}}";

    private string GenerateView() =>
$@"using UnityEngine;

namespace {_baseNamespace}.{_featureName}
{{
    public class {_featureName}View : ViewBase
    {{
        
    }}
}}";

    private string GenerateViewBase() =>
@"using System;
using UniRx;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    protected CompositeDisposable _disposables = new CompositeDisposable();

    protected virtual void OnDestroy()
    {
        _disposables?.Dispose();
    }
}";
}
