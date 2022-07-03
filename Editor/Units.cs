using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Janela do editor de unidades de defesa de torre
/// </summary>
public class TD2D_Units : EditorWindow
{
	// Estados possíveis
	private enum MyState
	{
		Disabled,
		Enemy,
		Defender,
		Tower,
		Barracks
	}
	// Descrição de unidade selecionado
	private class UnitData
	{
		public GameObject gameObject;
		public Price price;
		public Tower tower;
		public DefendersSpawner spawner;
		public TowerActionsInspector towerActions;
		public NavAgent navAgent;
		public DamageTaker damageTaker;
		public FeaturesInspector aiFeature;
		public Attack attack;
		public GameObject range;
		public bool flying;
		public int attackType;
		public bool targetCommon;
		public bool targetFlying;

		public void Clear()
		{
			gameObject = null;
			price = null;
			tower = null;
			spawner = null;
			towerActions = null;
			navAgent = null;
			damageTaker = null;
			aiFeature = null;
			attack = null;
			range = null;
			flying = false;
			attackType = 0;
			targetCommon = false;
			targetFlying = false;
		}
	}
	// A legenda ajuda a organizar a pesquisa de prefabs
	private class Lables
	{
		public const string bulletAlly = "l:BulletAlly";
		public const string bulletEnemy = "l:BulletEnemy";
		public const string towerAction = "l:TAction";
		public const string defender = "l:Defender";
		public const string tower = "l:Tower";
		public const string icon = "";
		public const string feature = "l:Feature";
	}
	// Estados da janela do selecionador
	private enum PickerState
	{
		None,
		BulletAlly,
		BulletEnemy,
		TowerActions,
		Defenders,
		Towers,
		Icons,
		Features
	}
	// Conteúdo visual para partes do editor
	private class Contents
	{
		public GUIContent newEnemyButton;
		public GUIContent newDefenderButton;
		public GUIContent newTowerButton;
		public GUIContent newBarracksButton;
		public GUIContent chooseBulletButton;
		public GUIContent focusOnFirePointButton;
		public GUIContent chooseDefenderButton;
		public GUIContent applyChangesButton;
		public GUIContent removeFromSceneButton;
		public GUIContent add;
		public GUIContent remove;
		public GUIContent next;
		public GUIContent prev;
	}

	// Meu estado
	private MyState myState = MyState.Disabled;
	// Descrição da unidade
	private UnitData unitData = new UnitData();
	// Apenas algum espaço livre na janela do editor
	private float guiSpace = 15f;
	// Estado da janela do selecionador
	private PickerState pickerState = PickerState.None;
	// ID da janela do selecionador
	private int currentPickerWindow;
	// O objeto é selecionado na janela do selecionador
	private Object pickedObject;
	// Tema visual do editor
	private GUISkin editorGuiSkin;
	// Editor de conteúdo visual
	private Contents contents = new Contents();

	[MenuItem("Window/TD2D/Units")]

