using System.Collections;
using System.Collections.Generic;
using Improbable.Demo;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityClient)]
public class TurretVisualizer : MonoBehaviour {

    [Require]
    private Improbable.Demo.TurretInfo.Reader TurretInfoReader;

    [SerializeField]
    private List<MeshRenderer> turretRenderers;

    [SerializeField]
    private GameObject turretRoot;

    [SerializeField]
    private ParticleSystem particles;

    private float rotationAngle, lastRotationAngle;
    private int framesSameRotation;

    private void OnEnable()
    {
        SetColor();

        rotationAngle = TurretInfoReader.Data.rotation;
        turretRoot.transform.localRotation = Quaternion.AngleAxis(TurretInfoReader.Data.rotation, Vector3.up);

        TurretInfoReader.ColorIdUpdated.Add(OnColorIdUpdated);
        TurretInfoReader.RotationUpdated.Add(OnRotationUpdated);

        VisualizerSettings.Instance.TankTurretColorVisualizers.Add(this);

    }

    private void OnDisable()
    {
        TurretInfoReader.ColorIdUpdated.Remove(OnColorIdUpdated);
        TurretInfoReader.RotationUpdated.Remove(OnRotationUpdated);

        VisualizerSettings.Instance.TankTurretColorVisualizers.Remove(this);
    }

    private void FixedUpdate()
    {
        if (Mathf.Approximately(lastRotationAngle, rotationAngle))
        {
            ++framesSameRotation;
        }
        else
        {
            if (framesSameRotation > 10)
            {
                Debug.LogWarning(string.Format("Entity {0} had same rotation for {1} frames!", gameObject.EntityId(), framesSameRotation));
            }

            framesSameRotation = 0;
        }

        lastRotationAngle = rotationAngle;
    }

    private void OnLosingAuth(LosingAuthInfo info)
    {
        if (particles != null && VisualizerSettings.Instance.UseTankTurretTransitionParticles)
        {
            particles.Play();
        }
    }

    private void OnColorIdUpdated(uint newColorId)
    {
        SetColor();
    }

    public void SetColor()
    {
        foreach (MeshRenderer renderer in turretRenderers)
        {
            renderer.material.color = WorkerColor.GetcolorFromId(VisualizerSettings.Instance.UseTankTurretAuthorityColor ? TurretInfoReader.Data.colorId : 0);
        }

        if (particles != null)
        {
            particles.startColor = WorkerColor.GetcolorFromId(TurretInfoReader.Data.colorId);
        }
    }

    private void OnRotationUpdated(float rotation)
    {
        rotationAngle = rotationAngle = TurretInfoReader.Data.rotation;
        turretRoot.transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.up);
    }
}
