using System.Collections;
using System.Collections.Generic;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Assets.Gamelogic.Player
{
    public class FlyCam : MonoBehaviour
    {
        [Require]
        private Position.Writer PositionWriter;

        private float yaw;
        private float pitch;
        private const float movementSpeed = 15.0f;

        private GameObject SpectatorCamera;

        void OnEnable()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            SpectatorCamera = GameObject.Find("Main Camera");
            SpectatorCamera.transform.SetParent(this.transform);
            SpectatorCamera.transform.localPosition = Vector3.zero;
            SpectatorCamera.transform.localRotation = Quaternion.identity;

            this.transform.position = Vector3.up;

            SpectatorCamera.SetActive(true);
        }

        void Update()
        {
            HandleRotationMovement();
            HandlePositionMovement();
            var rot = transform.rotation;
            //PositionWriter.Send(new Position.Update().SetCoords(transform.position.ToCoordinates()));
            //RotationWriter.Send(new Rotation.Update().SetRotation(new Improbable.Global.Quaternion(rot.x, rot.y, rot.z, rot.w)));
        }

        private void HandlePositionMovement()
        {
            Vector3 targetDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                targetDirection += transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                targetDirection -= transform.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                targetDirection -= transform.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                targetDirection += transform.right;
            }
            transform.position += targetDirection * movementSpeed * Time.deltaTime;
        }

        private void HandleRotationMovement()
        {
            if (Input.GetMouseButton(1))
            {
                yaw = (yaw + Input.GetAxis("Mouse X")) % 360f;
                pitch = (pitch - Input.GetAxis("Mouse Y")) % 360f;
                transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
            }
        }
    }
}
