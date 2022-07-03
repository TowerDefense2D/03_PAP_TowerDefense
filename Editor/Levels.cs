using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

/// <summary>
/// Janela do editor de níveis do jogo
/// </summary>
public class TD2D_Levels : EditorWindow
{
	// Principais estados
	private enum MyState
	{
		Disabled,
		LevelDescription,
		LevelMap
	}
	// Subestados
	private enum MySubState
	{
		MapFolder,
		BuildingPlace,
		TowersFolder,
		SpawnPoint,
		Pathway,
		PathwaysFolder,
		EnemiesFolder,
		LevelChooserFolder,
		LevelDescription
	}
	// Camadas entre editor e scripts de jogo
	private class Inspectors
	{
		public MapFolderInspector mapFolder;
		public TowersFolderInspector towersFolder;
		public BuildingPlaceInspector buildingPlace;
		public PathwaysFolderInspector pathwaysFolder;
		public PathwayInspector pathway;
		public SpawnPointInspector spawnPoint;
		public LevelManagerInspector levelManager;
		public WavesInfoInspector wavesInfo;
		public LevelChooseInspector levelChooser;
		public LevelDescriptionInspector levelDescription;
		public DataManagerInspector dataManager;

		/// <summary>
		/// Limpa os inspetores temporários.
		/// </summary>
		public void ClearTemporary()
		{
			buildingPlace = null;
			pathway = null;
			spawnPoint = null;
			levelDescription = null;
		}
	}
	// Conteúdo visual para partes do editor
	private class Contents
	{
		public GUIContent levelsFolder;
		public GUIContent mapFolder;
		public GUIContent towersFolder;
		public GUIContent pathwaysFolder;
		public GUIContent conditionsFolder;
		public GUIContent newLevel;
		public GUIContent chooseMap;
		public GUIContent spawnIcon;
		public GUIContent captureIcon;
		public GUIContent addTower;
		public GUIContent buildingPlace;
		public GUIContent defendPoint;
		public GUIContent tower;
		public GUIContent addPathway;
		public GUIContent capturePoint;
		public GUIContent spawnPoint;
		public GUIContent waypoint;
		public GUIContent levelIcon;
		public GUIContent next;
		public GUIContent prev;
		public GUIContent add;
		public GUIContent remove;
		public GUIContent resetProgress;
		public GUIContent openAllLevels;
	}
	// Estados da janela do selecionador
	private enum PickerState
	{
		None,
		Map,
		SpawnIcon,
		CaptureIcon,
		Tower,
		Enemy,
		LevelIcon
	}
	// Legendas que ajudam a organizar a pesquisa de prefabs
	private class Lables
	{
		public const string map = "l:Map";
		public const string spawnIcon = "l:SpawnIcon";
		public const string captureIcon = "l:CaptureIcon";
		public const string tower = "l:Tower";
		public const string levelIcon = "l:LevelIcon";
	}
	// Tema visual do editor
	private GUISkin editorGuiSkin;
	// Estado principal
	private MyState myState = MyState.Disabled;
	// Lista de subestados
	private List<MySubState> mySubState = new List<MySubState>();
	// Inspetores
	private Inspectors inspectors = new Inspectors();
	// Conteúdo visual
	private Contents contents = new Contents();
	// Estado da janela do seletor
	private PickerState pickerState = PickerState.None;
	// ID da janela do selecionador
	private int currentPickerWindow;
	// O objeto é selecionado na janela do selecionador
	private GameObject pickedGameObject;
	// Nome do nível ativo
	private string currentLevelName;
	// Apenas algum espaço livre na janela do editor
	private float guiSpace = 15f;
	// Inimigos permitidos para este nível
	private List<bool> enemiesList = new List<bool>();
	// Upgrades da torre permitidos para este nível
	private List<bool> towersList = new List<bool>();
	// Feitiços permitidos para este nível
	private List<bool> spellsList = new List<bool>();
	// Posição do scroll para a lista de inimigos
	private Vector2 enemiesScrollPos;
	// Posição do scroll para lista de atualizações de torre
	private Vector2 towersScrollPos;
	// Posição do scroll para lista de feitiços
	private Vector2 spellsScrollPos;
	// Posição do scroll para a lista de tempos limite de ondas
	private Vector2 wavesScrollPos;
	// Altura normal do botão
	private int buttonHeight = 30;
	// Largura normal do botão
	private int buttonWidth = 30;

	[MenuItem("Window/TD2D/Levels")]

