using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class VisualizerSettings : MonoBehaviour
{
    public static VisualizerSettings Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<PositionColorVisualizer> TilePositionColorVisualizers = new List<PositionColorVisualizer>();
    public List<PositionColorVisualizer> TankPositionColorVisualizers = new List<PositionColorVisualizer>();

    public List<TurretVisualizer> TankTurretColorVisualizers = new List<TurretVisualizer>();

    public bool UseTilePositionAuthorityColor { get; private set; }
    public bool UseTilePositionCheckOutColor { get; private set; }
    public bool UseTileTurretCheckOutColor { get; private set; }


    public bool UseTankPositionAuthorityColor { get; private set; }
    public bool UseTankPositionCheckOutColor { get; private set; }
    public bool UseTankTurretAuthorityColor { get; private set; }
    public bool UseTankTurretCheckOutColor { get; private set; }

    public bool UseTankPositionTransitionParticles { get; private set; }
    public bool UseTankTurretTransitionParticles { get; private set; }


    public void ChangeTilePositionAuthorityColorUse(bool value)
    {
        UseTilePositionAuthorityColor = value;

        foreach (PositionColorVisualizer visualizer in TilePositionColorVisualizers)
        {
            visualizer.SetColor();
        }
    }

    public void ChangeTilePositionCheckOutColorUse(bool value)
    {
        UseTilePositionCheckOutColor = value;
    }

    public void ChangeTileTurretCheckOutColorUse(bool value)
    {
        UseTileTurretCheckOutColor = value;
    }

    public void ChangeTankPositionAuthorityColorUse(bool value)
    {
        UseTankPositionAuthorityColor = value;

        foreach (PositionColorVisualizer visualizer in TankPositionColorVisualizers)
        {
            visualizer.SetColor();
        }
    }

    public void ChangeTankPositionCheckOutColorUse(bool value)
    {
        UseTankPositionCheckOutColor = value;
    }

    public void ChangeTankTurretAuthorityColorUse(bool value)
    {
        UseTankTurretAuthorityColor = value;

        foreach (TurretVisualizer visualizer in TankTurretColorVisualizers)
        {
            visualizer.SetColor();
        }
    }

    public void ChangeTankTurretCheckOutColorUse(bool value)
    {
        UseTankTurretCheckOutColor = value;
    }

    public void ChangeTankPositionTransitionParticlesUse(bool value)
    {
        UseTankPositionTransitionParticles = value;
    }

    public void ChangeTankTurretTransitionParticlesUse(bool value)
    {
        UseTankTurretTransitionParticles = value;
    }


}
