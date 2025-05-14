using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    private PlayerController _player;
    public PlayerController Player { get => _player; }

    public HashSet<SlimeGreenController> Greens { get; set; } = new HashSet<SlimeGreenController>();

    private GameObject _playerResource;
    private GameObject _greenResource;

    protected override void Initialize()
    {
        base.Initialize();

        _playerResource = Resources.Load<GameObject>(Define.PlayerPath);
    }

    public void ResourceAllLoad()
    {
        _playerResource = Resources.Load<GameObject>(Define.PlayerPath);
        _greenResource = Resources.Load<GameObject>(Define.SlimeGreenPath);
    }

    public T Spawn<T>(Vector3 spawnPos) where T : BaseController
    {
        Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            GameObject obj = Instantiate(_playerResource, spawnPos, Quaternion.identity);
            PlayerController playerController = obj.GetOrAddComponent<PlayerController>();
            _player = playerController;
            Camera.main.GetOrAddComponent<CameraController>();
            return playerController as T;
        }
        else if (type == typeof(SlimeGreenController))
        {
            GameObject obj = Instantiate(_greenResource, spawnPos, Quaternion.identity);
            SlimeGreenController slimeGreenController = obj.GetOrAddComponent<SlimeGreenController>();
            Greens.Add(slimeGreenController);
            return slimeGreenController as T;
        }
        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        obj.gameObject.SetActive(false);
    }

    protected override void Clear()
    {
        base.Clear();
        Greens.Clear();
        _player = null;
        Resources.UnloadUnusedAssets();
    }

}