	/// <summary>
	/// Mostra a janela.
	/// </summary>
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TD2D_Levels));
	}

	/// <summary>
	/// Ativa o evento enable.
	/// </summary>
	void OnEnable()
	{
		EditorSceneManager.sceneOpened += OnSceneOpened;
		Selection.selectionChanged += OnSelectionChanged;
		EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
		UpdateStuff();
	}

	/// <summary>
	/// Ativa o evento disable.
	/// </summary>
	void OnDisable()
	{
		EditorSceneManager.sceneOpened -= OnSceneOpened;
		Selection.selectionChanged -= OnSelectionChanged;
		EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
	}

	/// <summary>
	/// Levanta a cena aberta do evento.
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="openSceneMode">Open modo de cena.</param>
	private void OnSceneOpened(Scene scene, OpenSceneMode openSceneMode)
	{
		UpdateStuff();
	}

	/// <summary>
	/// Aumenta o evento alterado de estado do modo de jogo.
	/// </summary>
	private void OnPlaymodeStateChanged(UnityEditor.PlayModeStateChange playModeStateChange)
	{
		if (playModeStateChange == PlayModeStateChange.EnteredEditMode || playModeStateChange == PlayModeStateChange.ExitingPlayMode)
		{
			UpdateStuff();
		}
	}

	/// <summary>
	/// Ativa o evento de seleção alterada.
	/// </summary>
	private void OnSelectionChanged()
	{
		inspectors.ClearTemporary();
		mySubState.Clear();
		// Verifica se há objetos de jogo selecionados
		if (Selection.GetFiltered<GameObject>(SelectionMode.ExcludePrefab).Length == 1)
		{
			// Atualiza a lista de inspetores dependendo do gameobject selecionado
			switch (myState)
			{
			case MyState.LevelMap:
				if ((Selection.activeObject as GameObject).GetComponentInParent<MapFolderInspector>() != null)
				{
					mySubState.Add(MySubState.MapFolder);
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<BuildingPlaceInspector>() != null)
				{
					mySubState.Add(MySubState.BuildingPlace);
					inspectors.buildingPlace = (Selection.activeObject as GameObject).GetComponentInParent<BuildingPlaceInspector>();
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<TowersFolderInspector>() != null)
				{
					mySubState.Add(MySubState.TowersFolder);
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<SpawnPointInspector>() != null)
				{
					mySubState.Add(MySubState.SpawnPoint);
					inspectors.spawnPoint = (Selection.activeObject as GameObject).GetComponentInParent<SpawnPointInspector>();
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<PathwayInspector>() != null)
				{
					mySubState.Add(MySubState.Pathway);
					inspectors.pathway = (Selection.activeObject as GameObject).GetComponentInParent<PathwayInspector>();
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<PathwaysFolderInspector>() != null)
				{
					mySubState.Add(MySubState.PathwaysFolder);
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<LevelManagerInspector>() != null)
				{
					mySubState.Add(MySubState.EnemiesFolder);
				}
				break;
			case MyState.LevelDescription:
				if ((Selection.activeObject as GameObject).GetComponentInParent<LevelDescriptionInspector>() != null)
				{
					mySubState.Add(MySubState.LevelDescription);
					inspectors.levelDescription = (Selection.activeObject as GameObject).GetComponentInParent<LevelDescriptionInspector>();
					contents.levelIcon = new GUIContent(inspectors.levelDescription.icon.sprite.texture, "Choose icon for this level");
				}
				if ((Selection.activeObject as GameObject).GetComponentInParent<LevelChooseInspector>() != null)
				{
					mySubState.Add(MySubState.LevelChooserFolder);
				}
				break;
			}
		}
	}

	/// <summary>
	/// Atualiza os dados do material.
	/// </summary>
	private void UpdateStuff()
	{
		// Define tema visual do editor
		editorGuiSkin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/EditorGuiSkin.guiskin", typeof(GUISkin));
		// Definir rótulos para prefaas. O rótulo será usado na janela do selecionador
		string[] prefabs;
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Map/LevelMaps"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.map});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Map/SpawnIcons"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.spawnIcon});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Map/CaptureIcons"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.captureIcon});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Units/Towers/Towers"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.tower});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Levels/Icons"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.levelIcon});
		}

		// Procura inspetores básicos
		inspectors.mapFolder = GameObject.FindObjectOfType<MapFolderInspector>();
		inspectors.towersFolder = GameObject.FindObjectOfType<TowersFolderInspector>();
		inspectors.pathwaysFolder = GameObject.FindObjectOfType<PathwaysFolderInspector>();
		inspectors.wavesInfo = GameObject.FindObjectOfType<WavesInfoInspector>();
		inspectors.levelManager = GameObject.FindObjectOfType<LevelManagerInspector>();
		inspectors.levelChooser = GameObject.FindObjectOfType<LevelChooseInspector>();
		inspectors.dataManager = GameObject.FindObjectOfType<DataManagerInspector>();

		// Define o conteúdo visual
		contents.levelsFolder = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Levels.png", typeof(Texture)), "Create new levels");
		contents.mapFolder = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Map.png", typeof(Texture)), "Game map visual settings");
		contents.towersFolder = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/BuildingPlace.png", typeof(Texture)), "Towers placement and settings");
		contents.pathwaysFolder = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Pathway.png", typeof(Texture)), "Enemies pathways creating");
		contents.conditionsFolder = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Conditions.png", typeof(Texture)), "Level conditions settings");
		contents.newLevel = new GUIContent("Create new level", (Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/CreateNewLevel.png", typeof(Texture)), "Create new scene with tower defense map and open it");
		contents.chooseMap = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/ChooseMap.png", typeof(Texture)), "Load game map");
		contents.spawnIcon = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddSpawnIcon.png", typeof(Texture)), "Add spawn place icon to level map");
		contents.captureIcon = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddCaptureIcon.png", typeof(Texture)), "Add capture place icon to level map");
		contents.addTower = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddTower.png", typeof(Texture)), "Add tower building place. The tower type may be choosen after that");
		contents.buildingPlace = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/BuildingPlace.png", typeof(Texture)), "Focus on building place of this tower");
		contents.defendPoint = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/DefendPoint.png", typeof(Texture)), "Focus on defend point of this tower");
		contents.tower = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Tower.png", typeof(Texture)), "Choose tower type");
		contents.addPathway = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddPathway.png", typeof(Texture)), "Add pathway for enemy waves. It consist of spawn point and waypoints. Dublicate waypoints to create a path");
		contents.capturePoint = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddCapturePoint.png", typeof(Texture)), "Add capture point. The pathways must end inside capture point area");
		contents.spawnPoint = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/SpawnPoint.png", typeof(Texture)), "Focus on spawn point of this pathway");
		contents.waypoint = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Waypoint.png", typeof(Texture)), "Add waypoint to this pathway");
		contents.next = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Next.png", typeof(Texture)));
		contents.prev = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Prev.png", typeof(Texture)));
		contents.add = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Add.png", typeof(Texture)));
		contents.remove = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Remove.png", typeof(Texture)));
		contents.resetProgress = new GUIContent("Reset game progress", "Delete saved file with \"in game\" completed levels data");
		contents.openAllLevels = new GUIContent("Permit all levels", "Make all game levels opened while in level chooser menu");

		// Verifica que tipo de cena abriu agora
		if (inspectors.mapFolder != null && inspectors.towersFolder != null && inspectors.pathwaysFolder != null && inspectors.levelManager != null && inspectors.wavesInfo != null)
		{
			// Esta é a cena do mapa do nível

			myState = MyState.LevelMap;
			string sceneName = SceneManager.GetActiveScene().name;
			currentLevelName = sceneName;
			// Foca a câmera no mapa de nível
			GameObject levelMap = GameObject.Find("LevelMap");
			if (levelMap != null)
			{
				CameraFocus(levelMap);
			}

			// Atualiza listas de geradores de nível para as pastas especificadas
			inspectors.levelManager.enemiesList.Clear();
			prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/TD2D/Prefabs/Units/Enemies/Units"});
			foreach (string prefab in prefabs)
			{
				inspectors.levelManager.enemiesList.Add((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab), typeof(GameObject)));
			}
			inspectors.levelManager.towersList.Clear();
			prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/TD2D/Prefabs/Units/Towers/Towers"});
			foreach (string prefab in prefabs)
			{
				inspectors.levelManager.towersList.Add((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab), typeof(GameObject)));
			}
			inspectors.levelManager.spellsList.Clear();
			prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {"Assets/TD2D/Prefabs/Spells/Spells"});
			foreach (string prefab in prefabs)
			{
				inspectors.levelManager.spellsList.Add((GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab), typeof(GameObject)));
			}

			// Obtém uma lista de inimigos aleatórios
			enemiesList.Clear();
			for (int i = 0; i < inspectors.levelManager.enemiesList.Count; ++i)
			{
				if (inspectors.levelManager.enemies.Contains(inspectors.levelManager.enemiesList[i]) == true)
				{
					enemiesList.Add(true);
				}
				else
				{
					enemiesList.Add(false);
				}
			}
			// Obtém lista de atualizações das torres
			towersList.Clear();
			for (int i = 0; i < inspectors.levelManager.towersList.Count; ++i)
			{
				if (inspectors.levelManager.towers.Contains(inspectors.levelManager.towersList[i]) == true)
				{
					towersList.Add(true);
				}
				else
				{
					towersList.Add(false);
				}
			}
			// Obtém a lista de feitiços
			spellsList.Clear();
			for (int i = 0; i < inspectors.levelManager.spellsList.Count; ++i)
			{
				if (inspectors.levelManager.spells.Contains(inspectors.levelManager.spellsList[i]) == true)
				{
					spellsList.Add(true);
				}
				else
				{
					spellsList.Add(false);
				}
			}
		}
		else
		{
			if (inspectors.levelChooser != null)
			{
				// Esta é uma cena para a descrição do nível

				myState = MyState.LevelDescription;
				// Exibe descrição do nível ativo
				GameObject levelDescriptionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TD2D/Prefabs/Levels/" + currentLevelName +".prefab");
				if (levelDescriptionPrefab != null)
				{
					inspectors.levelChooser.AddLevel(levelDescriptionPrefab);
					GameObject levelDescription = PrefabUtility.InstantiatePrefab(levelDescriptionPrefab) as GameObject;
					inspectors.levelChooser.SetActiveLevel(levelDescription);
				}
				LevelDescriptionInspector levelDescriptionInspector = GameObject.FindObjectOfType<LevelDescriptionInspector>();
				if (levelDescriptionInspector != null)
				{
					// Foca na descrição do nível
					Selection.activeObject = levelDescriptionInspector.gameObject;
					CameraFocus(levelDescriptionInspector.gameObject);
				}
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
			else
			{
				// ATENÇAO CARLOS, isto não é uma cena do nível Tower Defense

				myState = MyState.Disabled;
				currentLevelName = "";
			}
		}

		OnSelectionChanged();
	}

	/// <summary>
	/// Mostra a janela do selecionador.
	/// </summary>
	/// <param name="state">State.</param>
	/// <param name="filter">Filter.</param>
	private void ShowPicker(PickerState state, string filter)
	{
		pickerState = state;
		// Cria um ID de controle do seletor de janela
		currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive);
		// Usa o ID que você acabou de criar
		EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, filter, currentPickerWindow);
	}

	/// <summary>
	/// Foca a câmera no gameobject.
	/// </summary>
	/// <param name="focus">Focus.</param>
	private void CameraFocus(GameObject focus)
	{
		SceneView sceneView = SceneView.lastActiveSceneView;
		if (sceneView != null)
		{
			Object selected = Selection.activeObject;
			Selection.activeObject = focus;
			sceneView.FrameSelected();
			Selection.activeObject = selected;
		}
	}

	/// <summary>
	/// Ativa o evento de atualização do inspetor.
	/// </summary>
	void Update()
	{
		Repaint();
	}

	/// <summary>
	/// Ativa o GU event.
	/// </summary>
	void OnGUI()
	{
		// Define tema visual
		GUI.skin = editorGuiSkin;

		if (EditorApplication.isPlaying == false)
		{
			// Exibe uma caixa de ajuda se a cena do modelo de nível for escolhido
			if (myState == MyState.LevelMap)
			{
				if (currentLevelName == "Level0")
				{
					EditorGUILayout.HelpBox("Active scene is a level map template", MessageType.Info);
				}
			}

			// Exibe GUI dependendo do objeto ativo
			EditorGUI();
			LevelsGUI();
			MapGUI();
			TowersFolderGUI();
			BuildingPlaceGUI();
			PathwaysFolderGUI();
			PathwayGUI();
			SpawnPointGUI();
			LevelFolderGUI();
			LevelDescriptionGUI();

			// Exibe caixa de ajuda se a cena for desconhecida
			if (myState == MyState.Disabled)
			{
				EditorGUILayout.HelpBox("Active scene is not a Tower Defense 2D scene", MessageType.Info);
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Editor disabled in play mode", MessageType.Info);
		}
	}

	/// <summary>
	/// Exibe as guias principais do editor.
	/// </summary>
	private void EditorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		// Tab dos Níveis
		if (GUILayout.Button(contents.levelsFolder, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
		{
			Selection.activeObject = null;
		}

		switch (myState)
		{
		case MyState.LevelMap:
			// Tab dos mapas
			if (GUILayout.Button(contents.mapFolder, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				UpdateStuff();
				Selection.activeObject = inspectors.mapFolder.gameObject;
			}
			// Tab das torres
			if (GUILayout.Button(contents.towersFolder, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				UpdateStuff();
				Selection.activeObject = inspectors.towersFolder.gameObject;
			}
			// Tab dos caminhos
			if (GUILayout.Button(contents.pathwaysFolder, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				UpdateStuff();
				Selection.activeObject = inspectors.pathwaysFolder.gameObject;
			}
			// Tab das condições do nível
			if (GUILayout.Button(contents.conditionsFolder, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				UpdateStuff();
				Selection.activeObject = inspectors.levelManager.gameObject;
			}
			break;
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// GUI de níveis de exibição.
	/// </summary>
	private void LevelsGUI()
	{
		if (mySubState.Count == 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Levels");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Botão que Cria um novo nível
			if (GUILayout.Button(contents.newLevel) == true)
			{
				string[] filePaths = Directory.GetFiles(Application.dataPath + "/TD2D/Scenes/Levels", "*.unity");
				int counter = 1;
				// Procura por cenas de níveis
				for (;;)
				{
					string levelName = "Level" + counter.ToString();
					bool hitted = false;
					foreach (string file in filePaths)
					{
						if (Path.GetFileNameWithoutExtension(file) == levelName)
						{
							hitted = true;
						}
					}
					// Nenhuma cena desse nível
					if (hitted == false)
					{
						// Cria novo nível a partir do modelo
						if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
						{
							// Cena
							if (AssetDatabase.ValidateMoveAsset("Assets/TD2D/Scenes/Levels/Template/Level0.unity", "Assets/TD2D/Scenes/Levels/" + levelName + ".unity") == "")
							{
								AssetDatabase.CopyAsset("Assets/TD2D/Scenes/Levels/Template/Level0.unity", "Assets/TD2D/Scenes/Levels/" + levelName + ".unity");
							}
							// Prefab da descrição de nível
							if (AssetDatabase.ValidateMoveAsset("Assets/TD2D/Prefabs/Levels/Template/Level0.prefab", "Assets/TD2D/Prefabs/Levels/" + levelName + ".prefab") == "")
							{
								AssetDatabase.CopyAsset("Assets/TD2D/Prefabs/Levels/Template/Level0.prefab", "Assets/TD2D/Prefabs/Levels/" + levelName + ".prefab");
							}
							AssetDatabase.Refresh();
							// Abrir cena criada
							EditorSceneManager.OpenScene("Assets/TD2D/Scenes/Levels/" + levelName +".unity");
						}
						break;
					}
					counter++;
				}
			}

			switch (myState)
			{
			case MyState.LevelMap:
					// Botão Mudar para a descrição do nível
					if (currentLevelName != "" && currentLevelName != "Level0")
				{
					if (GUILayout.Button("Edit description") == true)
					{
						if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
						{
							EditorSceneManager.OpenScene("Assets/TD2D/Scenes/LevelChoose.unity");
						}
					}
				}
				break;
			case MyState.LevelDescription:
					// Mudar para o botão de edição do mapa
					if (currentLevelName != "")
				{
					if (GUILayout.Button("Edit map") == true)
					{
						if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
						{
							EditorSceneManager.OpenScene("Assets/TD2D/Scenes/Levels/" + currentLevelName +".unity");
						}
					}
				}
				break;
			}

			GUILayout.Space(guiSpace);

			// Botão de redefinir o progresso do jogo
			if (inspectors.dataManager != null)
			{
				if (GUILayout.Button(contents.resetProgress) == true)
				{
					inspectors.dataManager.ResetGameProgress();
				}
			}

			GUILayout.Space(guiSpace);

			// Abra o botão de todos os níveis do jogo
			if (inspectors.dataManager != null && inspectors.levelChooser != null)
			{
				if (GUILayout.Button(contents.openAllLevels) == true)
				{
					foreach (GameObject level in inspectors.levelChooser.GetLevelPrefabs())
					{
						inspectors.dataManager.PermitLevel(level.name);
					}
					Debug.Log("All game levels allowed now");
				}
			}
		}
	}

	/// <summary>
	/// Exibe GUI do mapa.
	/// </summary>
	private void MapGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.MapFolder))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Map");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Botão que escolhe a imagem do mapa
			if (GUILayout.Button(contents.chooseMap, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				ShowPicker(PickerState.Map, Lables.map);
			}
			// Botão que Cria o ícone de spawn
			if (GUILayout.Button(contents.spawnIcon, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				ShowPicker(PickerState.SpawnIcon, Lables.spawnIcon);
			}
			// Botão que Cria o ícone de captura
			if (GUILayout.Button(contents.captureIcon, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				ShowPicker(PickerState.CaptureIcon, Lables.captureIcon);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Se houver um novo objeto selecionado na janela do selecionador
			if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				pickedGameObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
			}
			// Janela do selecionador fechada
			if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				switch (pickerState)
				{
				case PickerState.Map:
					if (pickedGameObject != null)
					{
							// Carrega um novo mapa
							inspectors.mapFolder.LoadMap(pickedGameObject);
						if (inspectors.mapFolder.map != null)
						{
							Selection.activeGameObject = inspectors.mapFolder.map.gameObject;
							CameraControl cameraControl = FindObjectOfType<CameraControl>();
							if (cameraControl != null)
							{
								cameraControl.focusObjectRenderer = inspectors.mapFolder.map;
							}
						}
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
					break;
				case PickerState.SpawnIcon:
					if (pickedGameObject != null)
					{
							// Adiciona o ícone de spawn ao mapa
							Selection.activeObject = inspectors.mapFolder.AddSpawnIcon(pickedGameObject);
					}
					break;
				case PickerState.CaptureIcon:
					if (pickedGameObject != null)
					{
							// Adicionar o ícone de captura ao mapa
							Selection.activeObject = inspectors.mapFolder.AddCaptureIcon(pickedGameObject);
					}
					break;
				}

				pickedGameObject = null;
				pickerState = PickerState.None;
			}
		}
	}

	/// <summary>
	/// Torres de exibição GUI.
	/// </summary>
	private void TowersFolderGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.TowersFolder))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Towers");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Escolhe o botão da torre anterior
			if (GUILayout.Button(contents.prev, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.towersFolder.GetPrevioustBuildingPlace(Selection.activeObject as GameObject);
			}
			// botão que adiciona uma torre
			if (GUILayout.Button(contents.addTower, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				GameObject tower = inspectors.towersFolder.AddTower();
				if (tower != null)
				{
					Selection.activeObject = tower;
				}
			}
			// Escolhe o botão da próxima torre
			if (GUILayout.Button(contents.next, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.towersFolder.GetNextBuildingPlace(Selection.activeObject as GameObject);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Exibe GUI do local de construção.
	/// </summary>
	private void BuildingPlaceGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.BuildingPlace))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Tower");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Foca no botão de lugar de construção
			if (GUILayout.Button(contents.buildingPlace, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.buildingPlace.gameObject;
			}
			// Foca no botão defender ponto
			if (GUILayout.Button(contents.defendPoint, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.buildingPlace.GetDefendPoint();
			}
			// Botão de mudança da torre
			if (GUILayout.Button(contents.tower, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				ShowPicker(PickerState.Tower, Lables.tower);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Novo objeto selecionado na janela do selecionador
			if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				pickedGameObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
				if (pickedGameObject != null)
				{
					// Define uma nova torre para este local de construção
					Selection.activeObject = inspectors.buildingPlace.ChooseTower(pickedGameObject);
				}
			}
			// Janela do selecionador fechada
			if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				pickedGameObject = null;
				pickerState = PickerState.None;
			}
		}
	}

	/// <summary>
	/// Exibe caminhos GUI.
	/// </summary>
	private void PathwaysFolderGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.PathwaysFolder))
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Pathways");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Escolhe o botão do ponto de captura anterior
			if (GUILayout.Button(contents.prev, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.GetPrevioustCapturePoint(Selection.activeObject as GameObject);
			}
			// Botão que adiciona um/diversos ponto de captura
			if (GUILayout.Button(contents.capturePoint, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.AddCapturePoint();
			}
			// Escolhe o botão do próximo ponto de captura
			if (GUILayout.Button(contents.next, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.GetNextCapturePoint(Selection.activeObject as GameObject);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Botão Escolhe caminho anterior
			if (GUILayout.Button(contents.prev, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.GetPrevioustPathway(Selection.activeObject as GameObject);
			}
			// Botão que adiciona um/diversos caminhos
			if (GUILayout.Button(contents.addPathway, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.AddPathway();
			}
			// Botão que escolhe o próximo caminho
			if (GUILayout.Button(contents.next, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathwaysFolder.GetNextPathway(Selection.activeObject as GameObject);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Exibe GUI do caminho.
	/// </summary>
	private void PathwayGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.Pathway))
		{
			GUILayout.Space(guiSpace);

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Pathway");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Foca no botão de ponto de spawn
			if (GUILayout.Button(contents.spawnPoint, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathway.GetSpawnPoint();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Escolhe o botão do waypoint anterior
			if (GUILayout.Button(contents.prev, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathway.GetPrevioustWaypoint(Selection.activeObject as GameObject);
			}
			// Botão que adiciona um ponto de passagem
			if (GUILayout.Button(contents.waypoint, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathway.AddWaypoint();
			}
			// Botão que escolhe o próximo waypoint
			if (GUILayout.Button(contents.next, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) == true)
			{
				Selection.activeObject = inspectors.pathway.GetNextWaypoint(Selection.activeObject as GameObject);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Exibe GUI do ponto de spawn.
	/// </summary>
	private void SpawnPointGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.SpawnPoint))
		{
			GUILayout.Space(guiSpace);

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Enemies");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Exibe a contagem de inimigos para cada ronda neste ponto de spawn
			for (int i = 0; i < inspectors.spawnPoint.enemies.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Wave " + (i + 1));
				GUILayout.FlexibleSpace();
				inspectors.spawnPoint.enemies[i] = EditorGUILayout.IntField(inspectors.spawnPoint.enemies[i]);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			// Botão que remove a ronda
			if (GUILayout.Button(contents.remove, GUILayout.Width(20), GUILayout.Height(20)) == true)
			{
				inspectors.spawnPoint.RemoveWave();
			}
			GUILayout.FlexibleSpace();
			// Botão que adiciona a onda
			if (GUILayout.Button(contents.add, GUILayout.Width(20), GUILayout.Height(20)) == true)
			{
				inspectors.spawnPoint.AddWave();
			}
			EditorGUILayout.EndHorizontal();

			if (GUI.changed == true)
			{
				inspectors.spawnPoint.UpdateWaveList();
				inspectors.wavesInfo.Update();
			}
		}
	}

	/// <summary>
	/// GUI de configuração do nível.
	/// </summary>
	private void LevelFolderGUI()
	{
		if (myState == MyState.LevelMap && mySubState.Contains(MySubState.EnemiesFolder))
		{
			GUILayout.Space(guiSpace);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Gold amount");
			// Quantidade de ouro para este nível
			inspectors.levelManager.goldAmount = EditorGUILayout.IntField(inspectors.levelManager.goldAmount);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Defeat attempts");
			// tentativas de derrota antes de perder para este nível
			inspectors.levelManager.defeatAttempts = EditorGUILayout.IntField(inspectors.levelManager.defeatAttempts);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Random enemies");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			// Atualiza o número das listas dos inimigos aleatórios
			while (enemiesList.Count < inspectors.levelManager.enemiesList.Count)
			{
				enemiesList.Add(true);
			}
			while (enemiesList.Count > inspectors.levelManager.enemiesList.Count)
			{
				enemiesList.RemoveAt(enemiesList.Count - 1);
			}
			// Exibe lista de inimigos aleatórios
			enemiesScrollPos = EditorGUILayout.BeginScrollView(enemiesScrollPos, GUILayout.MaxHeight(55));
			for (int i = 0; i < enemiesList.Count; ++i)
			{
				enemiesList[i] = EditorGUILayout.Toggle(inspectors.levelManager.enemiesList[i].name, enemiesList[i]);
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Tower upgrades");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			// Atualiza a lista de atualizações de torre
			while (towersList.Count < inspectors.levelManager.towersList.Count)
			{
				towersList.Add(true);
			}
			while (towersList.Count > inspectors.levelManager.towersList.Count)
			{
				towersList.RemoveAt(towersList.Count - 1);
			}
			// Exibe a lista de atualizações da torre
			towersScrollPos = EditorGUILayout.BeginScrollView(towersScrollPos, GUILayout.MaxHeight(55));
			for (int i = 0; i < towersList.Count; ++i)
			{
				towersList[i] = EditorGUILayout.Toggle(inspectors.levelManager.towersList[i].name, towersList[i]);
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Spells");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			// Atualiza lista de feitiços
			while (spellsList.Count < inspectors.levelManager.spellsList.Count)
			{
				spellsList.Add(true);
			}
			while (spellsList.Count > inspectors.levelManager.spellsList.Count)
			{
				spellsList.RemoveAt(towersList.Count - 1);
			}
			// Exibe a lista de feitiços
			spellsScrollPos = EditorGUILayout.BeginScrollView(spellsScrollPos, GUILayout.MaxHeight(55));
			for (int i = 0; i < spellsList.Count; ++i)
			{
				spellsList[i] = EditorGUILayout.Toggle(inspectors.levelManager.spellsList[i].name, spellsList[i]);
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Waves timeouts");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.HelpBox("Waves number calculated from spawnpoints", MessageType.Info);
			// Exibe tempos limite entre ondas
			wavesScrollPos = EditorGUILayout.BeginScrollView(wavesScrollPos, GUILayout.MaxHeight(55));
			for (int i = 0; i < inspectors.wavesInfo.timeouts.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Wave " + (i + 1));
				inspectors.wavesInfo.timeouts[i] = EditorGUILayout.FloatField(inspectors.wavesInfo.timeouts[i]);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();

			// Aplica as mudanças
			if (GUI.changed == true)
			{
				inspectors.levelManager.enemies.Clear();
				for (int i = 0; i < enemiesList.Count; ++i)
				{
					if (enemiesList[i] == true)
					{
						inspectors.levelManager.enemies.Add(inspectors.levelManager.enemiesList[i]);
					}
				}
				inspectors.levelManager.towers.Clear();
				for (int i = 0; i < towersList.Count; ++i)
				{
					if (towersList[i] == true)
					{
						inspectors.levelManager.towers.Add(inspectors.levelManager.towersList[i]);
					}
				}
				inspectors.levelManager.spells.Clear();
				for (int i = 0; i < spellsList.Count; ++i)
				{
					if (spellsList[i] == true)
					{
						inspectors.levelManager.spells.Add(inspectors.levelManager.spellsList[i]);
					}
				}
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}

	/// <summary>
	/// Exibe GUI de descrição do nível.
	/// </summary>
	private void LevelDescriptionGUI()
	{
		if (myState == MyState.LevelDescription && mySubState.Contains(MySubState.LevelDescription))
		{
			GUILayout.Space(guiSpace);

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// botão que define ícon do nível
			if (GUILayout.Button(contents.levelIcon, GUILayout.MaxWidth(100f), GUILayout.MaxHeight(100f)) == true)
			{
				ShowPicker(PickerState.LevelIcon, Lables.levelIcon);
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			// Cabeçalho
			GUILayout.Label("Header");
			inspectors.levelDescription.header.text = EditorGUILayout.TextField(inspectors.levelDescription.header.text);
			// Descrição
			GUILayout.Label("Description");
			inspectors.levelDescription.description.text = EditorGUILayout.TextArea(inspectors.levelDescription.description.text, GUILayout.MaxHeight(80f));
			// Atenção
			GUILayout.Label("Attention");
			inspectors.levelDescription.attention.text = EditorGUILayout.TextArea(inspectors.levelDescription.attention.text, GUILayout.MaxHeight(40f));

			GUILayout.Space(guiSpace);
			// Botão Aplica alterações
			if (GUILayout.Button("Apply changes") == true)
			{
				PrefabUtility.ReplacePrefab(inspectors.levelDescription.gameObject, PrefabUtility.GetCorrespondingObjectFromSource(inspectors.levelDescription.gameObject), ReplacePrefabOptions.ConnectToPrefab);
			}

			// Novo objeto selecionado na janela do selecionador
			if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				pickedGameObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
				if (pickedGameObject != null)
				{
					SpriteRenderer spriteRenderer = pickedGameObject.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null)
					{
						// Define o ícone na descrição do nível
						inspectors.levelDescription.icon.sprite = spriteRenderer.sprite;
						EditorUtility.SetDirty(inspectors.levelDescription.gameObject);
						contents.levelIcon = new GUIContent(inspectors.levelDescription.icon.sprite.texture, "Choose icon for this level");
					}
				}
			}
			// Janela do selecionador fechada
			if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
			{
				pickedGameObject = null;
				pickerState = PickerState.None;
			}

			if (GUI.changed == true)
			{
				EditorUtility.SetDirty(inspectors.levelDescription.gameObject);
			}
		}
	}
}