	/// <summary>
	/// Mostra a janela.
	/// </summary>
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(TD2D_Units));
	}

	/// <summary>
	/// Ativo o enable event.
	/// </summary>
	void OnEnable()
	{
		EditorSceneManager.sceneOpened += OnSceneOpened;
		Selection.selectionChanged += OnSelectionChanged;
		EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
		UpdateStuff();
	}

	/// <summary>
	/// Ativa o disable event.
	/// </summary>
	void OnDisable()
	{
		EditorSceneManager.sceneOpened -= OnSceneOpened;
		Selection.selectionChanged -= OnSelectionChanged;
		EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
	}

	/// <summary>
	/// Ativa a cena do opened event 
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="openSceneMode">Open scene mode.</param>
	private void OnSceneOpened(Scene scene, OpenSceneMode openSceneMode)
	{
		UpdateStuff();
	}

	/// <summary>
	/// Ativa o estado do modo do jogo changed event 
	/// </summary>
	private void OnPlaymodeStateChanged(UnityEditor.PlayModeStateChange playModeStateChange)
	{
		if (playModeStateChange == PlayModeStateChange.EnteredEditMode || playModeStateChange == PlayModeStateChange.ExitingPlayMode)
		{
			UpdateStuff();
		}
	}

	/// <summary>
	/// Ativa a seleção do changed event 
	/// </summary>
	private void OnSelectionChanged()
	{
		myState = MyState.Disabled;
		unitData.Clear();
		// Verifica o gameobject selecionado e preenche a descrição da unidade
		if (Selection.GetFiltered<GameObject>(SelectionMode.ExcludePrefab).Length == 1)
		{
			unitData.tower = (Selection.activeObject as GameObject).GetComponentInParent<Tower>();
			if (unitData.tower != null) // Torre
			{
				unitData.spawner = (Selection.activeObject as GameObject).GetComponentInParent<DefendersSpawner>();
				if (unitData.spawner != null) // Quartel de soldados
				{
					myState = MyState.Barracks;
					unitData.gameObject = unitData.tower.gameObject;
					unitData.price = unitData.spawner.GetComponent<Price>();
					unitData.tower = unitData.spawner.GetComponent<Tower>();
					if (unitData.tower != null)
					{
						unitData.range = unitData.tower.range;
						if (unitData.tower.actions != null)
						{
							unitData.towerActions = unitData.tower.actions.GetComponent<TowerActionsInspector>();
						}
					}
					if (unitData.spawner.prefab != null)
					{
						contents.chooseDefenderButton.image = AssetPreview.GetAssetPreview(unitData.spawner.prefab);
					}
					else
					{
						contents.chooseDefenderButton.image = null;
					}
				}
				else // Torre à distância
				{
					myState = MyState.Tower;
					unitData.gameObject = unitData.tower.gameObject;
					unitData.price = unitData.gameObject.GetComponent<Price>();
					unitData.range = unitData.tower.range;
					if (unitData.tower.actions != null)
					{
						unitData.towerActions = unitData.tower.actions.GetComponent<TowerActionsInspector>();
					}
					unitData.attack = unitData.gameObject.GetComponentInChildren<Attack>();
					if (unitData.attack != null)
					{
						AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
						if (trigger != null)
						{
							unitData.targetCommon = trigger.tags.Contains("Enemy") ? true : false;
							unitData.targetFlying = trigger.tags.Contains("FlyingEnemy") ? true : false;
						}
						if ((unitData.attack is AttackRanged) && (unitData.attack as AttackRanged).arrowPrefab != null)
						{
							contents.chooseBulletButton.image = AssetPreview.GetAssetPreview((unitData.attack as AttackRanged).arrowPrefab);
						}
						else
						{
							contents.chooseBulletButton.image = null;
						}
					}
				}
			}
			else // Unidade
			{
				AiBehavior aiBehavior = (Selection.activeObject as GameObject).GetComponentInParent<AiBehavior>();
				if (aiBehavior != null)
				{
					unitData.gameObject = aiBehavior.gameObject;
					switch (unitData.gameObject.tag)
					{
					case "Enemy": goto case "FlyingEnemy";
					case "FlyingEnemy":
						unitData.price = unitData.gameObject.GetComponent<Price>();
						unitData.flying = unitData.gameObject.CompareTag("FlyingEnemy") ? true : false;
						unitData.aiFeature = unitData.gameObject.GetComponentInChildren<FeaturesInspector>();
						goto case "Defender";
					case "Defender":
						myState = unitData.gameObject.CompareTag("Defender") ? MyState.Defender : MyState.Enemy;
						unitData.navAgent = unitData.gameObject.GetComponent<NavAgent>();
						unitData.damageTaker = unitData.gameObject.GetComponent<DamageTaker>();
						unitData.attackType = 0;
						unitData.attack = unitData.gameObject.GetComponentInChildren<Attack>();
						if (unitData.attack != null && (unitData.attack is AttackMelee))
						{
							unitData.attackType = 1;
							AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
							if (trigger != null)
							{
								unitData.targetCommon = trigger.tags.Contains("Enemy") ? true : false;
								unitData.targetFlying = trigger.tags.Contains("FlyingEnemy") ? true : false;
							}
						}
						else if (unitData.attack != null && (unitData.attack is AttackRanged))
						{
							unitData.attackType = 2;
							AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
							if (trigger != null)
							{
								unitData.targetCommon = trigger.tags.Contains("Enemy") ? true : false;
								unitData.targetFlying = trigger.tags.Contains("FlyingEnemy") ? true : false;
							}
							if ((unitData.attack as AttackRanged).arrowPrefab != null)
							{
								contents.chooseBulletButton.image = AssetPreview.GetAssetPreview((unitData.attack as AttackRanged).arrowPrefab);
							}
							else
							{
								contents.chooseBulletButton.image = null;
							}
						}
						break;
					}
				}
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
		// Define os rótulos para prefabs. O rótulo será usado na janela do selecionador
		string[] prefabs;
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Bullets/Ally"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.bulletAlly});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Bullets/Enemy"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.bulletEnemy});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Units/Towers/Actions/Actions"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.towerAction});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Units/Defenders"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.defender});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Units/Towers/Towers"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.tower});
		}
		prefabs = AssetDatabase.FindAssets("", new string[] {"Assets/TD2D/Prefabs/Units/Enemies/AiFeatures"});
		foreach (string str in prefabs)
		{
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(str));
			AssetDatabase.SetLabels(obj, new string[] {Lables.feature});
		}

		// Define o conteúdo visual
		contents.newEnemyButton = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddEnemy.png", typeof(Texture)), "Create new enemy");
		contents.newDefenderButton = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddDefender.png", typeof(Texture)), "Create new defender");
		contents.newTowerButton = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddRangeTower.png", typeof(Texture)), "Create new ranged tower");
		contents.newBarracksButton = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/AddBarracks.png", typeof(Texture)), "Create new barracks");
		contents.chooseBulletButton = new GUIContent((Texture)null, "Choose bulet prefab for ranged attack");
		contents.focusOnFirePointButton = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/FirePoint.png", typeof(Texture)), "Move firepoint");
		contents.chooseDefenderButton = new GUIContent((Texture)null, "Choose defender prefab for this barracks");
		contents.applyChangesButton = new GUIContent("Apply changes", "Apply changes and connect to prefab");
		contents.removeFromSceneButton = new GUIContent("Remove from scene", "Remove this unit from scene");
		contents.add = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Add.png", typeof(Texture)));
		contents.remove = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Remove.png", typeof(Texture)));
		contents.next = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Next.png", typeof(Texture)));
		contents.prev = new GUIContent((Texture)AssetDatabase.LoadAssetAtPath("Assets/TD2D/Sprites/Editor/Prev.png", typeof(Texture)));

		OnSelectionChanged();
	}

	/// <summary>
	/// Mostra a janela do selecionador.
	/// </summary>
	/// <param name="state">Estado</param>
	/// <param name="filter">Filtro</param>
	private void ShowPicker<T>(PickerState state, string filter) where T : UnityEngine.Object
	{
		pickerState = state;
		// Cria um ID de controle do selecionador da janela
		currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive);
		// Utiliza o ID que eu acabei de criar
		EditorGUIUtility.ShowObjectPicker<T>(null, false, filter, currentPickerWindow);
	}

	/// <summary>
	/// Atualiza as camadas e tags da unidade.
	/// </summary>
	private void UpdateLayersAndTags()
	{
		if (unitData.gameObject != null)
		{
			switch (unitData.gameObject.gameObject.tag)
			{
			case "Tower":
			case "Defender":
				if (unitData.attack != null)
				{
					unitData.attack.gameObject.layer = LayerMask.NameToLayer("AttackAlly");
					AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
					if (trigger != null)
					{
						trigger.tags.Clear();
						trigger.tags.Add("Enemy");
						trigger.tags.Add("FlyingEnemy");
					}
				}
				if (unitData.attack != null)
				{
					unitData.attack.gameObject.layer = LayerMask.NameToLayer("AttackAlly");
					AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
					if (trigger != null)
					{
						trigger.tags.Clear();
						trigger.tags.Add("Enemy");
						trigger.tags.Add("FlyingEnemy");
					}
				}
				break;
			case "Enemy":
			case "FlyingEnemy":
				if (unitData.attack != null)
				{
					unitData.attack.gameObject.layer = LayerMask.NameToLayer("AttackEnemy");
					AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
					if (trigger != null)
					{
						trigger.tags.Clear();
						trigger.tags.Add("Defender");
					}
				}
				if (unitData.attack != null)
				{
					unitData.attack.gameObject.layer = LayerMask.NameToLayer("AttackEnemy");
					AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
					if (trigger != null)
					{
						trigger.tags.Clear();
						trigger.tags.Add("Defender");
					}
				}
				break;
			}
		}
	}

	/// <summary>
	/// Cria uma nova unidade a partir do pré-fabricado e coloca em cena.
	/// </summary>
	/// <param name="prefabPath">Caminho do Prefab</param>
	/// <param name="folderPath">Pasta do Prefab</param>
	private void CreateNewUnit(string prefabPath, string folderPath)
	{
		string[] templates;
		templates = AssetDatabase.FindAssets("", new string[] {prefabPath});
		if (templates.Length > 0)
		{
			string newUnitFolder = "";
			for (short i = 1; i < short.MaxValue; ++i)
			{
				if (AssetDatabase.ValidateMoveAsset(prefabPath, folderPath + i) == "")
				{
					newUnitFolder = folderPath + i;
					AssetDatabase.CopyAsset(prefabPath, newUnitFolder);
					break;
				}
			}
			AssetDatabase.Refresh();
			if (newUnitFolder != "")
			{
				string[] prefabs;
				prefabs = AssetDatabase.FindAssets("t:prefab", new string[] {newUnitFolder});
				if (prefabs.Length > 0)
				{
					Object unitPrefab = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(prefabs[0]));
					Selection.activeObject = unitPrefab;
					EditorUtility.FocusProjectWindow();
					GameObject newUnit = PrefabUtility.InstantiatePrefab(unitPrefab) as GameObject;
					Selection.activeObject = newUnit;
					newUnit.transform.SetAsLastSibling();
				}
			}
		}
	}

	/// <summary>
	/// Ativa o update event 
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
		// Define um tema visual
		GUI.skin = editorGuiSkin;

		if (EditorApplication.isPlaying == false)
		{
			switch (myState)
			{
			case MyState.Disabled:
				
				EditorGUILayout.HelpBox("Create your own units and towers", MessageType.Info);

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
					// Adiciona novo botão inimigo
					if (GUILayout.Button(contents.newEnemyButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
				{
					CreateNewUnit("Assets/TD2D/Prefabs/Stuff/Templates/Enemy", "Assets/TD2D/Prefabs/Units/Enemies/Units/Enemy");
				}
					// Adiciona um novo botão de defesa
					if (GUILayout.Button(contents.newDefenderButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
				{
					CreateNewUnit("Assets/TD2D/Prefabs/Stuff/Templates/Defender", "Assets/TD2D/Prefabs/Units/Defenders/Defender");
				}
					// Adiciona um novo botão de torre de longo alcance
					if (GUILayout.Button(contents.newTowerButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
				{
					CreateNewUnit("Assets/TD2D/Prefabs/Stuff/Templates/Tower", "Assets/TD2D/Prefabs/Units/Towers/Towers/MyTowers/Tower");
				}
					// Botão que adiciona um novo quartel de soldados
					if (GUILayout.Button(contents.newBarracksButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
				{
					CreateNewUnit("Assets/TD2D/Prefabs/Stuff/Templates/Barracks", "Assets/TD2D/Prefabs/Units/Towers/Towers/MyTowers/Barracks");
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				break;

			case MyState.Enemy:
			case MyState.Defender:
			case MyState.Tower:
			case MyState.Barracks:

				GUILayout.Space(guiSpace);

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(unitData.gameObject.name);
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				if (myState == MyState.Enemy || myState == MyState.Tower || myState == MyState.Barracks)
				{
						// Preço
						if (unitData.price != null)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Price");
						unitData.price.price = EditorGUILayout.IntField(unitData.price.price);
						EditorGUILayout.EndHorizontal();

						GUILayout.Space(guiSpace);
					}
				}

				if (myState == MyState.Enemy || myState == MyState.Defender)
				{
						// Velocidade
						EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Speed");
					unitData.navAgent.speed = EditorGUILayout.FloatField(unitData.navAgent.speed);
					EditorGUILayout.EndHorizontal();

						// Pontos de vida
						EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Hitpoints");
					unitData.damageTaker.hitpoints = EditorGUILayout.IntField(unitData.damageTaker.hitpoints);
					EditorGUILayout.EndHorizontal();
				}

				if (myState == MyState.Enemy)
				{
						// Bandeira que voa
						bool flyingEnemy = EditorGUILayout.Toggle("Flying", unitData.flying);
					if (flyingEnemy != unitData.flying && unitData.gameObject != null)
					{
						unitData.gameObject.tag = flyingEnemy == true ? "FlyingEnemy" : "Enemy";
						unitData.flying = flyingEnemy;
					}
				}

				if (myState == MyState.Enemy || myState == MyState.Defender || myState == MyState.Tower)
				{
					GUILayout.Space(guiSpace);

					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Attack");
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}

				if (myState == MyState.Enemy || myState == MyState.Defender)
				{
						// Tipo de ataque
						string[] attackTypes = new string[] {"None", "Melee", "Ranged"};
					int attackType = GUILayout.SelectionGrid(unitData.attackType, attackTypes, 1, EditorStyles.radioButton);
					if (attackType != unitData.attackType)
					{
						if (unitData.attack != null)
						{
							DestroyImmediate(unitData.attack.gameObject);
							unitData.attack = null;
						}
						switch (attackType)
						{
						case 1:
							Object meleeAttackPrefab = AssetDatabase.LoadAssetAtPath<Object>("Assets/TD2D/Prefabs/Stuff/AttackTypes/MeleeAttack.prefab");
							if (meleeAttackPrefab != null)
							{
								GameObject meleeAttack = Instantiate(meleeAttackPrefab, unitData.gameObject.transform) as GameObject;
								meleeAttack.name = meleeAttackPrefab.name;
								unitData.attack = meleeAttack.GetComponent<Attack>();
							}
							break;
						case 2:
							Object rangedAttackPrefab = AssetDatabase.LoadAssetAtPath<Object>("Assets/TD2D/Prefabs/Stuff/AttackTypes/RangedAttack.prefab");
							if (rangedAttackPrefab != null)
							{
								GameObject rangedAttack = Instantiate(rangedAttackPrefab, unitData.gameObject.transform) as GameObject;
								rangedAttack.name = rangedAttackPrefab.name;
								unitData.attack = rangedAttack.GetComponent<Attack>();
							}
							break;
						}
						unitData.attackType = attackType;
						Selection.activeObject = unitData.gameObject.gameObject;
						UpdateLayersAndTags();
					}
				}

				if (myState == MyState.Enemy || myState == MyState.Defender || myState == MyState.Tower)
				{
					if (unitData.attack != null)
					{
							// Dano
							EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Damage");
						unitData.attack.damage = EditorGUILayout.IntField(unitData.attack.damage);
						EditorGUILayout.EndHorizontal();
						// Cooldown geral
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Cooldown");
						unitData.attack.cooldown = EditorGUILayout.FloatField(unitData.attack.cooldown);
						EditorGUILayout.EndHorizontal();
						// Alcançe
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Range");
						unitData.attack.transform.localScale = EditorGUILayout.FloatField(unitData.attack.transform.localScale.x) * Vector3.one;
						EditorGUILayout.EndHorizontal();
					}

					if (unitData.attack is AttackRanged)
					{
							// Escolhe o botão de bala
							EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(contents.chooseBulletButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
						{
							switch (unitData.gameObject.tag)
							{
							case "Defender":
							case "Tower":
								ShowPicker<GameObject>(PickerState.BulletAlly, Lables.bulletAlly);
								break;
							case "Enemy":
							case "FlyingEnemy":
								ShowPicker<GameObject>(PickerState.BulletEnemy, Lables.bulletEnemy);
								break;
							}
						}
							// Foca no botão firepoint
							if (GUILayout.Button(contents.focusOnFirePointButton, GUILayout.MaxWidth(40f), GUILayout.MaxHeight(40f)) == true)
						{
							if ((unitData.attack as AttackRanged).firePoint != null)
							{
								Selection.activeGameObject = (unitData.attack as AttackRanged).firePoint.gameObject;
							}
						}
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
				}

				if (myState == MyState.Enemy)
				{
					if (unitData.aiFeature != null)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Features");
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

						AiFeature feature = (Selection.activeObject as GameObject).GetComponent<AiFeature>();

							// Botões de recursos
							EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(contents.add, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
						{
							ShowPicker<GameObject>(PickerState.Features, Lables.feature);
						}
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(contents.prev, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
						{
							Selection.activeObject = unitData.aiFeature.GetPreviousFeature(Selection.activeObject as GameObject);
						}
						if (GUILayout.Button(contents.next, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
						{
							Selection.activeObject = unitData.aiFeature.GetNextFeature(Selection.activeObject as GameObject);
						}
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(contents.remove, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
						{
							if (feature != null)
							{
								DestroyImmediate(feature.gameObject);
								Selection.activeObject = unitData.gameObject;
							}
						}
						EditorGUILayout.EndHorizontal();

						if (feature != null)
						{
							EditorGUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.Label(feature.name);
							GUILayout.FlexibleSpace();
							EditorGUILayout.EndHorizontal();

							AiFeaturesGUI(feature);
						}
					}
				}

				if (myState == MyState.Defender || myState == MyState.Tower)
				{
					if (unitData.attack != null)
					{
						// Alvos
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Targets");
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

						bool land = EditorGUILayout.Toggle("Land", unitData.targetCommon);
						bool flying = EditorGUILayout.Toggle("Flying", unitData.targetFlying);
						if (land != unitData.targetCommon || flying != unitData.targetFlying)
						{
							AiTriggerCollider trigger = unitData.attack.GetComponent<AiTriggerCollider>();
							if (trigger != null)
							{
								trigger.tags.Clear();
								if (land == true)
								{
									trigger.tags.Add("Enemy");
								}
								if (flying == true)
								{
									trigger.tags.Add("FlyingEnemy");
								}
							}
							unitData.targetCommon = land;
							unitData.targetFlying = flying;
						}
					}
				}

				if (myState == MyState.Barracks)
				{
					if (unitData.spawner != null)
					{
							// Número de defensores
							EditorGUILayout.LabelField("Defenders number");
						unitData.spawner.maxNum = EditorGUILayout.IntSlider(unitData.spawner.maxNum, 1, 3);

							// Cooldown entre os defensores ao spawnar
							EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Cooldown");
						unitData.spawner.cooldown = EditorGUILayout.FloatField(unitData.spawner.cooldown);
						EditorGUILayout.EndHorizontal();
					}

					if (unitData.range != null)
					{
						// Alcançe
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel("Range");
						unitData.range.transform.localScale = EditorGUILayout.FloatField(unitData.range.transform.localScale.x) * Vector3.one;
						EditorGUILayout.EndHorizontal();
					}

					if (unitData.spawner != null)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Defender");
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

							// PreFab do defensor
							EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(contents.chooseDefenderButton, GUILayout.MaxWidth(60f), GUILayout.MaxHeight(60f)) == true)
						{
							ShowPicker<GameObject>(PickerState.Defenders, Lables.defender);
						}
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
				}

				if (myState == MyState.Tower || myState == MyState.Barracks)
				{
					if (unitData.range != null)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Range");
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

						// Mostra o alcançe
						unitData.range.SetActive(EditorGUILayout.Toggle("Show range", unitData.range.activeSelf));
					}

					if (unitData.towerActions != null)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Actions");
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

						unitData.towerActions.gameObject.SetActive(EditorGUILayout.Toggle("Show actions", unitData.towerActions.gameObject.activeSelf));

							// Botões de ações
							if (unitData.towerActions.gameObject.activeSelf == true)
						{
							TowerAction action = (Selection.activeObject as GameObject).GetComponent<TowerAction>();

							EditorGUILayout.BeginHorizontal();
							if (GUILayout.Button(contents.add, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
							{
								ShowPicker<GameObject>(PickerState.TowerActions, Lables.towerAction);
							}
							GUILayout.FlexibleSpace();
							if (GUILayout.Button(contents.prev, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
							{
								Selection.activeObject = unitData.towerActions.GetPrevioustAction(Selection.activeObject as GameObject);
							}
							if (GUILayout.Button(contents.next, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
							{
								Selection.activeObject = unitData.towerActions.GetNextAction(Selection.activeObject as GameObject);
							}
							GUILayout.FlexibleSpace();
							if (GUILayout.Button(contents.remove, GUILayout.MaxWidth(30f), GUILayout.MaxHeight(30f)) == true)
							{
								if (action != null)
								{
									DestroyImmediate(action.gameObject);
									Selection.activeObject = unitData.gameObject;
								}
							}
							EditorGUILayout.EndHorizontal();

							if (action != null)
							{
								EditorGUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
								GUILayout.Label(action.name);
								GUILayout.FlexibleSpace();
								EditorGUILayout.EndHorizontal();


								TowerActionsGUI(action);
							}
						}
					}
				}

					// Novo objeto selecionado na janela do selecionador
					if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
				{
					pickedObject = EditorGUIUtility.GetObjectPickerObject();
					if (pickedObject != null)
					{
						switch (pickerState)
						{
						case PickerState.BulletAlly:
						case PickerState.BulletEnemy:
							if (unitData.attack != null && unitData.attack is AttackRanged)
							{
								(unitData.attack as AttackRanged).arrowPrefab = pickedObject as GameObject;
								contents.chooseBulletButton.image = AssetPreview.GetAssetPreview((unitData.attack as AttackRanged).arrowPrefab);
							}
							break;
						case PickerState.Defenders:
							if (unitData.spawner != null)
							{
								unitData.spawner.prefab = pickedObject as GameObject;
								contents.chooseDefenderButton.image = AssetPreview.GetAssetPreview(unitData.spawner.prefab);
							}
							break;
						case PickerState.Towers:
							{
								TowerActionBuild towerActionBuild = (Selection.activeObject as GameObject).GetComponent<TowerActionBuild>();
								if (towerActionBuild != null)
								{
									towerActionBuild.towerPrefab = pickedObject as GameObject;
									EditorUtility.SetDirty(towerActionBuild.gameObject);
								}
							}
							break;
						case PickerState.Icons:
							{
								TowerAction towerAction = (Selection.activeObject as GameObject).GetComponent<TowerAction>();
								if (towerAction != null)
								{
									Image image = towerAction.enabledIcon.GetComponent<Image>();
									if (image != null)
									{
										image.sprite = pickedObject as Sprite;
										EditorUtility.SetDirty(towerAction.gameObject);
									}
								}
							}
							break;
						}
					}
				}
					// Janela do selecionador fechada
					if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
				{
					if (pickedObject != null)
					{
						switch (pickerState)
						{
						case PickerState.TowerActions:
							if (unitData.towerActions != null)
							{
								unitData.towerActions.AddAction(pickedObject as GameObject);
							}
							break;
						case PickerState.Features:
							if (unitData.aiFeature != null)
							{
								unitData.aiFeature.AddFeature(pickedObject as GameObject);
							}
							break;
						}
					}
					pickedObject = null;
					pickerState = PickerState.None;
				}
				break;
			}

			if (myState != MyState.Disabled)
			{
				GUILayout.Space(guiSpace);

				// Botão que aplica alterações
				if (unitData.gameObject != null && PrefabUtility.GetPrefabType(unitData.gameObject) != PrefabType.None)
				{
					if (GUILayout.Button(contents.applyChangesButton) == true)
					{
						if (unitData.towerActions != null)
						{
							unitData.towerActions.gameObject.SetActive(false);
						}
						if (unitData.range != null)
						{
							unitData.range.SetActive(false);
						}
						PrefabUtility.ReplacePrefab(unitData.gameObject.gameObject, PrefabUtility.GetCorrespondingObjectFromSource(unitData.gameObject.gameObject), ReplacePrefabOptions.ConnectToPrefab);
					}

					// Botão Remove da cena
					if (GUILayout.Button(contents.removeFromSceneButton) == true)
					{
						DestroyImmediate(unitData.gameObject.gameObject);
						Selection.activeObject = null;
					}
				}
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Editor disabled in play mode", MessageType.Info);
		}
	}

	/// <summary>
	/// GUI para a árvore de ação da torre
	/// </summary>
	/// <param name="action">Ação</param>
	private void TowerActionsGUI(TowerAction action)
	{
		if (action is TowerActionCooldown)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Cooldown");
			(action as TowerActionCooldown).cooldown = EditorGUILayout.FloatField((action as TowerActionCooldown).cooldown);
			EditorGUILayout.EndHorizontal();
		}

		if (action is TowerActionBuild)
		{
			if (GUILayout.Button("Choose tower prefab") == true)
			{
				ShowPicker<GameObject>(PickerState.Towers, Lables.tower);
			}
			if (GUILayout.Button("Choose icon") == true)
			{
				ShowPicker<Sprite>(PickerState.Icons, Lables.icon);
			}
		}
		else if (action is TowerActionSkill)
		{
			if (GUILayout.Button("Choose icon") == true)
			{
				ShowPicker<Sprite>(PickerState.Icons, Lables.icon);
			}
		}
	}

	/// <summary>
	/// GUI para recursos da unidade
	/// </summary>
	/// <param name="feature">Recurso</param>
	private void AiFeaturesGUI(AiFeature feature)
	{
		Heal heal = feature.GetComponent<Heal>();
		if (heal != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Range");
			heal.transform.localScale = EditorGUILayout.FloatField(heal.transform.localScale.x) * Vector3.one;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Heal amount");
			heal.healAmount = EditorGUILayout.IntField(heal.healAmount);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Cooldown");
			heal.cooldown = EditorGUILayout.FloatField(heal.cooldown);
			EditorGUILayout.EndHorizontal();

			return;
		}
		AoeHeal aoeHeal = feature.GetComponent<AoeHeal>();
		if (aoeHeal != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Range");
			aoeHeal.transform.localScale = EditorGUILayout.FloatField(aoeHeal.transform.localScale.x) * Vector3.one;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Heal amount");
			aoeHeal.healAmount = EditorGUILayout.IntField(aoeHeal.healAmount);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Cooldown");
			aoeHeal.cooldown = EditorGUILayout.FloatField(aoeHeal.cooldown);
			EditorGUILayout.EndHorizontal();

			return;
		}
		AloneSpeedUp aloneSpeedUp = feature.GetComponent<AloneSpeedUp>();
		if (aloneSpeedUp != null)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Speed up amount");
			aloneSpeedUp.speedUpAmount = EditorGUILayout.FloatField(aloneSpeedUp.speedUpAmount);
			EditorGUILayout.EndHorizontal();

			return;
		}
	}
}
