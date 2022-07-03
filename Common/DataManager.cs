using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Versão do formato dos dados salvados. Utilizar para verificar se o formato de dados armazenado é igual ao formato de dados real (Para eu próprio)
/// </summary>
[Serializable]
public class DataVersion
{
    public int major = 1;
    public int minor = 0;
}

/// <summary>
/// Formato dos dados de progresso do jogo armazenados.
/// </summary>
[Serializable]
public class GameProgressData
{
	public System.DateTime saveTime = DateTime.MinValue;    // Tempo salvado
	public string lastCompetedLevel = "";                   // O nome do nível foi concluído pela última vez
	public List<string> openedLevels = new List<string>();  // Lista com níveis disponíveis para jogar
}

/// <summary>
/// Formato das configurações de jogo armazenados.
/// </summary>
[Serializable]
public class GameConfigurations
{
	public float soundVolume = 0.5f;
	public float musicVolume = 0.5f;
}

/// <summary>
/// Salvar e carregar dados dos arquivos.
/// </summary>
public class DataManager : MonoBehaviour
{
	// Singleton
	public static DataManager instance;

	// Recipiente de dados de progresso do jogo
	public GameProgressData progress = new GameProgressData();
	// Recipiente de configurações do jogo	
	public GameConfigurations configs = new GameConfigurations();

	// Recipiente da versão de dados
	private DataVersion dataVersion = new DataVersion();
	// Nome do arquivo com versão dos dados
	private string dataVersionFile = "/DataVersion.dat";
	// Nome do arquivo com dados de progresso do jogo
	private string gameProgressFile = "/GameProgress.dat";
	// Nome do arquivo com as configurações do jogo
	private string gameConfigsFile = "/GameConfigs.dat";

	/// <summary>
	/// Desperta esta instância.
	/// </summary>
	void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            UpdateDataVersion();
            LoadGameProgress();
			LoadGameConfigs();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

	/// <summary>
	/// Ativa o destroy event.
	/// </summary>
	void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	/// <summary>
	/// Atualiza a versão do formato de dados.
	/// </summary>
	private void UpdateDataVersion()
    {
        if (File.Exists(Application.persistentDataPath + dataVersionFile) == true)
        {
            BinaryFormatter bfOpen = new BinaryFormatter();
            FileStream fileToOpen = File.Open(Application.persistentDataPath + dataVersionFile, FileMode.Open);
            DataVersion version = (DataVersion)bfOpen.Deserialize(fileToOpen);
            fileToOpen.Close();

            switch (version.major)
            {
                case 1:
					// Os dados armazenados têm a versão 1.x
					// Algum handler para converter dados se for necessário...
					break;
            }
        }
        BinaryFormatter bfCreate = new BinaryFormatter();
        FileStream fileToCreate = File.Create(Application.persistentDataPath + dataVersionFile);
        bfCreate.Serialize(fileToCreate, dataVersion);
        fileToCreate.Close();
    }

	/// <summary>
	/// Exclui o arquivo com os dados do jogo salvados. Apenas para debug
	/// </summary>
	public void DeleteGameProgress()
	{
		File.Delete(Application.persistentDataPath + gameProgressFile);
		progress = new GameProgressData();
		Debug.Log("Saved game progress deleted");
	}

	/// <summary>
	/// Salva o progresso do jogo em arquivo.
	/// </summary>
	public void SaveGameProgress()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + gameProgressFile);
        progress.saveTime = DateTime.Now;
        bf.Serialize(file, progress);
        file.Close();
    }

	/// <summary>
	/// Carrega o progresso do jogo do arquivo.
	/// </summary>
	public void LoadGameProgress()
    {
        if (File.Exists(Application.persistentDataPath + gameProgressFile) == true)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + gameProgressFile, FileMode.Open);
            progress = (GameProgressData)bf.Deserialize(file);
            file.Close();
        }
    }

	/// <summary>
	/// Salva as configurações do jogo em arquivo.
	/// </summary>
	public void SaveGameConfigs()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + gameConfigsFile);
		bf.Serialize(file, configs);
		file.Close();
	}

	/// <summary>
	/// Carrega as configurações do jogo do arquivo.
	/// </summary>
	public void LoadGameConfigs()
	{
		if (File.Exists(Application.persistentDataPath + gameConfigsFile) == true)
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + gameConfigsFile, FileMode.Open);
			configs = (GameConfigurations)bf.Deserialize(file);
			file.Close();
		}
	}
}
