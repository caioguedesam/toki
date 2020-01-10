using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's coordinate
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineIgnoreAxis : CinemachineExtension
{
    public bool ignoreX = false;
    public bool ignoreY = false;

    private float initialXPosition, initialYPosition;

    private void Start()
    {
        initialXPosition = VirtualCamera.State.RawPosition.x;
        initialYPosition = VirtualCamera.State.RawPosition.y;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;

            if(ignoreX)
            {
                pos.x = initialXPosition;
            }
            if(ignoreY)
            {
                pos.y = initialYPosition;
            }
            state.RawPosition = pos;
        }
    }
}