using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera), typeof(CinemachineConfiner))]
public class VirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;
    private Camera mainCamera;

    public async void Awake()
    {
        mainCamera = Camera.main;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner>();
        Player player = await ManagerHub.Instance.ServerManager.WaitForThisPlayerAsync();

        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }
}
